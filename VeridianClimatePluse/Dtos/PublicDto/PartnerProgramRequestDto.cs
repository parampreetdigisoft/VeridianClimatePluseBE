using VeridianClimatePulse.Dtos.CommonDto;

namespace VeridianClimatePulse.Dtos.PublicDto
{
    public class PartnerProgramRequestDto : PaginationRequest
    {
        public string? Program { get; set; }
        public int? ClimateProgramID { get; set; }
        public string? Location { get; set; }
        public int? PillarID { get; set; }
    }
    
}
