using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MailKit.Net.Smtp;
using MimeKit.Utils;
using Azure;
using Azure.Data.Tables;
using StorageOperations.Interface;
using Microsoft.Azure.Functions.Worker;

namespace EmailFunctionApp
{
    public class EmailFunctionApp
    {
        private readonly ILogger _logger;
        private readonly ITableStorageOperations _tableStorageOperations;
        public TableClient _tableClient;
        string storageConnString = Environment.GetEnvironmentVariable("StorageConnectionString");
        public EmailFunctionApp(ILoggerFactory loggerFactory, ITableStorageOperations tableStorageOperations)
        {
            _tableStorageOperations = tableStorageOperations;
            _logger = loggerFactory.CreateLogger<EmailFunctionApp>();
            _tableClient = new TableClient(storageConnString, "EmailNotificationLog");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="myQueueItem"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("EmailFunctionApp")]
        public async Task Run([QueueTrigger("myqueue-items", Connection = "StorageConnectionString")] string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
            var options = new JsonSerializerOptions
            {
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true,
            };
            string projectDirectory = string.Empty;
            QueueMessageDto email = JsonSerializer.Deserialize<QueueMessageDto>(myQueueItem, options);

            //Demo purpose no null refrence handling
            try
            {
                //Email Status Logging to Table Storage
                EmailAudit emailAudit = new EmailAudit();
                EmailAuditLog auditRecordTo = new EmailAuditLog();

                string timeStampTo = Guid.NewGuid().ToString();
                emailAudit.DateTimeStamp = timeStampTo;
                emailAudit.Email = myQueueItem;

                EmailAuditLog logs = new EmailAuditLog(email.ToEmail, timeStampTo);
                logs.EmailSubject = email.EmailSubject;
                logs.EmailTemplate = email.EmailTemplate;
                logs.Status = "Ready to Send";
                await _tableStorageOperations.CreateAsync(logs, _tableClient);

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(Environment.GetEnvironmentVariable("FromName"), Environment.GetEnvironmentVariable("FromAddress")));
                message.Subject = email.EmailSubject;
                message.To.Add(new MailboxAddress("", email.ToEmail));
                message.Cc.Add(new MailboxAddress("", email.CcEmail));

                var builder = new BodyBuilder();
                builder.HtmlBody = email.EmailTemplate;
                try
                {
                    // send email
                    using var smtp = new SmtpClient();
                    smtp.Connect(Environment.GetEnvironmentVariable("Host"), 25, SecureSocketOptions.None);
                    smtp.Send(message);
                    smtp.Disconnect(true);

                    //Log Sent
                    var filterText = $"PartitionKey eq '{emailAudit.Email}' and RowKey eq '{emailAudit.DateTimeStamp}'";
                    await foreach (var x in _tableStorageOperations.QueryAsync<EmailAuditLog>(filterText, CancellationToken.None, _tableClient))
                    {
                        x.Status = "Sent";
                        await _tableStorageOperations.UpdateAsync(x, _tableClient);

                    }
                    log.LogInformation($"Email Sent: {myQueueItem}");
                }
                catch (Exception ex)
                {
                    _logger.LogError("FN_EmailNotification-Failed:" + ex.ToString());
                    //Log as Failed
                    var filterText = $"PartitionKey eq '{emailAudit.Email}' and RowKey eq '{emailAudit.DateTimeStamp}'";
                    await foreach (var x in _tableStorageOperations.QueryAsync<EmailAuditLog>(filterText, CancellationToken.None, _tableClient))
                    {
                        x.Status = "Failed";
                        await _tableStorageOperations.UpdateAsync(x, _tableClient);

                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("FN_EmailNotification -Error:" + ex.ToString());
            }

        }
    }
}
