using System.Collections.Generic;

namespace PathDistribution.Models
{
    public class PathData
    {
        public PathData()
        {
            PathAssign = new PathAssignments();
            OffPathAssign = new PathAssignments();
            PathDistribution = new List<PathDistribution>();
            CaseDetails = new List<CaseDetails>();
            cases = new CaseMoves();
        }

        public PathAssignments PathAssign { get; set; }
        
        public PathAssignments OffPathAssign { get; set; }

        public List<PathDistribution> PathDistribution { get; set; }

        public List<CaseDetails> CaseDetails { get; set; }
        public List<TypeBreakdown> TypeBreakdowns { get; set; }

        public CaseMoves cases { get; set; }
        
        public bool DownloadPDF { get; set; }

        public StartSettings Settings { get; set; }

        public string InvalidSchedule { get; set; }

        public List<AddedSlide> AddedSlides { get; set; }
        
    }
}