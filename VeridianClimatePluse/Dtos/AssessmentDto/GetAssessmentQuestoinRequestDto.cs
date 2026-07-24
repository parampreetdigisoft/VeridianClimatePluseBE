using VeridianClimatePulse.Dtos.CommonDto;

namespace VeridianClimatePulse.Dtos.AssessmentDto
{
    public class GetAssessmentQuestionRequestDto : PaginationRequest
    {
        public int AssessmentID { get; set; } 
        public int? PillarID { get; set; }
    }
}
