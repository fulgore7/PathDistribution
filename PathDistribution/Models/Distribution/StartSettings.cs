using System;

namespace PathDistribution.Models
{
    public class StartSettings
    {
        public StartSettings()
        {
            StartAccession = DateTime.MinValue;
            EndAccession = DateTime.MinValue;
            ResetDist = DateTime.MinValue.AddDays(1);
            Priority = Priorities.Weekday;
        }
        public DateTime StartAccession { get; set; }
        public DateTime EndAccession { get; set; }
        public DateTime Distribution { get { return EndAccession.AddDays(1); } }
        public Priorities Priority { get; set; }

        public DateTime ResetDist { get; set; }
    }

    public enum Priorities
    {
        Weekday = 1,
        Saturday = 2
    }

    public enum ProcessTypes
    {
        NotProcessed = 0,
        Processed = 1,
        Generated = 2,
        BadStartDate = 3,
        BadEndDate = 4
    }
}