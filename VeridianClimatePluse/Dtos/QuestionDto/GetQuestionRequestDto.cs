using VeridianClimatePulse.Dtos.CommonDto;

namespace VeridianClimatePulse.Dtos.QuestionDto
{
    public class GetQuestionRequestDto : PaginationRequest
    {
        public int? PillarID { get; set; }
    }
}
