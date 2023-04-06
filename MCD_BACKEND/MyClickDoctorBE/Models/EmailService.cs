using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace MyClickDoctorBE.Models
{
    public interface IEmailService
    {
        void SendEmail(string subject, string body, string toAddress , bool isBodyHtml = true, string Paths="");
    }
    public class EmailService: IEmailService
    {
        public void SendEmail(string subject, string body, string toAddress = "", bool isBodyHtml = true,string Paths="")
        {
            try
            {
                string MailServerHost = ApplicationGlobalVariable.MailServerHost; //"smtp.gmail.com";
                string MailServerPort = ApplicationGlobalVariable.MailServerPort;
                string MailServerUsername = ApplicationGlobalVariable.MailServerUsername;
                string MailServerPassword = ApplicationGlobalVariable.MailServerPassword;
                string MailFromAddress = ApplicationGlobalVariable.MailFromAddress;

                //string MailServerHost = "mail.adequateinfosoft.com"; //"smtp.gmail.com";
                //string MailServerPort = "587";
                //string MailServerUsername = "contact@adequateinfosoft.com";
                //string MailServerPassword = "Hr7!7fb4";
                //string MailFromAddress = "contact@adequateinfosoft.com"; //obj.email;

                string MailFromName = ApplicationGlobalVariable.MailFromName;
                var client = new SmtpClient(MailServerHost, Convert.ToInt32(MailServerPort))
                {
                    Credentials = new NetworkCredential(MailServerUsername, MailServerPassword),
                    EnableSsl = false,
                };
                //MailMessage mail = new MailMessage(new MailAddress(MailFromAddress, MailFromName),);
                MailMessage mail = new MailMessage(new MailAddress(MailFromAddress, MailFromName), new MailAddress(toAddress));
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;
                //client.UseDefaultCredentials = true;
                client.Send(mail);
            }
            catch (Exception ex)
            {
                string path = System.IO.Path.Combine(Paths, @"Clinic");
                string Completefile = path + "/" + "Email-" + DateTime.Now.ToShortDateString() + "" + "." + "json";
                if (!System.IO.File.Exists(Completefile))
                {
                    System.IO.File.WriteAllText(Completefile, JsonConvert.SerializeObject(ex.Message) + Environment.NewLine + "LogTime:- " + DateTime.UtcNow + Environment.NewLine + Environment.NewLine);
                }
                else
                {
                    System.IO.File.AppendAllText(Completefile, JsonConvert.SerializeObject(ex.Message) + Environment.NewLine + "LogTime:- " + DateTime.UtcNow + Environment.NewLine + Environment.NewLine);
                }
            }
        }
    }
}
