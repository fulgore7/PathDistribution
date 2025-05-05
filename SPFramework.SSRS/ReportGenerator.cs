using SPFramework.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace SPFramework.SSRS
{
    public enum ReportFormats
    {
        Csv,
        Image,
        Pdf,
        Excel,
        Word
    }

    public class ReportGenerator
    {
        protected string ReportPath { get; }
        protected List<ReportingExecution.ParameterValue> Parameters { get; }

        public string FullPath { get; set; }

        public ReportGenerator(string reportPath)
        {
            ReportPath = reportPath;
            FullPath = ReportPath;
            Parameters = new List<ReportingExecution.ParameterValue>();
        }

        public void AddParameter(string parameterName, string value)
        {
            Parameters.Add(new ReportingExecution.ParameterValue() { Name = parameterName, Value = value });
        }

        public void AddParameterWithDefault(string parameterName)
        {
            Parameters.Add(new ReportingExecution.ParameterValue() { Name = parameterName });
        }

        public Stream Render(ReportFormats reportFormat)
        {
            MemoryStream stream = new MemoryStream();
            string encoding;
            string mimeType;
            string extension;

            //ReportingService.ReportingService2010 rs = new ReportingService.ReportingService2010();
            ReportingExecution.ReportExecutionService rsExec = new ReportingExecution.ReportExecutionService();
            
            string passWord = SecurityLoader.GetConnectionString("dbAdmin");
            //rs.Credentials = new NetworkCredential("dbadmin", passWord, "sarapath");
            rsExec.Credentials = new NetworkCredential("dbadmin", passWord, "sarapath");

            //rs.Timeout = 10800000;
            rsExec.Timeout = 10800000;

            ReportingExecution.ExecutionInfo execInfo = rsExec.LoadReport(ReportPath, null);
            
            FullPath = string.Format(@"{0}?{1}&rs%3AParameterLanguage=en-US",
                                     rsExec.Url,
                                     WebUtility.UrlEncode(ReportPath));

            if (Parameters.Count > 0)
            {
                List<ReportingExecution.ParameterValue> parameters = new List<ReportingExecution.ParameterValue>();

                foreach (var item in Parameters)
                {
                    if (string.IsNullOrEmpty(item.Value))
                    {
                        foreach (var item2 in execInfo.Parameters)
                        {
                            if (item.Name == item2.Name)
                            {
                                bool firstrun = true;

                                if (item2.DefaultValues == null)
                                {
                                    parameters.Add(new ReportingExecution.ParameterValue() { Name = item.Name, Value = null });
                                }
                                else
                                {
                                    foreach (var item3 in item2.DefaultValues)
                                    {
                                        if (firstrun)
                                        {
                                            item.Value = item3;
                                            firstrun = false;
                                        }
                                        else
                                        {
                                            parameters.Add(new ReportingExecution.ParameterValue() { Name = item.Name, Value = item3 });
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                Parameters.AddRange(parameters);

                rsExec.SetExecutionParameters(Parameters.ToArray(), "en-us");
            }

            ReportingExecution.Warning[] warnings;
            string[] streamIDs;
            var results = rsExec.Render(Enum.GetName(typeof(ReportFormats), reportFormat), null, out extension, out encoding, out mimeType, out warnings, out streamIDs);

            stream.Write(results, 0, results.Length);
            stream.Position = 0;

            return stream;
        }
    }
}
