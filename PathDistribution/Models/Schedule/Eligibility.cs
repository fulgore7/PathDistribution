using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;


namespace PathDistribution.Models
{
    public enum EligibilityTypes
    {
        [Display(Name = "Not Eligible")]
        NotEligible = 0,
        Eligible = 1,
        Preferred = 2
    }
    
    public class EligibilityData
    {
        public List<Eligibility> Eligibilities { get; set; }

        public List<Pathologists> Paths { get; set; }

        public List<Tuple<string, string>> Assignments { get; set; }
    }

    public class Eligibility
    {
        public int pkEligibility { get; set; }
        public string chrPath { get; set; }
        public string chrName { get; set; }
        public bool bitActive { get; set; }
        public string chrAbbr { get; set; }
        public string chrAssignment { get; set; }
        public EligibilityTypes intEligibility { get; set; }
        public int intPerformed { get; set; }
        public int intCredited { get; set; }
    }
}