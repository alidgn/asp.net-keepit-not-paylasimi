using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeepIT.Entities.Messages;


namespace KeepIT.BusinessLayer
{
    public class BusinessLayerResult<T> where T : class
    {
        public List<ErrorMessageObj> Errors { get; set; }
        public T Result { get; set; }

        public BusinessLayerResult()
        {
            Errors = new List<ErrorMessageObj>();
        }

        public void AddError(ErrorMessageCodes code, string message)
        {
            Errors.Add(new ErrorMessageObj() { Code = code , Message = message});

        }

        //public void AddError(WarningMessageCodes code, string message)
        //{
        //    Errors.Add(new KeyValuePair<WarningMessageCodes, string>(code, message));

        //}

    }
}
