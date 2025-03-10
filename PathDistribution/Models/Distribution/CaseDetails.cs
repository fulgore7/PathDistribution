using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PathDistribution.Models
{
    public class CaseDetails
    {
        public string chrPath { get; set; }
        public int RowID { get; set; }
        public string specnum_formatted { get; set; }
        public DateTime accession_date { get; set; }
        public string parttype { get; set; }
        public string ordererclient { get; set; }
        public string stainabbr { get; set; }
        public string block { get; set; }
        public string catabbr { get; set; }
        public string blocklabel { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime stainorderdate { get; set; }
        public int slidecount { get; set; }
        public int slidecountactual { get; set; }
        public string PriorityColor { get; set; }
        public string PriorityWeight { get; set; }
        public bool IsFrozen { get; set; }
        public string AssignmentType { get; set; }
        public string SecondaryAssignmentType { get; set; }

        public bool IsLesserAssigned { get; set; }

        public List<string> Paths { get; set; }
        public string pathList { get; set; }
        public string BGColor { get; set; }
        public bool IsPlastic { get; set; }
        public string PartGroup { get; set; }

        public bool SatMon { get; set; }
    }
    
}