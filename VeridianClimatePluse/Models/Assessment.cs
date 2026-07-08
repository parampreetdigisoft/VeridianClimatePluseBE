namespace VeridianClimatePulse.Models
{
    public class Assessment
    {
        public int AssessmentID { get; set; }

        /// <summary>
        /// Legacy: For backward compatibility with country-based assessments
        /// </summary>
        public int? UserCountryMappingID { get; set; }

        /// <summary>
        /// New: For climate program-based assessments
        /// </summary>
        public int? UserClimateProgramMappingID { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
        public AssessmentPhase? AssessmentPhase { get; set; } = Models.AssessmentPhase.InProgress;

        // Navigation properties
        public UserCountryMapping? UserCountryMapping { get; set; }
        public UserClimateProgramMapping? UserClimateProgramMapping { get; set; }
        public ICollection<PillarAssessment> PillarAssessments { get; set; } = new List<PillarAssessment>();
    }

    public enum AssessmentPhase : byte
    {
        NotStarted = 0,   // Assessment not submitted at all
        InProgress = 1,   // User has access to edit
        EditRequested = 2, // User requested permission to edit
        EditRejected = 3, // Admin/analyst rejected edit request
        EditApproved = 4, // Admin/analyst approved edit request
        Completed = 5     // Assessment completed
    }
}
