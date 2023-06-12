using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailFunctionApp
{
    public class QueueMessageDto
    {
        public string ToEmail { get; set; }
        public string CcEmail { get; set; }
        public string EmailSubject { get; set; }
        public string EmailTemplate { get; set; }
    }
}
