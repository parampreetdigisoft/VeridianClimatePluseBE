using VeridianClimatePulse.Dtos.ProgramDto;

namespace VeridianClimatePulse.Dtos.UserDtos
{
    public class GetUserByRoleResponse : PublicUserResponse
    {
        public List<AddUpdateProgramDto> ClimatePrograms { get; set; } = new();
    }
}
