using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailFunctionApp
{
    public class EmailDetailsDto
    {
        public List<string> ToEmail { get; set; }
        public List<string> CcEmail { get; set; }
        //Email Subject and Email Body Can be saved read Database based on the email type 
        //Below properties are given just for Demo
        public string EmailBody { get; set; }
        public string EmailSubject { get; set; }
    }
}
