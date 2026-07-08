using VeridianClimatePulse.Dtos.CommonDto;
using VeridianClimatePulse.Models;

namespace VeridianClimatePulse.Dtos.UserDtos
{
    public class GetUserByRoleRequestDto : PaginationRequest
    {
        public UserRole? GetUserRole { get; set; }
        public int UserID { get; set; }
    }
}
