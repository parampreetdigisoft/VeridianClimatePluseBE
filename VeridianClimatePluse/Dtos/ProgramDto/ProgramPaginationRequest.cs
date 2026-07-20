using VeridianClimatePulse.Dtos.CommonDto;

namespace VeridianClimatePulse.Dtos.ProgramDto
{
    public class ProgramPaginationRequest: PaginationRequest
    {
        public int? ClimateProgramID { get; set; }
    }
}
