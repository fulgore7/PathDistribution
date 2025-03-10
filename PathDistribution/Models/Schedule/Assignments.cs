using System.ComponentModel.DataAnnotations;

namespace PathDistribution.Models
{
    public enum AssignmentType
    {
        Daily = 0,
        Weekly = 1,
        [Display(Name = "Partial Off")]
        PartialOff = 2,
        [Display(Name = "Full Off")]
        FullOff = 3
    }

    public class Assignments
    {
        public int? pkAssignment { get; set; }
        [MaxLength(10)]
        public string chrAbbr { get; set; }
        [MaxLength(100)]
        public string chrAssignment { get; set; }
        public int intMaxSlideCount { get; set; }
        public AssignmentType intAssignmentType { get; set; }
        public bool bitActive { get; set; }
        public bool bitIsScheduled { get; set; }
    }
}