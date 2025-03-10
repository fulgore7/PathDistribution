using System;
using System.Collections.Generic;

namespace PathDistribution.Models
{
    public class CaseMove
    {
        public DateTime dteDateAssigned { get; set; }
        public string chrCaseNumber { get; set; }
        public string chrPath { get; set; }
        public int intSlideCount { get; set; }
        public string chrComment { get; set; }
    }

    public class CaseMoves: List<CaseMove>
    {
        public List<string> Paths { get; set; }

        public MinMax MinAndMax { get; set; }
    }
}