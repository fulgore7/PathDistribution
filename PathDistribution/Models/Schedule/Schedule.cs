using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace PathDistribution.Models
{
    public class ScheduleData
    {
        public List<Schedule> Schedules { get; set; }

        public List<OffSchedule> OffSchedule { get; set; }
        public List<string> Assignments { get; set; }
        public List<ScheduleComments> Comments { get; set; }
        public List<SelectListItem> WeekEnd { get; set; }

        public List<Tuple<DateTime, string>> WeekDays 
        {
            get
            {
                // Get the en-US culture.
                CultureInfo ci = new CultureInfo("en-US");
                // Get the DateTimeFormatInfo for the en-US culture.
                DateTimeFormatInfo dtfi = ci.DateTimeFormat;
                return new List<Tuple<DateTime, string>>(Schedules.GroupBy(x => x.dteScheduleDate)
                                                                  .Select(y => new Tuple<DateTime, string>(y.Key,
                                                                                                           dtfi.GetShortestDayName(y.Key.DayOfWeek))));
            }
        }
        public List<ScheduleIssues> Issues { get; set; }

        public List<Tuple<string, string>> OffTypes { get; set; }
    }
    public class Schedule
    {
        public DateTime dteScheduleWeek { get; set; }
        public DateTime dteScheduleDate { get; set; }
        public string chrAbbr { get; set; }
        public string chrPath { get; set; }

        public string chrPaths { get; set; }

        public List<SelectListItem> chrPathList 
        { 
            get
            {
                return (chrPaths??string.Empty).Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                               .Select(x => new SelectListItem() { Value = x, Text = x, Selected = x == chrPath })
                               .ToList();
            }
        }
    }

    public class OffSchedule
    {
        public string chrPath { get; set; }
        public string chrName { get; set; }
        
        public string Mo { get; set; }
        public string Tu { get; set; }
        public string We { get; set; }
        public string Th { get; set; }
        public string Fr { get; set; }
    }

    public class PathSchedWeek
    {
        public string chrPath { get; set; }
        public int AssnSum { get; set; }
    }

    public class PathOffScheduleWeek
    {
        public string cActLabel { get; set; }
    }

    public class ScheduleComments
    {
        public int? pkComment { get; set; }
        public DateTime dteWeek { get; set; }
        public string chrComment { get; set; }
    }

    public class SchedulePath
    {
        public string chrAbbr { get; set; }
        public DateTime dteAssigned { get; set; }
        public string chrPath { get; set; }
    }

    public class ScheduleIssues
    {
        public int intOrder { get; set; }
        public string Issues { get; set; }
    }
}