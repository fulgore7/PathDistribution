using Microsoft.Ajax.Utilities;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using PathDistribution.Helpers;
using PathDistribution.Models.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace PathDistribution.Models
{
    public enum ReportTypes
    {
        AssignmentTotals = 0,
        WeekendCallSched = 1,
        WorkSched = 2,
        CallSched = 3,
        PathSched = 4
    }

    public enum ReportParameterTypes
    {
        WeekDateTime,
        DateTime,
        Year,
        Pathologist
    }

    public class Report
    {
        public IPrincipal ReportUser { get; set; }

        public ReportTypes ReportType { get; set; }

        public string ReportName
        {
            get
            {
                switch (ReportType)
                {
                    case ReportTypes.AssignmentTotals:
                        return "assignmenttotals";
                    case ReportTypes.WeekendCallSched:
                        return "weekendcallsched";
                    case ReportTypes.WorkSched:
                        return "PathSched";
                    case ReportTypes.CallSched:
                        return "CallSched";
                    case ReportTypes.PathSched:
                        return "PathSched";
                    default:
                        return string.Empty;
                }
            }
        }

        public string ReportPath
        {
            get
            {
                switch (ReportType)
                {
                    case ReportTypes.AssignmentTotals:
                        return @"/Histology/Distribution/Application Reports/Assignment Totals";
                    case ReportTypes.WeekendCallSched:
                        return @"/Histology/Distribution/Application Reports/Weekend Call Schedule";
                    case ReportTypes.WorkSched:
                        return @"/Histology/Distribution/Application Reports/Work Schedule";
                    case ReportTypes.CallSched:
                        return @"/Histology/Distribution/Application Reports/Call Schedule";
                    case ReportTypes.PathSched:
                        return @"/Histology/Distribution/Application Reports/Pathologist Schedule";
                    default:
                        return string.Empty;
                }
            }
        }

        public List<RptParameter> ReportParameters
        {
            get
            {
                switch (ReportType)
                {
                    case ReportTypes.AssignmentTotals:
                        return new List<RptParameter>();
                    case ReportTypes.WeekendCallSched:
                        return new List<RptParameter>()
                        {
                            new RptParameter() { Name = "year", Prompt = "Year", MultiValue = false, Type = ReportParameterTypes.Year, DataType = SqlDbType.Int, MaxLength = 0, Default = DateTime.Now.Year.ToString() }
                        };
                    case ReportTypes.WorkSched:
                        return new List<RptParameter>()
                        {
                            new RptParameter() { Name = "StartDate", Prompt = "Beginning Date", MultiValue = false, Type = ReportParameterTypes.WeekDateTime, DataType = SqlDbType.DateTime, MaxLength = 0, Default = DateTime.Today.AddDays(-1 * ((int)DateTime.Today.DayOfWeek)).AddDays(1).ToShortDateString() },
                            new RptParameter() { Name = "EndDate", Prompt = "Ending Date", MultiValue = false, Type = ReportParameterTypes.WeekDateTime, DataType = SqlDbType.DateTime, MaxLength = 0, Default = DateTime.Today.AddDays(-1 * ((int)DateTime.Today.DayOfWeek)).AddDays(32).ToShortDateString() }
                        };
                    case ReportTypes.CallSched:
                        return new List<RptParameter>()
                        {
                            new RptParameter() { Name = "StartDate", Prompt = "Beginning Date", MultiValue = false, Type = ReportParameterTypes.WeekDateTime, DataType = SqlDbType.DateTime, MaxLength = 0, Default = DateTime.Today.AddDays(-1 * ((int)DateTime.Today.DayOfWeek)).AddDays(1).ToShortDateString() },
                            new RptParameter() { Name = "EndDate", Prompt = "Ending Date", MultiValue = false, Type = ReportParameterTypes.WeekDateTime, DataType = SqlDbType.DateTime, MaxLength = 0, Default = DateTime.Today.AddDays(-1 * ((int)DateTime.Today.DayOfWeek)).AddDays(32).ToShortDateString() }
                        };
                    case ReportTypes.PathSched:
                        return new List<RptParameter>()
                        {
                            new RptParameter() { Name = "StartDate", Prompt = "Beginning Date", MultiValue = false, Type = ReportParameterTypes.WeekDateTime, DataType = SqlDbType.DateTime, MaxLength = 0, Default = DateTime.Today.AddDays(-1 * ((int)DateTime.Today.DayOfWeek)).AddDays(1).ToShortDateString() },
                            new RptParameter() { Name = "EndDate", Prompt = "Ending Date", MultiValue = false, Type = ReportParameterTypes.WeekDateTime, DataType = SqlDbType.DateTime, MaxLength = 0, Default = DateTime.Today.AddDays(-1 * ((int)DateTime.Today.DayOfWeek)).AddDays(32).ToShortDateString() },
                            new RptParameter() { Name = "PathList", Prompt = "Paths", MultiValue = true, Type = ReportParameterTypes.Pathologist, DataType = SqlDbType.VarChar, MaxLength = Int32.MaxValue, Default = string.Join(",", GetPaths(DateTime.Today.AddDays(-1 * ((int)DateTime.Today.DayOfWeek)).AddDays(1),DateTime.Today.AddDays(-1 * ((int)DateTime.Today.DayOfWeek)).AddDays(32)).Select(x => x.chrCopathAbbr).ToList()) }
                        };
                    default:
                        return new List<RptParameter>();
                }
            }
        }

        public bool PostSharepoint
        {
            get
            {
                return ReportType == (ReportTypes.WorkSched | ReportTypes.CallSched) && HttpContext.Current.IsAdmin();
            }
        }

        public bool NotifyCouriers
        {
            get
            {
                return ReportType == ReportTypes.WorkSched && HttpContext.Current.IsAdmin();
            }
        }

        public List<Pathologists> Paths { get; set; }

        public List<Pathologists> GetPaths(DateTime start, DateTime end)
        {
            AdminDAL dal = new AdminDAL();

            if (ReportType == ReportTypes.PathSched)
                Paths = dal.GetPathSchedPaths(start, end).Where(x => HttpContext.Current.IsAdmin() && ReportUser.Identity.Name.Equals(x.chrCopathAbbr)).ToList();
            else
                Paths = new List<Pathologists>();

            return Paths;
        }

        private int GetWeekNumber()
        {
            CultureInfo ciCurr = CultureInfo.CurrentCulture;
            int weekNum = ciCurr.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            return weekNum;
        }
    }

    public class RptParameter
    {
        public string Name { get; set; }
        public ReportParameterTypes Type { get; set; }
        public bool MultiValue { get; set; }
        public string Prompt { get; set; }
        public SqlDbType DataType { get; set; }
        public int MaxLength { get; set; }
        public string Default { get; set; }

    }

}