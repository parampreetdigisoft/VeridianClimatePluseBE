using VeridianClimatePulse.Dtos.CommonDto;
using VeridianClimatePulse.Models;

namespace VeridianClimatePulse.Dtos.kpiDto
{
    public class GetAnalyticalLayerRequestDto : PaginationRequest
    {
        public int? ClimateProgramID { get; set; }
        public int? LayerID { get; set; }
        public int Year { get; set; } = DateTime.Now.Year;
    }
}
