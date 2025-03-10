using System;
using System.Collections.Generic;

namespace PathDistribution.Models
{
    public class VacationSchedules
    {
        public Tuple<DateTime, DateTime> Dates { get; set; }

        public List<string> Paths { get; set; }

        public List<Tuple<string,string>> Assignments { get; set; }

        public List<PathScheduleData> PathScheduleData { get; set; }

        public List<PathScheduleDates> PathScheduleDates { get; set; }

        public List<PTORequest> PTORequests { get; set; }
    }
    public class PathScheduleData
    {
        public string chrPath { get; set; }
        public int DaysOff { get; set; }
        public int Balance { get; set; }
    }
    public class PathScheduleDates
    {
        public DateTime dte { get; set; }
        public int WeekNum { get; set; }
        public string WkDay { get; set; }
        public bool isHoliday { get; set; }
        public string chrPath { get; set; }
        public string chrValue { get; set; }
        public string chrColor { get; set; }
        public List<Tuple<string, string, string>> OffAssignments { get; set; }
    }

    public class PTORequest
    {
        public int pkPTO { get; set; }
        public string chrPath { get; set; }
        public DateTime dteStart { get; set; }
        public DateTime dteEnd { get; set; }
        public int intStatus { get; set; }
        public string chrAbbr { get; set; }
        public string chrStatusChangedBy { get; set; }
        public DateTime dteStatusChanged { get; set; }
        public string chrComments { get; set; }
        public DateTime dteCreated { get; set; }
        public DateTime dteModified { get; set; }
    }

    public class Holidays
    {
        internal static DateTime AdjustFixedHoliday(int year, int month, int day)
        {
            var d = new DateTime(year, month, day);

            switch (d.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    d = d.AddDays(1);
                    break;
                case DayOfWeek.Saturday:
                    d = d.AddDays(-1);
                    break;
            }
            return d;
        }

        internal static DateTime GetMemorialDay(int year)
        {
            // Set first of last month
            DateTime d = new DateTime(year, 6, 1);

            // Roll the days backwards until Monday.
            if (d.DayOfWeek != DayOfWeek.Monday)
            {
                while (d.DayOfWeek != DayOfWeek.Monday)
                {
                    d = d.AddDays(-1);
                }
            }
            return d;
        }

        internal static DateTime GetLaborDay(int year)
        {
            // Set first of last month
            DateTime d = new DateTime(year, 9, 1);

            // Roll the days forwards until Monday.
            if (d.DayOfWeek != DayOfWeek.Monday)
            {
                while (d.DayOfWeek != DayOfWeek.Monday)
                {
                    d = d.AddDays(1);
                }
            }
            return d;
        }

        internal static DateTime GetThanksgiving(int year)
        {
            //Get Last day of the month
            DateTime d = new DateTime(year, 12, 1).AddDays(-1);
            // Move back a week if needed
            if ((int)d.DayOfWeek < 4)
            {
                d = d.AddDays(-7);
            }
            // Roll the days backwards until Thursday.
            if (d.DayOfWeek != DayOfWeek.Thursday)
            {
                while (d.DayOfWeek != DayOfWeek.Thursday)
                {
                    d = d.AddDays(-1);
                }
            }
            return d;
        }

        public static bool IsHoliday(DateTime dateTime)
        {
            return AdjustFixedHoliday(dateTime.Year, 1, 1).Equals(dateTime)
                   || GetMemorialDay(dateTime.Year).Equals(dateTime)
                   || AdjustFixedHoliday(dateTime.Year, 7, 4).Equals(dateTime)
                   || GetLaborDay(dateTime.Year).Equals(dateTime)
                   || GetThanksgiving(dateTime.Year).Equals(dateTime)
                   || AdjustFixedHoliday(dateTime.Year, 12, 25).Equals(dateTime);
        }
    }
}