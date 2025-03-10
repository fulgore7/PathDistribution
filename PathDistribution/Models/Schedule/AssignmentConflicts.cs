using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace PathDistribution.Models
{
    public enum AssignmentConflictTypes
    {
        [Display(Name = "None")]
        NoConflict = 0,
        [Display(Name = "Partial")]
        PartialConflict = 1,
        [Display(Name = "Full")]
        FullConflict = 2
    }

    public class AssignmentConflicts
    {
        public List<PrimaryConflict> PrimaryConflicts
        {
            get
            {
                return Conflicts.Select(x => new PrimaryConflict() 
                                                { 
                                                    chrPrimaryAbbr = x.chrPrimaryAbbr, 
                                                    chrPrimaryAssignment = x.chrPrimaryAssignment 
                                                })
                                .GroupBy(y => new 
                                                { 
                                                    y.chrPrimaryAbbr, 
                                                    y.chrPrimaryAssignment 
                                                })
                                .Select(a => a.First())
                                .ToList();
            }
        }
        public List<AssignmentConflict> Conflicts { get; set; }
    }

    public class PrimaryConflict
    {
        public string chrPrimaryAbbr { get; set; }
        public string chrPrimaryAssignment { get; set; }
    }

    public class AssignmentConflict
    {
        public int? pkAssignmentConflict { get; set; }
        public string chrPrimaryAbbr { get; set; }
        public string chrPrimaryAssignment { get; set; }
        public string chrRelatedAbbr { get; set; }
        public string chrRelatedAssignment { get; set; }
        public AssignmentConflictTypes intConflictType { get; set; }
    }

    public class MoveAssignmentConflict
    {
        public int pkAssignmentConflict { get; set; }
        public int intOldConflictType { get; set; }
        public int intNewConflictType { get; set; }
    }
}