using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeepIT.Entities.Messages
{
    public class ErrorMessageObj
    {
        public ErrorMessageCodes Code { get; set; }
        public string Message { get; set; }
    }
}
