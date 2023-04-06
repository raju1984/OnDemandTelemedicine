using MyClickDoctorBE.App_Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace MyClickDoctorBE.Models
{
    public class AppCommonLogic
    {
        public class AES_ALGORITHM
        {
            internal string Encrypt(string clearText)
            {
                string EncryptionKey = "sadvahvdwed";//ConfigurationManager.AppSettings["Encryptionkey"];
                byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(clearBytes, 0, clearBytes.Length);
                            cs.Close();
                        }
                        clearText = Convert.ToBase64String(ms.ToArray());
                    }
                }
                return clearText;
            }

            internal string Decrypt(string cipherText)
            {
                cipherText = cipherText.Replace(" ", "+");
                string EncryptionKey = "sadvahvdwed";
                byte[] cipherBytes = Convert.FromBase64String(cipherText);
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(cipherBytes, 0, cipherBytes.Length);
                            cs.Close();
                        }
                        cipherText = Encoding.Unicode.GetString(ms.ToArray());
                    }
                }
                return cipherText;
            }
        }
        //public static bool SendForgetPassword(string email, string name, string password)
        //{
        //    bool IsSend = false;
        //    try
        //    {
        //        string MailServerHost = "mail.adequateinfosoft.com"; //"smtp.gmail.com";
        //        string MailServerPort = "25";
        //        string MailServerUsername = "contact@adequateinfosoft.com";
        //        string MailServerPassword = "Hr7!7fb4";
        //        string MailFromAddress = "contact@adequateinfosoft.com";
        //        string MailFromName = "My Click Doctor";
        //        var client = new SmtpClient(MailServerHost, Convert.ToInt32(MailServerPort))
        //        {
        //            Credentials = new NetworkCredential(MailServerUsername, MailServerPassword),
        //            EnableSsl = false,
        //        };
        //        //MailMessage mail = new MailMessage(new MailAddress(MailFromAddress, MailFromName),);
        //        MailMessage mail = new MailMessage(new MailAddress(MailFromAddress, MailFromName), new MailAddress(email));
        //        mail.Subject = "Forget Password";
        //        mail.Body = string.Format("Hi {0},<br><br>We received a request to reset the password for " +
        //            "your account associated with this email address.<br><br>Your temporary password is:<b>{1}</b><br><br>Please open the web and sign in to your account with this temporary password. then you can reset your password" +
        //            "<br><br>Sincerely,<br>{2}", name, password, MailFromName);
        //        mail.IsBodyHtml = true;
        //        //client.UseDefaultCredentials = true;
        //        client.Send(mail);
        //        IsSend = true;
        //        return IsSend;
        //    }
        //    catch (Exception ex)
        //    {
        //        IsSend = false;
        //    }
        //    return IsSend;
        //}
        public static bool SendForgetPassword(string email, string name, string UserId, int Type)
        {
            bool IsSend = false;
            int id = 2;
            string Path = "https://test.myclickdoctor.com/reset-pass?UserId=" + UserId + "&Type=" + Type + "&id=" + id + "";
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
                    EnableSsl = true,
                };
                //MailMessage mail = new MailMessage(new MailAddress(MailFromAddress, MailFromName),);
                MailMessage mail = new MailMessage(new MailAddress(MailFromAddress, MailFromName), new MailAddress(email));
                mail.Subject = AppResource.ForgetPassword;
                mail.Body = string.Format(AppResource.Dear + " {0},<br><br>" +
                AppResource.forgetblow + "<br><br>" +
                "<a href='" + Path + "'>'" + AppResource.click_here + "'</a>" + "<br><br>" +
                AppResource.browserlink + ":<br><br>" + Path + "<br><br>" +
                AppResource.forgetDiscard +
               "<br><br>" + AppResource.change_sincyerly + ",<br>{2}", name, email, AppResource.cahnge_team);
                mail.IsBodyHtml = true;
                //client.UseDefaultCredentials = true;

                client.Send(mail);
                IsSend = true;
                //   return IsSend;
            }
            catch (Exception ex)
            {
                IsSend = false;
                //  _log.LogInformation(ex.Message);
                //string path = Path.Combine(_appEnvironment.WebRootPath, @"Images");
                //string Completefile = path + "/" + "J" + DateTime.UtcNow.Ticks + "" + "." + "json";
                //System.IO.File.WriteAllText(Completefile, JsonConvert.SerializeObject(ex.Message));
            }
            return IsSend;
        }
        public static bool SendEmail(string email, string name, string UserId, int Type)
        {
            bool IsSend = false;
            string Role = "";
            int id = 1;
            string Path = "https://test.myclickdoctor.com/reset-pass?UserId=" + UserId + "&Type=" + Type + "&id=" + id + "";
            if (Type == 2)
            {
                Role = AppResource.Doctor;
            }
            else if (Type == 5)
            {
                Role = AppResource.PharmaRepresentative;
            }
            else if (Type == 3)
            {
                Role = AppResource.PharmaceuticalCompany;
            }
            else if (Type == 4)
            {
                Role = AppResource.Patient;
            }
            else if (Type == 6)
            {
                Role = AppResource.Nurse;
            }
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
                    EnableSsl = true,
                };
                //MailMessage mail = new MailMessage(new MailAddress(MailFromAddress, MailFromName),);
                MailMessage mail = new MailMessage(new MailAddress(MailFromAddress, MailFromName), new MailAddress(email));
                mail.Subject = AppResource.RegistrationConfirmation;
                mail.Body = string.Format("Hi [{0}],<br><br>" +
                    AppResource.medicalregister + AppResource.belowdetail + "<br><br>" +
                    AppResource.UserName + ":[{1}]<br>" +
                    AppResource.clicklink + ": " + "<a href='" + Path + "'>'" + AppResource.click_here + "'</a>" +
                    "<br><br>" + AppResource.BestRegard + ",<br>{2}", name, email, AppResource.cahnge_team);
                mail.IsBodyHtml = true;
                //client.UseDefaultCredentials = true;
                client.Send(mail);
                IsSend = true;
                return IsSend;
            }
            catch (Exception ex)
            {
                IsSend = false;
            }
            return IsSend;
        }
        //public static bool SendEmailToDoctor(string email, string name, string UserId, int Type)
        //{
        //    bool IsSend = false;
        //    string Role = "";
        //    int id = 1;
        //    string Path = "https://test.myclickdoctor.com/reset-pass?UserId=" + UserId + "&Type=" + Type + "&id=" + id + "";
        //    string Path1 = "https://doctor.medicalscan.hu/mcdreg/doc";
        //    if (Type == 2)
        //    {
        //        Role = AppResource.Doctor;
        //    }
        //    else if (Type == 5)
        //    {
        //        Role = AppResource.PharmaRepresentative;
        //    }
        //    else if (Type == 3)
        //    {
        //        Role = AppResource.PharmaceuticalCompany;
        //    }
        //    else if (Type == 4)
        //    {
        //        Role = AppResource.Patient;
        //    }
        //    else if (Type == 6)
        //    {
        //        Role = AppResource.Nurse;
        //    }
        //    try
        //    {
        //        string MailServerHost = ApplicationGlobalVariable.MailServerHost; //"smtp.gmail.com";
        //        string MailServerPort = ApplicationGlobalVariable.MailServerPort;
        //        string MailServerUsername = ApplicationGlobalVariable.MailServerUsername;
        //        string MailServerPassword = ApplicationGlobalVariable.MailServerPassword;
        //        string MailFromAddress = ApplicationGlobalVariable.MailFromAddress;
        //        string MailFromName = ApplicationGlobalVariable.MailFromName;
        //        var client = new SmtpClient(MailServerHost, Convert.ToInt32(MailServerPort))
        //        {
        //            Credentials = new NetworkCredential(MailServerUsername, MailServerPassword),
        //            EnableSsl = true,
        //        };
        //        //MailMessage mail = new MailMessage(new MailAddress(MailFromAddress, MailFromName),);
        //        MailMessage mail = new MailMessage(new MailAddress(MailFromAddress, MailFromName), new MailAddress(email));
        //        mail.Subject = AppResource.RegistrationConfirmation;
        //        mail.Body = string.Format("Kedves Dr." + "{0}<br><br>" +
        //          "Örömmel értesítjük, hogy sikeresen regisztráltuk a myclickdoctorban." + " <br><br>" +
        //          "A myclickdoctor használatához szükséges felhasználói útmutatókat az alábbi oldalon találja:" + "<br>" +
        //          "<a href='" + Path1 + "'>'" + Path1 + "'</a>" + "<br>" +
        //          "Kérjük, bejelentkezés előtt figyelmesen olvassa el a felhasználói útmutatókat!" + "<br><br>" +
        //          "Bejelentkezési adatai a következőek:" + "<br>" +
        //          "Felhasználónév " + ": {1}<br>" +
        //          "A jelszó létrehozásához és a bejelentkezéshez kattintson az alábbi linkre" + ": " + "<a href='" + Path + "'>'" + "Kattintson ide" + "'</a>" + "<br><br>" +
        //          "Kérdés, probléma esetén keressen minket bizalommal elérhetőségeink valamelyikén!" +
        //          "<br><br>" + "Üdvözlettel" + ",<br>{2}" + "<br>" +
        //          "+36 1 336 2104" + "<br>" +
        //          "helpdesk@myclickdoctor.hu", name, email, AppResource.cahnge_team);
        //        mail.IsBodyHtml = true;
        //        //client.UseDefaultCredentials = true;
        //        client.Send(mail);
        //        IsSend = true;
        //        return IsSend;
        //    }
        //    catch (Exception ex)
        //    {
        //        IsSend = false;
        //    }
        //    return IsSend;
        //}
        public static bool SendEmailToDoctor(string email, string name, string UserId, int Type)
        {
            bool IsSend = false;
            string Role = "";
            int id = 1;
            string Path = "https://test.myclickdoctor.com/reset-pass?UserId=" + UserId + "&Type=" + Type + "&id=" + id + "";
            string Path1 = "https://doctor.medicalscan.hu/mcdreg/doc";
            if (Type == 2)
            {
                Role = AppResource.Doctor;
            }
            else if (Type == 5)
            {
                Role = AppResource.PharmaRepresentative;
            }
            else if (Type == 3)
            {
                Role = AppResource.PharmaceuticalCompany;
            }
            else if (Type == 4)
            {
                Role = AppResource.Patient;
            }
            else if (Type == 6)
            {
                Role = AppResource.Nurse;
            }
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
                    EnableSsl = true,
                };
                //MailMessage mail = new MailMessage(new MailAddress(MailFromAddress, MailFromName),);
                MailMessage mail = new MailMessage(new MailAddress(MailFromAddress, MailFromName), new MailAddress(email));
                mail.Subject = "Erősítse meg myclickdoctor regisztrációját!";
                mail.Body = string.Format("Kedves Dr." + "{0}<br><br>" +
                  "Köszöntjük Önt myclickdoctor felhasználói körében!" + " <br><br>" +
                  "Regisztrációja megerősítéséhez kattintson az alábbi linkre" + ": " + "<a href='" + Path + "'>'" + "Kattintson ide" + "'</a>" + "<br><br>" +
                  "Bejelentkezési adatai a következőek:" + "<br>" +
                  "Felhasználónév " + ": {1}<br>" +
                  "Jelszó: A fenti linkre kattintva tudja megadni jelszavát" +
                  "Kérdés, probléma esetén keressen minket bizalommal elérhetőségeink valamelyikén!" +
                  "<br><br>" + "Üdvözlettel" + ",<br>{2}" + "<br>" +
                  "+36 1 336 2104" + "<br>" +
                  "helpdesk@myclickdoctor.hu", name, email, AppResource.cahnge_team);
                mail.IsBodyHtml = true;
                //client.UseDefaultCredentials = true;
                client.Send(mail);
                IsSend = true;
                return IsSend;
            }
            catch (Exception ex)
            {
                IsSend = false;
            }
            return IsSend;
        }
        public static bool SendEmailToDoctorAgain(string email, string name, string UserId, int Type)
        {
            bool IsSend = false;
            string Role = "";
            int id = 1;
            string Path = "https://test.myclickdoctor.com/reset-pass?UserId=" + UserId + "&Type=" + Type + "&id=" + id + "";
            if (Type == 2)
            {
                Role = AppResource.Doctor;
            }
            else if (Type == 5)
            {
                Role = AppResource.PharmaRepresentative;
            }
            else if (Type == 3)
            {
                Role = AppResource.PharmaceuticalCompany;
            }
            else if (Type == 4)
            {
                Role = AppResource.Patient;
            }
            else if (Type == 6)
            {
                Role = AppResource.Nurse;
            }
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
                    EnableSsl = true,
                };
                //MailMessage mail = new MailMessage(new MailAddress(MailFromAddress, MailFromName),);
                MailMessage mail = new MailMessage(new MailAddress(MailFromAddress, MailFromName), new MailAddress(email));
                mail.Subject = "Erősítse meg myclickdoctor regisztrációját!";
                mail.Body = string.Format("Kedves Dr." + "{0}<br><br>" +
                  "Köszöntjük Önt myclickdoctor felhasználói körében!" + " <br><br>" +
                  "Felhívjuk figyelmét, hogy regisztrációja megerősítésére már csak 24 órája maradt! Ezt az alábbi linkre kattintva tudja megtenni" + ": " + "<a href='" + Path + "'>'" + "Kattintson ide" + "'</a>" + "<br><br>" +
                  "Bejelentkezési adatai a következőek:" + "<br>" +
                  "Felhasználónév " + ": {1}<br>" +
                  "Jelszó: A fenti linkre kattintva tudja megadni jelszavát" +
                  "Kérdés, probléma esetén keressen minket bizalommal elérhetőségeink valamelyikén!" +
                  "<br><br>" + "Üdvözlettel" + ",<br>{2}" + "<br>" +
                  "+36 1 336 2104" + "<br>" +
                  "helpdesk@myclickdoctor.hu", name, email, AppResource.cahnge_team);
                mail.IsBodyHtml = true;
                //client.UseDefaultCredentials = true;
                client.Send(mail);
                IsSend = true;
                return IsSend;
            }
            catch (Exception ex)
            {
                IsSend = false;
            }
            return IsSend;
        }
        public static Address GetLatLong(string City, string Zip)
        {
            Address addressViewModel = new Address();
            string latlong = "", address = "";
            if (City != string.Empty)
            {
                address = "https://maps.google.com/maps/api/geocode/xml?address=" + City + "&sensor=false&key=AIzaSyC6QSJ9PHH06rwALifkIDfx0xpYB2h7PmQ";
            }
            else
            {
                address = "https://maps.googleapis.com/maps/api/geocode/xml?components=postal_code:" + Zip.Trim() + "&sensor=false&key=AIzaSyC6QSJ9PHH06rwALifkIDfx0xpYB2h7PmQ";
            }
            var result = new System.Net.WebClient().DownloadString(address);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(result);
            XmlNodeList parentNode = doc.GetElementsByTagName("location");
            var lat = "";
            var lng = "";
            foreach (XmlNode childrenNode in parentNode)
            {
                lat = childrenNode.SelectSingleNode("lat").InnerText;
                lng = childrenNode.SelectSingleNode("lng").InnerText;
            }
            addressViewModel.Latitude = Convert.ToString(lat);
            addressViewModel.Longitude = Convert.ToString(lng);
            return addressViewModel;
        }
        public static bool SendInvitationMail(string email, string DocName, string City, string Email, string Specility, string Reqname)
        {
            bool IsSend = false;
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
                    EnableSsl = true,
                };
                //MailMessage mail = new MailMessage(new MailAddress(MailFromAddress, MailFromName),);
                MailMessage mail = new MailMessage(new MailAddress(MailFromAddress, MailFromName), new MailAddress("helpdesk@medicalscan.hu"));
                mail.Subject = AppResource.Invitation;
                mail.Body = string.Format(AppResource.dearteam + "<br><br>" +
                    AppResource.invitemsg + "<br><br>" +
                    AppResource.DoctorName + ": {0}<br>" +
                    AppResource.City + ": {1}<br>" +
                    AppResource.Email + ": {2}<br>" +
                    AppResource.Speciality + ": {3}<br>" +
                    AppResource.contactdoctor +
                    "<br><br>" + AppResource.change_sincyerly + ",<br>{4}", DocName, City, Email, Specility, Reqname);
                mail.IsBodyHtml = true;
                //client.UseDefaultCredentials = true;
                client.Send(mail);
                IsSend = true;
                return IsSend;
            }
            catch (Exception ex)
            {
                IsSend = false;
            }
            return IsSend;
        }
        public static bool SendEmailChangePassword(string email, string password, string Name)
        {
            bool IsSend = false;
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
                    EnableSsl = true,
                };
                //MailMessage mail = new MailMessage(new MailAddress(MailFromAddress, MailFromName),);
                MailMessage mail = new MailMessage(new MailAddress(MailFromAddress, MailFromName), new MailAddress(email));
                mail.Subject = AppResource.ChangePassword;
                mail.Body = string.Format(AppResource.cahnge_dear + " {3},<br><br>"
                   + AppResource.changepass_msg +
                    "<br><br>" + AppResource.change_sincyerly + ",<br>{2}", email, password, AppResource.cahnge_team, Name);
                mail.IsBodyHtml = true;
                client.Send(mail);
                IsSend = true;
                return IsSend;
            }
            catch (Exception ex)
            {
                IsSend = false;
            }
            return IsSend;
        }
        public static bool SendEmailCancelMeeting(string email, string MyName, string You, string CancelledBy, DateTime Time, int? Duration, string Title)
        {
            bool IsSend = false;
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
                    EnableSsl = true,
                };
                MailMessage mail = new MailMessage(new MailAddress(MailFromAddress, MailFromName), new MailAddress(email));
                mail.Subject = AppResource.CancelMeeting;
                mail.Body = string.Format(AppResource.cahnge_dear + " {0},<br><br>" +
                  AppResource.cancel_msg + "<br><br>" +
                   AppResource.AppointmentTitle + ":{3}<br>" +
                //  AppResource.ApointmentTime + ":{1}<br>" +
                  AppResource.Duration + ":{2}<br>" +
                  AppResource.Appointmentwith + ":{5}" + "<br><br>" +
                  AppResource.newAppointment + "<br><br>" +
                  AppResource.sorry + "<br><br>" +
                    "<br><br>" + AppResource.change_sincyerly + ",<br>{4}", MyName, Time, Duration, Title, AppResource.cahnge_team, You);
                mail.IsBodyHtml = true;
                client.Send(mail);
                IsSend = true;
                return IsSend;
            }
            catch (Exception ex)
            {
                IsSend = false;
            }
            return IsSend;
        }
        public static bool SendEmailProfileRequest(string Name, string Message)
        {
            bool IsSend = false;
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
                    EnableSsl = true,
                };
                //MailMessage mail = new MailMessage(new MailAddress(MailFromAddress, MailFromName),);
                MailMessage mail = new MailMessage(new MailAddress(MailFromAddress, MailFromName), new MailAddress("helpdesk@medicalscan.hu"));
                mail.Subject = "Request For Profile Change";
                mail.Body = string.Format("'" + AppResource.cahnge_dear + "' {0},<br><br>" +
                    "'" + Name + "' has requested you to change the profile data. Below are the details.<br>" +
                    "'" + Message + "'" +
                    "<br><br>'" + AppResource.change_sincyerly + "',<br>{1}", AppResource.cahnge_team, Name);
                mail.IsBodyHtml = true;
                //client.UseDefaultCredentials = true;
                client.Send(mail);
                IsSend = true;
                return IsSend;
            }
            catch (Exception ex)
            {
                IsSend = false;
            }
            return IsSend;
        }
        public static string ToHexString(string str)
        {
            var sb = new StringBuilder();

            var bytes = Encoding.Unicode.GetBytes(str);
            foreach (var t in bytes)
            {
                sb.Append(t.ToString("X2"));
            }

            return sb.ToString();
        }
        public static string FromHexString(string hexString)
        {
            var bytes = new byte[hexString.Length / 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }

            return Encoding.Unicode.GetString(bytes);
        }
        public static string ConvertStringToHex(String input, System.Text.Encoding encoding)
        {
            Byte[] stringBytes = encoding.GetBytes(input);
            StringBuilder sbBytes = new StringBuilder(stringBytes.Length * 2);
            foreach (byte b in stringBytes)
            {
                sbBytes.AppendFormat("{0:X2}", b);
            }
            return sbBytes.ToString();
        }
        public static bool SendEmailProfileRequestVerification(string email, string Name)
        {
            bool IsSend = false;
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
                    EnableSsl = true,
                };
                //MailMessage mail = new MailMessage(new MailAddress(MailFromAddress, MailFromName),);
                MailMessage mail = new MailMessage(new MailAddress(MailFromAddress, MailFromName), new MailAddress(email));
                mail.Subject = AppResource.ProfileConfirmation;
                mail.Body = string.Format(AppResource.cahnge_dear + " {1},<br><br>" +
                    AppResource.ProfileRequest + ".<br>" +
                    "<br>" + AppResource.change_sincyerly + ",<br>{0}", AppResource.cahnge_team, Name);
                mail.IsBodyHtml = true;
                //client.UseDefaultCredentials = true;
                client.Send(mail);
                IsSend = true;
                return IsSend;
            }
            catch (Exception ex)
            {
                IsSend = false;
            }
            return IsSend;
        }
        public static bool SendEmailProfileRequestVerificationDeny(string email, string Name)
        {
            bool IsSend = false;
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
                    EnableSsl = true,
                };
                //MailMessage mail = new MailMessage(new MailAddress(MailFromAddress, MailFromName),);
                MailMessage mail = new MailMessage(new MailAddress(MailFromAddress, MailFromName), new MailAddress(email));
                mail.Subject = AppResource.ProfileConfirmation;
                mail.Body = string.Format(AppResource.cahnge_dear + " {1},<br><br>"
                + AppResource.ProfileRequestDeny + ".<br>" +
                    "<br>" + AppResource.change_sincyerly + ",<br>{0}", AppResource.cahnge_team, Name);
                mail.IsBodyHtml = true;
                client.Send(mail);
                IsSend = true;
                return IsSend;
            }
            catch (Exception ex)
            {
                IsSend = false;
            }
            return IsSend;
        }
        public static bool SendMailOnProfileUpdate(string email)
        {
            bool IsSend = false;
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
                    EnableSsl = true,
                };
                MailMessage mail = new MailMessage(new MailAddress(MailFromAddress, MailFromName), new MailAddress(email));
                mail.Subject = AppResource.subject_changed;
                mail.Body = string.Format(AppResource.HiThere + "<br>" +
                    AppResource.MCDchange + "<br>'" +
                    AppResource.KindlyContact + "<br>'" +
                    AppResource.Thanks +
                    "<br><br>'" + AppResource.change_sincyerly + "',<br>{0}", AppResource.cahnge_team);
                mail.IsBodyHtml = true;
                client.Send(mail);
                IsSend = true;
                return IsSend;
            }
            catch (Exception ex)
            {
                IsSend = false;
            }
            return IsSend;
        }
        public static bool SendAppointmentBookingConfirmation(string email, string MyName, string Appointmentwith, string Title, int Duration, string StartDate = "")
        {
            bool IsSend = false;
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
                    EnableSsl = true,
                };
                MailMessage mail = new MailMessage(new MailAddress(MailFromAddress, MailFromName), new MailAddress(email));
                mail.Subject = AppResource.appointment_confirmationsubject;
                mail.Body = string.Format(AppResource.cahnge_dear + " {0},<br><br>" +
                  AppResource.success_appointment + "<br><br>" +
                  AppResource.AppointmentTitle + ":{1}<br>" +
                  AppResource.StartDate + ":{5}<br>" +
                  AppResource.TimeZone + ":{6}<br>" +
                  AppResource.Duration + ":{2}<br>" +
                  AppResource.Appointmentwith + ":{3}" + "<br><br>" +
                    "<br><br>" + AppResource.change_sincyerly + ",<br>{4}", MyName, Title, Duration, Appointmentwith, AppResource.cahnge_team, StartDate, "Central European Standard Time");
                mail.IsBodyHtml = true;
                client.Send(mail);
                IsSend = true;
                return IsSend;
            }
            catch (Exception ex)
            {
                IsSend = false;
            }
            return IsSend;
        }
        public static bool sendwarningmailtodoctor(string email, string name)
        {
            bool IsSend = false;
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
                    EnableSsl = true,
                };
                //MailMessage mail = new MailMessage(new MailAddress(MailFromAddress, MailFromName),);
                MailMessage mail = new MailMessage(new MailAddress(MailFromAddress, MailFromName), new MailAddress(email));
                mail.Subject = AppResource.warning;
                mail.Body = string.Format(AppResource.Dear + " {0},<br><br>" +
                AppResource.doctorwarningmsg + "<br><br>" +
               "<br><br>" + AppResource.change_sincyerly + ",<br>{2}", name, email, AppResource.cahnge_team);
                mail.IsBodyHtml = true;
                client.Send(mail);
                IsSend = true;
                return IsSend;
            }
            catch (Exception ex)
            {
                IsSend = false;
            }
            return IsSend;
        }
        public static bool sendmorbiditytopatient(string email, string name, int Id)
        {
            bool IsSend = false;
            int Type = 4;
            string Path = "https://test.myclickdoctor.com/morbidity?Id=" + Id + "&Type=" + Type;
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
                    EnableSsl = true,
                };
                MailMessage mail = new MailMessage(new MailAddress(MailFromAddress, MailFromName), new MailAddress(email));
                mail.Subject = AppResource.Morbidity;
                mail.Body = string.Format(AppResource.Dear + " {0},<br><br>" +
                AppResource.Morbiditymsg + "<br><br>" +
               "<a href='" + Path + "'>'" + AppResource.click_here + "'</a>" + "<br><br>" +
               "<br><br>" + AppResource.change_sincyerly + ",<br>{2}", name, email, AppResource.cahnge_team);
                mail.IsBodyHtml = true;
                client.Send(mail);
                IsSend = true;
                return IsSend;
            }
            catch (Exception ex)
            {
                IsSend = false;
            }
            return IsSend;
        }
        public static bool sharemorbiditytoDoctor(string email, string name, int Id, int DocId)
        {
            bool IsSend = false;
            int Type = 2;
            string Path = "https://test.myclickdoctor.com/morbidity?Id=" + Id + "&Type=" + Type + "&DocId=" + DocId;
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
                    EnableSsl = true,
                };
                MailMessage mail = new MailMessage(new MailAddress(MailFromAddress, MailFromName), new MailAddress(email));
                mail.Subject = AppResource.Morbidity;
                mail.Body = string.Format(AppResource.Dear + " {0},<br><br>" +
                AppResource.Morbiditymsg + "<br><br>" +
               "<a href='" + Path + "'>'" + AppResource.click_here + "'</a>" + "<br><br>" +
               "<br><br>" + AppResource.change_sincyerly + ",<br>{2}", name, email, AppResource.cahnge_team);
                mail.IsBodyHtml = true;
                client.Send(mail);
                IsSend = true;
                return IsSend;
            }
            catch (Exception ex)
            {
                IsSend = false;
            }
            return IsSend;
        }
        public static bool OtpEmail(string email, string name, int otp)
        {
            bool IsSend = false;
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
                    EnableSsl = true,
                };
                MailMessage mail = new MailMessage(new MailAddress(MailFromAddress, MailFromName), new MailAddress(email));
                mail.Subject = "OTP Verfication";
                mail.Body = string.Format(AppResource.Dear + " {0},<br><br>" +
                AppResource.EmailAuthMsg + "<br><br>" +
                otp + "<br><br>" +
               "<br><br>" + AppResource.change_sincyerly + ",<br>{2}", name, email, AppResource.cahnge_team);
                mail.IsBodyHtml = true;
                client.Send(mail);
                IsSend = true;
                return IsSend;
            }
            catch (Exception ex)
            {
                IsSend = false;
            }
            return IsSend;
        }
        public static bool SendEmailtodoctorProfileChange(string email, string Name)
        {
            bool IsSend = false;
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
                    EnableSsl = true,
                };
                //MailMessage mail = new MailMessage(new MailAddress(MailFromAddress, MailFromName),);
                MailMessage mail = new MailMessage(new MailAddress(MailFromAddress, MailFromName), new MailAddress(email));
                mail.Subject = AppResource.ProfileConfirmation;
                mail.Body = string.Format(AppResource.cahnge_dear + " {1},<br><br>" +
                    AppResource.Profilechanged + ".<br>" +
                    "<br>" + AppResource.change_sincyerly + ",<br>{0}", AppResource.cahnge_team, Name);
                mail.IsBodyHtml = true;
                //client.UseDefaultCredentials = true;
                client.Send(mail);
                IsSend = true;
                return IsSend;
            }
            catch (Exception ex)
            {
                IsSend = false;
            }
            return IsSend;
        }
        public static bool SendTimeCreationEmailToHelpdesk(string DocName, string DocCategory, string Specility, string email, string phone, string StartDate, int Duration, string Type, string Topic)
        {
            bool IsSend = false;
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
                    EnableSsl = true,
                };
                //helpdesk@myclickdoctor.hu
                MailMessage mail = new MailMessage(new MailAddress(MailFromAddress, MailFromName), new MailAddress("helpdesk@myclickdoctor.hu"));
                mail.Subject = AppResource.TimeslotcreationemailSubject;
                mail.Body = string.Format(AppResource.dearteam + "<br><br>" +
                    AppResource.Timeslotmsg + "<br><br>" +
                    AppResource.DoctorName + ": {0}<br>" +
                    AppResource.Doctor_Category + ": {1}<br>" +
                    AppResource.Speciality + ": {2}<br>" +
                    AppResource.Email + ": {3}<br>" +
                     AppResource.Phone + ": {4}<br>" +
                      AppResource.StartDate + ": {5}<br>" +
                       AppResource.Duration + ": {6}<br>" +
                        AppResource.Type + ": {7}<br>" +
                         AppResource.Topic + ": {8}<br>" +
                    "<br><br>" + AppResource.change_sincyerly + ",<br>{0}", DocName, DocCategory, Specility, email, phone, StartDate, Duration+ " perc", Type, Topic);
                mail.IsBodyHtml = true;
                //client.UseDefaultCredentials = true;
                client.Send(mail);
                IsSend = true;
                return IsSend;
            }
            catch (Exception ex)
            {
                IsSend = false;
            }
            return IsSend;
        }
    }
}
