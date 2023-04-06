using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

using MyClickDoctorBE.DatabaseEntity;
using MyClickDoctorBE.Models;
using Newtonsoft.Json;

namespace MyClickDoctorBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IHostingEnvironment _appEnvironment;
        public ValuesController(IHostingEnvironment appEnvironment)
        {
            _appEnvironment = appEnvironment;
        }
        // GET api/values
        [HttpGet]
        public IActionResult Get()
        {
            //List<calernderdata> list = new List<calernderdata>();
            //list.Add(new calernderdata { title = "ashok", date = DateTime.UtcNow.ToString() });
            //list.Add(new calernderdata { title = "fawad", date = DateTime.UtcNow.ToString() });
            //var serializer = new JsonSerializer();
            //var stringWriter = new StringWriter();
            //using (var writer = new JsonTextWriter(stringWriter))
            //{
            //    writer.QuoteName = false;
            //    writer.Formatting = Formatting.Indented;
            //    serializer.Serialize(writer, list);
            //}
            //var json = stringWriter.ToString();
            string pass = new AppCommonLogic.AES_ALGORITHM().Encrypt("197");
         //   string pass1 = new AppCommonLogic.AES_ALGORITHM().Decrypt("5");
            // AppCommonLogic.AES_ALGORITHM.Decrypt("plGXLDwZHx9M1+/F1iBSdHL2U88D2/NOQ1VNNSaLXBo=");
            DateTime date = Convert.ToDateTime("2022-03-01 11:30:48.000");
            DateTime startTimeFormate = DateTime.UtcNow; // This  is utc date time
            TimeZoneInfo systemTimeZone = TimeZoneInfo.Local;
            DateTime localDateTime = TimeZoneInfo.ConvertTimeFromUtc(startTimeFormate, systemTimeZone);
            string formattedDate = localDateTime.ToString("yyyy-MM-dd HH:mm");
            //  DateTime myDateTime = DateTime.SpecifyKind(date, DateTimeKind.Utc);
            //univDateTime = DateTime.Parse(startTimeFormate);
            //localDateTime = univDateTime.ToLocalTime();
            var centraltimezone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
            var formattedDate1 = TimeZoneInfo.ConvertTimeFromUtc(startTimeFormate, centraltimezone);
            DateTime now = DateTime.UtcNow.AddDays(-2);
            string dayname = now.DayOfWeek.ToString();
            return Ok(pass);
        }

        // GET api/values/5
        [HttpGet("{email}")]
        public ActionResult<string> Get(string email)
        {
            try
            {
                string MailServerHost = ApplicationGlobalVariable.MailServerHost; //"smtp.gmail.com";
                string MailServerPort = ApplicationGlobalVariable.MailServerPort;
                string MailServerUsername = ApplicationGlobalVariable.MailServerUsername;
                string MailServerPassword = ApplicationGlobalVariable.MailServerPassword;
                string MailFromAddress = ApplicationGlobalVariable.MailFromAddress;
                string MailFromName = ApplicationGlobalVariable.MailFromName;
                var client = new SmtpClient(MailServerHost, Convert.ToInt32(MailServerPort))
                {
                    Credentials = new NetworkCredential(MailServerUsername, MailServerPassword),
                    EnableSsl = false,
                };
                //MailMessage mail = new MailMessage(new MailAddress(MailFromAddress, MailFromName),);
                MailMessage mail = new MailMessage(new MailAddress(MailFromAddress, MailFromName), new MailAddress(email));
                mail.Subject = "You have a Lead";
                string body = string.Format("Hi {4},<br><br>" +
                "Name" + ":  {0}<br>" +
                "Email" + ":  {1}<br>" +
                "Phone" + ":  {2}<br>" +
                "Message" + ":  {3}<br><br>" + "Best Regards" + ",<br>{4}", "", "", "", "", "Adequate Infosoft");
                mail.IsBodyHtml = true;
                client.Send(mail);
            }
            catch (Exception ex)
            {
                return ex.Message;
                string path = Path.Combine(_appEnvironment.WebRootPath, @"Images");
                string Completefile = path + "/" + "J" + DateTime.UtcNow.Ticks + "" + "." + "json";
                System.IO.File.WriteAllText(Completefile, JsonConvert.SerializeObject(ex.Message));
            }
            return "values";
            //string toEmail = string.IsNullOrEmpty(email)
            //                      ? "toEmail"
            //                    : email;
            //MailMessage mail = new MailMessage()
            //{
            //    From = new MailAddress("hello@referhealth.com", "ReferHealth")
            //};
            //mail.To.Add(new MailAddress(toEmail));
            ////mail.CC.Add(new MailAddress(_emailSettings.CcEmail));

            //mail.Subject = "fahad";
            //mail.Body = body;
            //mail.IsBodyHtml = true;
            //mail.Priority = MailPriority.High;

            //using (SmtpClient smtp = new SmtpClient("smtp.live.com", 587))
            //{
            //    smtp.Credentials = new NetworkCredential("hello@referhealth.com", "RankedHealth1!");
            //    smtp.EnableSsl = true;
            //    smtp.Send(mail);
            //}




            //string toEmail = string.IsNullOrEmpty(email)
            //                    ? "toEmail"
            //                  : email;
            //MailMessage mail = new MailMessage()
            //{
            //    From = new MailAddress("hello@referhealth.com", "ReferHealth")
            //};
            //mail.To.Add(new MailAddress(toEmail));
            //mail.Subject = "fahad";
            //mail.Body = body;
            //mail.IsBodyHtml = true;
            //mail.Priority = MailPriority.High;

            //using (SmtpClient smtp = new SmtpClient("smtp.live.com", 587))
            //{
            //    smtp.Credentials = new NetworkCredential("hello@referhealth.com", "RankedHealth1!");
            //    smtp.EnableSsl = true;
            //    smtp.Send(mail);
            //}
            //return "values";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }

    public class calernderdata
    {
        public string title { get; set; }
        public string date { get; set; }
    }
}
