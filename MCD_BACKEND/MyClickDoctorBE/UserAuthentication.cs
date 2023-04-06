using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MyClickDoctorBE.DatabaseEntity;
using MyClickDoctorBE.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyClickDoctorBE
{
    public class UserAuthentication
    {
        private readonly RequestDelegate _next;
        private readonly string[] PublicPath = { "api/values" };
        private readonly IHostingEnvironment _appEnvironment;
        public UserAuthentication(RequestDelegate next, IHostingEnvironment appEnvironment)
        {
            _next = next;
            _appEnvironment = appEnvironment;
        }
        public async Task Invoke(HttpContext context)
        {
            string authHeader = context.Request.Headers["Authorization"];
            string lang = context.Request.Headers["Lang"];
            string Status = context.Request.Headers["Status"];
           // string HostName="";
           // string ApiName = "";
            //if (context.Request.Headers.ContainsKey("Origin"))
            //{
            //     HostName= context.Request.Headers.FirstOrDefault(u => u.Key == "Origin").Value;
            //}
            //if (context.Request.Headers.ContainsKey("path"))
            //{
                //ApiName = context.Request.Path.Value;
            //}
          //  header obj = new header();
            //obj.ApiName = ApiName;
            //obj.HostName = HostName;
            //obj.today = DateTime.UtcNow;
            //obj.authHeader = authHeader;
            //string path = Path.Combine(_appEnvironment.WebRootPath, @"Images");
            //string Completefile = path + "/" + "J" + DateTime.UtcNow.Ticks + "" + "." + "json";
            //System.IO.File.WriteAllText(Completefile, JsonConvert.SerializeObject(obj));
            ChangeCulture.ChangeLanguage(lang);
            if (Status == "1")
            {
                if (authHeader != null)
                {
                    AppDbCommonLogic db = new AppDbCommonLogic();
                    if (!db.Isvalid(authHeader))
                    {
                        context.Response.StatusCode = 401;
                        // await _next.Invoke(context);
                        return;
                    }
                    else
                    {
                        await _next.Invoke(context);
                    }
                }
                else
                {
                    context.Response.StatusCode = 401;
                    // no authorization header
                    //Unauthorized
                    // await _next.Invoke(context);
                    return;
                }
            }
            else
            {
                await _next.Invoke(context);
            }
        }
    }
}
