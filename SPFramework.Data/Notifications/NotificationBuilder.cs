using System;
using System.Data;

namespace SPFramework.Data.Notifications
{
    public class NotificationBuilder<T> where T : NotificationWatcher
    {
        protected internal T Query;

        public NotificationBuilder(T obj)
        {
            Query = obj;
        }

        public NotificationBuilder<T> AddTimer(int duration)
        {
            Query.AddTimer(duration);
            return this;
        }

        public NotificationBuilder<T> AddChangeHandler(Action<DataTable> objectChangeMapper)
        {
            Query.OnChange(objectChangeMapper);
            return this;
        }

        public NotificationBuilder<T> AddErrorHandler(Action<Exception> objectErrorMapper)
        {
            Query.OnError(objectErrorMapper);
            return this;
        }

        public NotificationBuilder<T> AddValidator(Action<DataTable> objectValidatorMapper)
        {
            Query.AddValidator(objectValidatorMapper);
            return this;
        }

        public T Setup()
        {
            return Query;
        }

        public T Start()
        {
            Query.Start();

            return Query;
        }
    }
}