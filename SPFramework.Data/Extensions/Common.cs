using SPFramework.Data.Exceptions;
using SPFramework.Security;
using System;
using System.Configuration;

namespace SPFramework.Data.Extensions
{
    public static class Common
    {
        public static string SetConnectionString(string connectName, ConnectionType connectType)
        {
            string connectString = string.Empty;

            try
            {
                switch (connectType)
                {
                    case ConnectionType.UseDirect:
                        connectString = connectName;
                        break;

                    case ConnectionType.UseConfiguration:
                        if (ConfigurationManager.ConnectionStrings.Count == 0)
                            throw new Exception("No connection strings found in config file");

                        if (string.IsNullOrWhiteSpace(ConfigurationManager.ConnectionStrings[connectName].ConnectionString))
                            throw new Exception(connectName + " was not found in config file");

                        connectString = ConfigurationManager.ConnectionStrings[connectName].ConnectionString;
                        break;

                    case ConnectionType.UseSecurityLoader:
                        connectString = SecurityLoader.GetConnectionString(connectName);
                        if (string.IsNullOrWhiteSpace(connectString))
                            throw new Exception(connectName + " connection string was not retrieved from Security Loader.");
                        break;
                }
            }
            catch (Exception e)
            {
                throw new SPSqlException("An error occured setting up the connection with supplied connection string: ", e);
            }

            return connectString;
        }
    }
}