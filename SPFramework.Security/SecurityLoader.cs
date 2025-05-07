using System;

//using System.Collections.Concurrent;
using System.Diagnostics;
using System.ServiceModel;

namespace SPFramework.Security
{
    [ServiceContract(SessionMode = SessionMode.NotAllowed)]
    public interface ISPSecurity
    {
        [OperationContract]
        string GetConnectionString(string name);
    }

    public static class SecurityLoader
    {
        //static readonly ConcurrentDictionary<string, string> ConnStore = new ConcurrentDictionary<string, string>();
        //static DateTime _datRefreshConnStore;

        public static string GetConnectionString(string name)
        {
            return GetConnStringFromWebService(name);

            // Do we have any connections in the Connection Store?
            /*
                        if (_ConnStore.Count > 0)
                        {
                            if (DateTime.Now < _datRefreshConnStore.AddMinutes(10))
                            {
                                if (_ConnStore.ContainsKey(name))
                                {
                                    // Exists in tne dictionary
                                    string conn = GetConnStringFromDictionary(name);

                                    if (string.IsNullOrWhiteSpace(conn))
                                    {
                                        conn = GetConnStringFromWebService(name);
                                        try
                                        {
                                            AddConnectionToDictionary(name, conn);
                                        }
                                        catch { }
                                    }
                                    return conn;
                                }
                                else
                                {
                                    // Does not exist in the dictionary, so add it.
                                    string conn = GetConnStringFromWebService(name);
                                    try
                                    {
                                        AddConnectionToDictionary(name, conn);
                                    }
                                    catch { }
                                    return conn;
                                }
                            }
                            else
                            {
                                // Time to refresh the Connection Store.
                                _ConnStore = new ConcurrentDictionary<string, string>();
                                string conn = GetConnStringFromWebService(name);
                                try
                                {
                                    AddConnectionToDictionary(name, conn);
                                }
                                catch { }
                                _datRefreshConnStore = DateTime.Now;
                                return conn;
                            }
                        }
                        else
                        {
                            // No Connections have been added to the
                            // Connection Store yet.
                            _ConnStore = new ConcurrentDictionary<string, string>();
                            string conn = GetConnStringFromWebService(name);
                            try
                            {
                                AddConnectionToDictionary(name, conn);
                            }
                            catch { }
                            _datRefreshConnStore = DateTime.Now;
                            return conn;
                        }
            */
        }

        //private static void AddConnectionToDictionary(string name, string conn)
        //{
        //    try
        //    {
        //        ConnStore.AddOrUpdate(name, conn, (key, value) => conn);
        //    }
        //    catch
        //    {
        //        // ignored
        //    }
        //}

        private static WSHttpBinding ConfigBinding()
        {
            WSHttpBinding binding = new WSHttpBinding(SecurityMode.Transport, false)
            {
                Name = "wsHttpBinding",
                MaxBufferPoolSize = int.MaxValue,
                MaxReceivedMessageSize = int.MaxValue,
                ReceiveTimeout = TimeSpan.FromHours(24),
                SendTimeout = TimeSpan.FromHours(24),
                OpenTimeout = TimeSpan.FromHours(24),
                CloseTimeout = TimeSpan.FromHours(24),
                ReaderQuotas =
                {
                    MaxArrayLength = int.MaxValue,
                    MaxBytesPerRead = int.MaxValue,
                    MaxDepth = int.MaxValue,
                    MaxNameTableCharCount = int.MaxValue,
                    MaxStringContentLength = int.MaxValue
                },
                Security =
                {
                    Mode = SecurityMode.Transport,
                    Transport = {ClientCredentialType = HttpClientCredentialType.None}
                }
            };
            return binding;
        }

        private static string GetConnStringFromWebService(string name)
        {
            EndpointAddress securityEndpoint;
            ChannelFactory<ISPSecurity> dcf = null;
            ISPSecurity securityclient;

            string sDbConStr;

            try
            {
                // Connect to the primary SPSecurityWCF
                securityEndpoint = new EndpointAddress("https://spiis.sarapath.com:1516/SPSecurityWCF.svc");
                dcf = new ChannelFactory<ISPSecurity>(ConfigBinding(), securityEndpoint);
                securityclient = dcf.CreateChannel();

                sDbConStr = securityclient.GetConnectionString(name);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("SPFramework", ex.Message);
                dcf?.Abort();

                try
                {
                    // Connect to the secondary SPSecurityWCF
                    securityEndpoint = new EndpointAddress("https://SPWAREHOUSE.sarapath.com:1516/SPSecurityWCF.svc");
                    dcf = new ChannelFactory<ISPSecurity>(ConfigBinding(), securityEndpoint);
                    securityclient = dcf.CreateChannel();

                    sDbConStr = securityclient.GetConnectionString(name);
                }
                catch (Exception ex1)
                {
                    EventLog.WriteEntry("SPFramework", ex1.Message);

                    dcf?.Abort();
                    sDbConStr = "";
                }
            }

            return sDbConStr;
        }

        //private static string GetConnStringFromDictionary(string name)
        //{
        //    try
        //    {
        //        string conn;
        //        if (ConnStore.TryGetValue(name, out conn))
        //        {
        //            return conn;
        //        }
        //        else
        //        {
        //            return string.Empty;
        //        }
        //    }
        //    catch
        //    {
        //        // ignored
        //    }
        //    return string.Empty;
        //}
    }
}