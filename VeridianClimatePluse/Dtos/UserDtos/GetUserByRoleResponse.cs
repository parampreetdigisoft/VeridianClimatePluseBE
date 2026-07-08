using VeridianClimatePulse.Dtos.CountryDto;

namespace VeridianClimatePulse.Dtos.UserDtos
{
    public class GetUserByRoleResponse : PublicUserResponse
    {
        public List<AddUpdateCountryDto> Countries { get; set; } = new();
    }
}
