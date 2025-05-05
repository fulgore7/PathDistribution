using System.Collections.Generic;
using System.IO;

namespace SPFramework.SSRS
{
    public class ReportBuilder<T> where T : ReportGenerator
    {
        protected internal ReportGenerator Query;

        public ReportBuilder(ReportGenerator rdl)
        {
            Query = rdl;
        }

        public string FullPath { get { return Query.FullPath; } }

        public ReportBuilder<T> AddParameter(string parameterName, string value)
        {
            Query.AddParameter(parameterName, value);
            return this;
        }

        public ReportBuilder<T> AddParameterWithDefault(string parameterName)
        {
            Query.AddParameterWithDefault(parameterName);
            return this;
        }

        public Stream Render(ReportFormats reportFormat)
        {
            return Query.Render(reportFormat);
        }

        public List<ParameterInfo> GetParameters()
        {
            return ReportInfo.GetParameters(Query.FullPath);
        }
    }
}
