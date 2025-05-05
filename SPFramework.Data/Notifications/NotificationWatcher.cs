using SPFramework.Data.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;

namespace SPFramework.Data.Notifications
{
    public enum QueryTypes
    {
        Query = 1,
        StoredProcedure = 2
    }

    public class NotificationWatcher
    {
        private Timer _timer;
        private int timerduration;
        private Action<DataTable> OnChangeMapper { get; set; }

        private Action<Exception> OnErrorMapper { get; set; }

        private List<Action<DataTable>> Validators { get; }

        private string ConnectionString { get; }

        private SqlConnection Connection { get; set; }

        private SqlCommand Command { get; set; }

        private SqlDependency SqlDependency { get; set; }

        private string Query { get; }

        private QueryTypes QueryType { get; }

        public NotificationWatcher(string query, QueryTypes queryType, string connectionString, ConnectionType connectType)
        {
            ConnectionString = Common.SetConnectionString(connectionString, connectType);
            Query = query;
            QueryType = queryType;
            SqlDependency.Stop(ConnectionString);
            SqlDependency.Start(ConnectionString);
            Validators = new List<Action<DataTable>>();
        }

        public void AddTimer(int duration)
        {
            timerduration = duration;

            _timer = new Timer(new TimerCallback(Timer_OnChange));
        }

        public void OnChange(Action<DataTable> objectChangeMapper)
        {
            OnChangeMapper = objectChangeMapper;
        }

        public void OnError(Action<Exception> objectErrorMapper)
        {
            OnErrorMapper = objectErrorMapper;
        }

        public void AddValidator(Action<DataTable> objectvalidatorMapper)
        {
            Validators.Add(objectvalidatorMapper);
        }

        public void Start()
        {
            try
            {
                if (OnChangeMapper == null)
                    throw new Exception(@"You must provide a delegate to process the recordset returned with a notification of change.");

                if (string.IsNullOrWhiteSpace(ConnectionString))
                    throw new Exception(@"You must provide a valid connection string to connect to the database.");
            }
            catch (Exception e)
            {
                if (OnErrorMapper == null)
                    throw;
                else
                    OnErrorMapper.Invoke(e);
            }

            RegisterForChanges();
            if (_timer != null)
                _timer.Change(timerduration, Timeout.Infinite);
        }

        public void Stop()
        {
            try
            {
                if (_timer != null)
                    _timer.Change(Timeout.Infinite, Timeout.Infinite);
                SqlDependency.Stop(ConnectionString);
                if (Connection != null &&
                Connection.State == ConnectionState.Open)
                {
                    Connection.Close();
                }
            }
            catch (Exception e)
            {
                if (OnErrorMapper == null)
                    throw;
                else
                    OnErrorMapper.Invoke(e);
            }
        }

        private void RegisterForChanges()
        {
            bool success = false;

            while (!success)
            {
                try
                {
                    if (SqlDependency != null)
                    {
                        SqlDependency.OnChange -= SqlDependency_OnChange;
                        SqlDependency.Stop(ConnectionString);
                        SqlDependency = null;
                    }

                    if (Command != null)
                    {
                        Command.Dispose();
                        Command = null;
                    }

                    if (Connection != null)
                    {
                        Connection.Dispose();
                        Connection = null;
                    }

                    Connection = new SqlConnection(ConnectionString);

                    Command = new SqlCommand(Query, Connection)
                    {
                        CommandType = QueryType == QueryTypes.Query ? CommandType.Text : CommandType.StoredProcedure,
                        Notification = null
                    };

                    SqlDependency.Start(ConnectionString);
                    SqlDependency = new SqlDependency(Command, null, 300);

                    Command.Connection.Open();
                    SqlDataReader reader = Command.ExecuteReader(CommandBehavior.CloseConnection);
                    DataTable dt = new DataTable();
                    dt.Load(reader);

                    foreach (Action<DataTable> validator in Validators)
                    {
                        validator.Invoke(dt);
                    }

                    OnChangeMapper.Invoke(dt);

                    if (Command != null)
                        Command.Dispose();

                    if (Connection != null)
                        Connection.Dispose();

                    success = true;
                }
                catch (Exception e)
                {
                    if (OnErrorMapper == null)
                        throw;
                    else
                        OnErrorMapper.Invoke(e);
                }
            }

            SqlDependency.OnChange += SqlDependency_OnChange;
        }

        private void SqlDependency_OnChange(object sender, SqlNotificationEventArgs args)
        {
            ((SqlDependency)sender).OnChange -= SqlDependency_OnChange;

            if (_timer != null)
                _timer.Change(Timeout.Infinite, Timeout.Infinite);

            RegisterForChanges();

            if (_timer != null)
                _timer.Change(timerduration, Timeout.Infinite);
        }

        private void Timer_OnChange(object state)
        {
            if (_timer != null)
                _timer.Change(Timeout.Infinite, Timeout.Infinite);

            SqlDependency.OnChange -= SqlDependency_OnChange;

            RegisterForChanges();

            if (_timer != null)
                _timer.Change(timerduration, Timeout.Infinite);
        }
    }
}