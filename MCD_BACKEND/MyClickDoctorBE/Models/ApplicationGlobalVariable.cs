using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
namespace MyClickDoctorBE.Models
{
    public class ApplicationGlobalVariable
    {
        static IConfiguration conf = (new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build());
        //public static string MailServerHost = conf["Logging:EmailSettings:MailServerHost"].ToString();
        //public static string MailServerPort = conf["Logging:EmailSettings:MailServerPort"].ToString();
        //public static string MailServerUsername = conf["Logging:EmailSettings:MailServerUsername"].ToString();
        //public static string MailServerPassword = conf["Logging:EmailSettings:MailServerPassword"].ToString();
        //public static string MailFromAddress = conf["Logging:EmailSettings:MailFromAddress"].ToString();
        //public static string MailFromName = conf["Logging:EmailSettings:MailFromName"].ToString();

        public static string MailServerHost = "smtp.forpsi.com";
        public static string MailServerPort = "587";
        public static string MailServerUsername = "helpdesk@myclickdoctor.hu";
        public static string MailServerPassword = "hd@MCD13579.1";
        public static string MailFromAddress = "helpdesk@myclickdoctor.hu";
        public static string MailFromName = "myclickdoctor";
        public static string PixelId = conf["Logging:BarionPayments:PixelId"].ToString();
        public static string version = conf["Logging:VersionSetting:Version"].ToString();
        public static string url = conf["Logging:RegisterLink:Url"].ToString();
        public static string WebsiteUrl = "https://testapi.myclickdoctor.com/";
       // public static string WebsiteUrl = "https://localhost:44365/";
    }
}



