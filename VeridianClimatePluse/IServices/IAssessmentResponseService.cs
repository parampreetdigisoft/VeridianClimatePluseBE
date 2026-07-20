using VeridianClimatePulse.Common.Models;
using VeridianClimatePulse.Dtos.AssessmentDto;
using VeridianClimatePulse.Dtos.CommonDto;
using VeridianClimatePulse.Dtos.dashboard;
using VeridianClimatePulse.Models;

namespace VeridianClimatePulse.IServices
{
    public interface IAssessmentResponseService
    {
        Task<List<AssessmentResponse>> GetAllAsync();
        Task<AssessmentResponse> GetByIdAsync(int id);
        Task<AssessmentResponse> AddAsync(AssessmentResponse response);
        Task<AssessmentResponse> UpdateAsync(int id, AssessmentResponse response);
        Task<bool> DeleteAsync(int id);
        Task<ResultResponseDto<string>> SaveAssessment(AddAssessmentDto request);
        Task<PaginationResponse<GetProgramAssessmentResponseDto>> GetAssessmentResult(GetAssessmentRequestDto request, UserRole role);
        Task<PaginationResponse<GetAssessmentQuestionResponseDto>> GetAssessmentQuestion(GetAssessmentQuestoinRequestDto request);
        Task<ResultResponseDto<string>> ImportAssessmentAsync(IFormFile file,int userID);
        Task<GetProgramQuestionHistoryResponseDto> GetProgramQuestionHistory(UserProgramRequestDto userProgramRequestDto);
        Task<ResultResponseDto<GetAssessmentHistoryDto>> GetAssessmentProgressHistory(int assessmentID);
        Task<ResultResponseDto<string>> ChangeAssessmentStatus(ChangeAssessmentStatusRequestDto r);
        Task<ResultResponseDto<string>> TransferAssessment(TransferAssessmentRequestDto r, int userID, UserRole userRole);
        Task<ResultResponseDto<AiProgramPillarDashboardResponseDto>> GetProgramPillarHistory(UserProgramDashBoardRequestDto userProgramRequestDto,int userID, UserRole userRole);
    }
} 