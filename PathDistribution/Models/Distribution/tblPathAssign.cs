using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PathDistribution.Models
{
    public class tblPathAssign
    {
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime dteAssigned { get; set; }
        
        public string chrName { get; set; }

        public string chrAssignment { get; set; }
        
        public string chrOverName { get; set; }

        public string chrOverComment { get; set; }

        public bool bitOverride { get; set; }

        public bool bitDelete { get; set; }
    }

    public class PathAssignments: List<tblPathAssign>
    {
        public List<string> Paths { get; set; }

        public List<string> OffAssignments { get; set; }
    }
}