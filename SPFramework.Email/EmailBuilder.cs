using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPFramework.Email
{
    public class EmailBuilder<T> where T : EmailGenerator
    {
        protected internal EmailGenerator Query;

        public EmailBuilder()
        {
            Query = new EmailGenerator();
        }

        public EmailBuilder(EmailGenerator rdl)
        {
            Query = rdl;
        }

        public EmailBuilder<T> AddTo(string to)
        {
            Query.AddTo(to);
            return this;
        }

        public EmailBuilder<T> AddCC(string cc)
        {
            Query.AddCC(cc);
            return this;
        }

        public EmailBuilder<T> AddBCC(string bcc)
        {
            Query.AddBCC(bcc);
            return this;
        }

        public EmailBuilder<T> AddReplyTo(string replyTo)
        {
            Query.AddReplyTo(replyTo);
            return this;
        }

        public EmailBuilder<T> IsBodyHtml(bool bol)
        {
            Query.IsBodyHtml(bol);
            return this;
        }

        public EmailBuilder<T> AddAttachment(string attachmentName, Stream stream, AttachmentTypes mediaType)
        {
            Query.AddAttachment(attachmentName, stream, mediaType);
            return this;
        }

        public EmailBuilder<T> AddAttachment(string attachmentName, Stream stream, AttachmentTypes mediaType, string contentType)
        {
            Query.AddAttachment(attachmentName, stream, mediaType);
            return this;
        }

        public void Send(string subject, string body, int priority)
        {
            Query.Send(subject, body, priority);
        }
    }
}
