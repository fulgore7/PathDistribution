namespace PathDistribution.Models
{
    public class Pathologists
    {
        public string pkPath { get; set; }
        public string chrCopathAbbr { get; set; }
        public string chrScheduleAbbr { get; set; }
        public string chrName { get; set; }
        public bool bitHeme { get; set; }
        public bool bitCyto { get; set; }
        public bool bitSkin { get; set; }
        public bool bitGeneral { get; set; }
        public bool bitActive { get; set; }
        public int intGI { get; set; }
        public bool bitGU { get; set; }
        public int intOrder { get; set; }
        public bool bitIsConsultant { get; set; }
        public bool bitIsScheduled { get; set; }
    }
}