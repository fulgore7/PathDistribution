using System.ComponentModel.DataAnnotations;

namespace PathDistribution.Models
{
    public class TypeBreakdown
    {
        [Display(Name ="Path")]
        public string chrPath { get; set; }
        public int SK { get; set; }
        public int GI { get; set; }
        public int GU { get; set; }
        public int BRST { get; set; }
        public int FNA { get; set; }
        public int NCB { get; set; }
        public int GP { get; set; }
        public int PBX { get; set; }
        public int BX { get; set; }
        public int BB { get; set; }
        public int WS { get; set; }
        public int CYTO { get; set; }
        public int TOTAL { get; set; }
    }
}