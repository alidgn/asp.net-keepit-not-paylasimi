using KeepIT.Common;
using KeepIT.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KeepIT.WebApp.Init
{
    public class WebCommon : ICommon
    {
        public string GetCurrentUsername()
        {
            if(HttpContext.Current.Session["login"] != null)
            {
                KeepITUser user = HttpContext.Current.Session["login"] as KeepITUser;
                return user.Username;
            }

            return "system";
        }
    }
}