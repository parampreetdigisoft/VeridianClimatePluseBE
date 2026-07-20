using VeridianClimatePulse.Common.Models;
using VeridianClimatePulse.Dtos.AssessmentDto;
using VeridianClimatePulse.Dtos.CommonDto;
using VeridianClimatePulse.Dtos.ProgramDto;
using VeridianClimatePulse.Models;

namespace VeridianClimatePulse.IServices
{
    public interface IProgramService
    {
        Task<PaginationResponse<StaffProgramMappingResponseDto>> GetProgramsAsync(ProgramPaginationRequest request, UserRole userRole);
        Task<ResultResponseDto<List<StaffProgramMappingResponseDto>>> GetAllProgramsByUserId(int userId, UserRole userRole);
        Task<ResultResponseDto<ClimateProgram>> GetByIdAsync(int id);
        Task<ResultResponseDto<string>> AddBulkProgramsAsync(BulkAddProgramDto q, string image = "");
        Task<ResultResponseDto<ClimateProgram>> EditProgramAsync(int id, AddUpdateProgramDto q);
        Task<ResultResponseDto<bool>> DeleteProgramAsync(int id);
        Task<ResultResponseDto<object>> AssignProgramToUser(int userId, int ClimateProgramID, int assignedByUserId);
        Task<ResultResponseDto<object>> EditAssignProgram(int id,int userId, int ClimateProgramID, int assignedByUserId);
        Task<ResultResponseDto<object>> UnAssignProgram(StaffProgramUnMappingRequestDto requestDto);
        Task<ResultResponseDto<List<StaffProgramMappingResponseDto>>> GetProgramByUserIdForAssessment(int userId);
        Task<ResultResponseDto<ProgramHistoryDto>> GetProgramHistory(int userID, DateTime updatedA, UserRole userRole);
        Task<ResultResponseDto<List<GetProgramsSubmissionHistoryResponseDto>>> GetProgramsProgressByUserId(int userID, DateTime updateAt, UserRole userRole);
        Task<ResultResponseDto<string>> AddUpdateProgram(AddUpdateProgramDto q);
        Task<ResultResponseDto<List<StaffProgramMappingResponseDto>>> GetAllProgramsByLocation(GetNearestProgramRequestDto r);
        Task<ResultResponseDto<List<StaffProgramMappingResponseDto>>> GetAiAccessProgram(int userId, UserRole userRole);        
        Task<ResultResponseDto<byte[]>> ExportPrograms(ExportProgramsWithOptionDto request, int userId, UserRole userRole);
    }
}
