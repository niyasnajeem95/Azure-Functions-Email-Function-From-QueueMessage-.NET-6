using Azure;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailFunctionApp
{
    public class EmailAudit
    {
        public string Email { get; set; }
        public string DateTimeStamp { get; set; }
    }
    public class EmailAuditLog : ITableEntity
    {

        public EmailAuditLog(string mailRecipient, string dateTimeStamp)
        {
            this.PartitionKey = mailRecipient;
            this.RowKey = dateTimeStamp;
        }
        public EmailAuditLog() { }
        public string MailRecipient
        {
            get;
            set;
        }

        public string EmailSubject
        {
            get;
            set;
        }
        public string EmailTemplate
        {
            get;
            set;
        }
        public string Status
        {
            get; set;
        }
        private string _partitionKey;
        private string _rowKey;
        private DateTimeOffset _dateTimeOffset;
        private ETag _eTag;
        public string PartitionKey { get => _partitionKey; set => _partitionKey = value; }
        public string RowKey { get => _rowKey; set => _rowKey = value; }
        public DateTimeOffset? Timestamp { get => _dateTimeOffset; set => _dateTimeOffset = DateTime.UtcNow; }
        public ETag ETag { get => _eTag; set => _eTag = value; }

    }
}
