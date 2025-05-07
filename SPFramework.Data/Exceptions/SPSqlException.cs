using System;
using System.Text;

namespace SPFramework.Data.Exceptions
{
    [Serializable]
    public class SPSqlException : ApplicationException
    {
        /// <summary>
        /// Constructs this exception with the specified error message and
        /// root exception that caused the error this exception encapsulates.
        /// </summary>
        /// <param name="message">the error message that describes this exception.</param>
        /// <param name="exception">the root exception this exception encapsulates.</param>
        public SPSqlException(string message, Exception exception)
            : base(FormatErrorMessage(message, exception), exception)
        {
        }

        //Helper Method for displaying error details
        private static string FormatErrorMessage(string message, Exception ex)
        {
            StringBuilder msgBuilder = new StringBuilder(message);
            msgBuilder.AppendFormat("{0}{1}", ex.Message, Environment.NewLine);
            msgBuilder.AppendFormat("Source: {0}{1}", ex.Source, Environment.NewLine);
            msgBuilder.AppendFormat("StackTrace: {0}{1}", ex.StackTrace, Environment.NewLine);
            if (null != ex.InnerException)
            {
                msgBuilder.AppendFormat("Inner Exception: {0}{1}", ex.InnerException.Message, Environment.NewLine);
            }
            return Convert.ToString(msgBuilder);
        }
    }
}