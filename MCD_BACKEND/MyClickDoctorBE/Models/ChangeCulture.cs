using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace MyClickDoctorBE.Models
{
    public  class ChangeCulture
    {
        public static string ChangeLanguage(string Lang)
        {
            string lgcode = "";
            ApiResponse Resp = new ApiResponse();
            if(Lang!="en")
            {
                lgcode = "hu";
            }
            else
            {
                lgcode = "en";
            }
            try
            {
                CultureInfo ci = new CultureInfo(lgcode);
                System.Threading.Thread.CurrentThread.CurrentUICulture = ci;
                System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(ci.Name);
            }
            catch (Exception ex)
            {
                Resp.msg = ex.Message;
            }
            return Lang;
        }
    }
}
