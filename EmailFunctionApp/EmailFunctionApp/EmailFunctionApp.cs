using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.Logging;

namespace EmailFunctionApp

{
    public class EmailFunctionApp
    {
        private readonly ILogger _logger;
        public EmailFunctionApp(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<EmailFunctionApp>();
        }
        [FunctionName("EmailFunctionApp")]
        public void Run([QueueTrigger("myqueue-items", Connection = "StorageConnectionString")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
            var options = new JsonSerializerOptions
            {
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true,
            };
            EmailDetailsDto email = JsonSerializer.Deserialize<EmailDetailsDto>(myQueueItem, options);
            try {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(Environment.GetEnvironmentVariable("FromName"), Environment.GetEnvironmentVariable("FromAddress")));
                message.Subject = email.EmailSubject;
                if (email.ToEmail != null)
                {
                    email.ToEmail = email.ToEmail.ToList();
                    foreach (string mailid in email.ToEmail)
                        message.To.Add(new MailboxAddress("", mailid));
                }
                if (email.CcEmail != null && email.ToEmail != null)
                {
                    email.CcEmail = email.CcEmail.Except(email.ToEmail).ToList();
                    email.CcEmail = email.CcEmail.Distinct().ToList();
                    foreach (string mailid in email.CcEmail)
                        message.Cc.Add(new MailboxAddress("", mailid));
                }
            }
            catch(Exception ex)
            {
                _logger.LogError("EmailFunctionAppError:" + ex.ToString());
            }

        }
    }
}
