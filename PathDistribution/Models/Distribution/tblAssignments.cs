namespace PathDistribution.Models
{
    public class tblAssignments
    {
        public int pkAssignment { get; set; }

        public string chrAbbr { get; set; }

        public string chrAssignment { get; set; }

        public int intMaxSlideCount { get; set; }

        public int intSlideException { get; set; }

        public bool bitIsOffAssignment { get; set; }
    }
}