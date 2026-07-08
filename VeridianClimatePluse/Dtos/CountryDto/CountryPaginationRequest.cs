using VeridianClimatePulse.Dtos.CommonDto;

namespace VeridianClimatePulse.Dtos.CountryDto
{
    public class CountryPaginationRequest: PaginationRequest
    {
        public int? CountryID { get; set; }
    }
}
