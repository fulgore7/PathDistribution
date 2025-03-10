namespace PathDistribution.Models
{
    public class PathDistribution
    {
        public string chrPath { get; set; }
        public int PreAssignedSLIDES { get; set; }
        public int ComplexSLIDES { get; set; }
        public int SPDSLIDES { get; set; }
        public int PCDSLIDES { get; set; }
        public int PPD_SlidesSLIDES { get; set; }
        public int ClientSLIDES { get; set; }
        public int ClinicianSLIDES { get; set; }
        public int HEMESLIDES { get; set; }
        public int SKINSLIDES { get; set; }
        public int GENERALSLIDES { get; set; }
        public int SDSLIDES { get; set; }
        public int TOTALSLIDES { get; set; }
        public bool IsOff { get; set; }
        public bool IsConsultant { get; set; }
        public int AddedSlides { get; set; }
        public string Limits { get; set; }
    }
}