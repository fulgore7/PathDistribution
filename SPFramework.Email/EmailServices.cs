using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace SPFramework.Email
{
    public class EmailServices
    {
        public static bool IsValidEmail(string sEmailAddress)
        {
            Regex r = new Regex(@"^(([A-Za-z0-9]+_+)|([A-Za-z0-9]+\-+)|([A-Za-z0-9]+\.+)|([A-Za-z0-9]+\++))*[A-Za-z0-9]+@((\w+\-+)|(\w+\.))*\w{1,63}\.[a-zA-Z]{2,6}$");
            return r.Match(sEmailAddress).Success;
        }

        public static void SendEmail(string host, int port, bool useSSL, string from, List<string> to, string subject, string body)
        {
            using (SmtpClient client = new SmtpClient(host))
            {
                using (MailMessage message = new MailMessage())
                {
                    message.From = new MailAddress(from);
                    message.To.Add(string.Join(",", to));
                    message.Subject = subject;
                    message.Body = body;

                    client.Send(message);
                }
            }
        }

        public static void SendEmail(List<string> to, string subject, string body)
        {
            using (SmtpClient client = new SmtpClient("smtp.office365.com", 587))
            {
                client.Credentials = new NetworkCredential("app@sarapath.com", "W1!2@Zabc");
                client.EnableSsl = true;

                using (MailMessage message = new MailMessage())
                {
                    message.From = new MailAddress("app@sarapath.com");
                    message.To.Add(string.Join(",", to));
                    message.Subject = subject;
                    message.Body = body;

                    client.Send(message);
                }
            }
        }

        //public static void SendEmailWithPDF(List<string> to, string subject, string body, string attachmentName, Stream attachment)
        //{
        //    using (SmtpClient client = new SmtpClient("smtp.office365.com", 587))
        //    {
        //        client.Credentials = new NetworkCredential("app@sarapath.com", "W1!2@Zabc");
        //        client.EnableSsl = true;

        //        using (MailMessage message = new MailMessage())
        //        {
        //            message.From = new MailAddress("app@sarapath.com");
        //            message.To.Add(string.Join(",", to));
        //            message.Subject = subject;
        //            message.Body = body;
        //            message.Attachments.Add(new Attachment(attachment, attachmentName, System.Net.Mime.MediaTypeNames.Application.Pdf));
        //            client.Send(message);
        //        }
        //    }
        //}
    }
}
