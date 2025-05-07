using SPFramework.Security;
using SPFramework.SSRS.ReportingService;
using System.Collections.Generic;
using System.Linq;

namespace SPFramework.SSRS
{
    public class ParameterInfo: ReportingExecution.ParameterValue
    {

    }

    public static class ReportInfo
    {
        public static List<ParameterInfo> GetParameters(string reportPath)
        {
            List<ParameterInfo> parameters = new List<ParameterInfo>();

            ReportingService2010 rs = new ReportingService2010();

            string passWord = SecurityLoader.GetConnectionString("dbAdmin");
            rs.Credentials = new System.Net.NetworkCredential("dbadmin", passWord, "sarapath");

            rs.Timeout = 10800000;

            string historyID = null;
            bool forRendering = false;
            ParameterValue[] values = null;
            DataSourceCredentials[] credentials = null;
            ItemParameter[] reportParameters;

            reportParameters = rs.GetItemParameters(reportPath, historyID, forRendering, values, credentials);

            if (reportParameters != null)
                parameters = reportParameters.Select(x => new ParameterInfo() { Label = x.Prompt, Name = x.Name }).ToList();

            return parameters;
        }

    }
}
