using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace SPFramework.Email
{
    public enum AttachmentTypes
    {
        [Description(MediaTypeNames.Text.Plain)]
        Plain,
        [Description(MediaTypeNames.Text.Html)]
        Html,
        [Description(MediaTypeNames.Text.Xml)]
        Xml,
        [Description(MediaTypeNames.Text.RichText)]
        RichText,
        [Description(MediaTypeNames.Application.Soap)]
        Soap,
        [Description(MediaTypeNames.Application.Octet)]
        Octet,
        [Description(MediaTypeNames.Application.Rtf)]
        Rtf,
        [Description(MediaTypeNames.Application.Pdf)]
        Pdf,
        [Description(MediaTypeNames.Application.Zip)]
        Zip,
        [Description(MediaTypeNames.Image.Gif)]
        Gif,
        [Description(MediaTypeNames.Image.Tiff)]
        Tiff,
        [Description(MediaTypeNames.Image.Jpeg)]
        Jpeg
    }
    public class EmailGenerator
    {
        private List<string> To { get; set; }
        private List<string> CC { get; set; }
        private List<string> BCC { get; set; }
        private List<string> ReplyTo { get; set; }
        private List<Attachment> Attachments { get; set; }
        private bool IsBodyHTML { get; set; }

        private string AttachmentTypeDesc(AttachmentTypes value)
        {
            var enumMember = value.GetType().GetMember(value.ToString()).FirstOrDefault();
            var descriptionAttribute = enumMember == null ? default(DescriptionAttribute) : enumMember.GetCustomAttributes(typeof(DescriptionAttribute),true).FirstOrDefault() as DescriptionAttribute;
            return descriptionAttribute == null ? value.ToString() : descriptionAttribute.Description;
        }

        public EmailGenerator()
        {
            To = new List<string>();
            CC = new List<string>();
            BCC = new List<string>();
            ReplyTo = new List<string>();
            Attachments = new List<Attachment>();
            IsBodyHTML = false;
        }

        public void AddTo(string to)
        {
            if (!To.Contains(to))
                To.Add(to);
        }

        public void AddCC(string cc)
        {
            if (!CC.Contains(cc))
                CC.Add(cc);
        }

        public void AddBCC(string bcc)
        {
            if (!BCC.Contains(bcc))
                BCC.Add(bcc);
        }

        public void AddReplyTo(string replyTo)
        {
            if (!ReplyTo.Contains(replyTo))
                ReplyTo.Add(replyTo);
        }

        public void IsBodyHtml(bool bol)
        {
            IsBodyHTML = bol;
        }

        public void AddAttachment(string attachmentName, Stream stream, AttachmentTypes mediaType)
        {
            if (Attachments.Where(x => x.Name == attachmentName).Count() == 0)
                Attachments.Add(new Attachment(stream, attachmentName, AttachmentTypeDesc(mediaType)));
        }

        public void AddAttachment(string attachmentName, Stream stream, AttachmentTypes mediaType, string contentType)
        {
            if (Attachments.Where(x => x.Name == attachmentName).Count() == 0)
                Attachments.Add(new Attachment(stream, attachmentName, AttachmentTypeDesc(mediaType)));
        }

        public void Send (string subject, string body, int priority)
        {
            if (To.Count == 0)
                throw new Exception("Need to provide at least one email address to send to.");

            if (string.IsNullOrEmpty(subject))
                throw new Exception("Need to provide a subject to send using.");

            if (string.IsNullOrEmpty(body))
                throw new Exception("Need to provide a body for the content of the email.");

            using (SmtpClient client = new SmtpClient("smtp.office365.com", 587))
            {
                client.Credentials = new NetworkCredential("app@sarapath.com", "W1!2@Zabc");
                client.EnableSsl = true;

                using (MailMessage message = new MailMessage())
                {
                    message.From = new MailAddress("app@sarapath.com");
                    message.To.Add(string.Join(",", To));
                    
                    if (CC.Count > 0)
                        message.CC.Add(string.Join(", ", CC));

                    if (BCC.Count > 0)
                        message.Bcc.Add(string.Join(", ", BCC));

                    if (ReplyTo.Count > 0)
                        message.ReplyToList.Add(string.Join(", ", ReplyTo));

                    message.Subject = subject;
                    message.Body = body;
                    message.Priority = (MailPriority)Enum.Parse(typeof(MailPriority),priority.ToString());
                    message.IsBodyHtml = IsBodyHTML;

                    if (Attachments.Count > 0)
                    {
                        foreach (var item in Attachments)
                        {
                            message.Attachments.Add(item);
                        }
                    }

                    client.Send(message);
                }
            }
        }
    }
}
