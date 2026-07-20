using VeridianClimatePulse.Common.Models;
using VeridianClimatePulse.Dtos.AssessmentDto;
using VeridianClimatePulse.Dtos.CommonDto;
using VeridianClimatePulse.Dtos.UserDtos;
using VeridianClimatePulse.Models;

namespace VeridianClimatePulse.IServices
{
    public interface IUserService
    {
        User GetByEmail(string email);
        Task<PaginationResponse<GetUserByRoleResponse>> GetUserByRoleWithAssignedProgram(GetUserByRoleRequestDto requestDto, int userid, UserRole userRole);
        Task<ResultResponseDto<List<PublicUserResponse>>> GetEvaluatorByAnalyst(GetAssignUserDto requestDto);
        Task<ResultResponseDto<List<GetAssessmentResponseDto>>> GetUsersAssignedToProgram(int climateProgramID);
        Task<ResultResponseDto<UpdateUserResponseDto>> GetUserInfo(int userId);

    }
} 