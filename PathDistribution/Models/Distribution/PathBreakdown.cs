using System.Collections.Generic;

namespace PathDistribution.Models
{
    public class PathBreakdown
    {
        public List<PathDistribution> PathDistribution { get; set; }

        public List<CaseDetails> CaseDetails { get; set; }

        public bool IsOff { get; set; }
        public bool Padding { get; set; }
        public showMessages showMessage { get; set; }
    }

    public enum showMessages
    {
        Nothing = 0,
        Consult = 1,
        Off = 2
    }
}