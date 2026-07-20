using VeridianClimatePulse.Common.Models;
using VeridianClimatePulse.Dtos.AiDto;
using VeridianClimatePulse.Dtos.AssessmentDto;
using VeridianClimatePulse.Dtos.ClientDto;
using VeridianClimatePulse.Dtos.CommonDto;
using VeridianClimatePulse.Dtos.kpiDto;
using VeridianClimatePulse.Dtos.ProgramDto;
using VeridianClimatePulse.Dtos.PublicDto;
using VeridianClimatePulse.Enums;
using VeridianClimatePulse.Models;

namespace VeridianClimatePulse.IServices
{
    public interface IClientService
    {
        Task<List<Pillar>> GetAllAsync(int userId, UserRole userRole);
        Task<ResultResponseDto<List<PartnerProgramResponseDto>>> GetClientPrograms(int userID);
        Task<ResultResponseDto<ProgramHistoryDto>> GetProgramHistory(int userId, TieredAccessPlan tier);
        Task<ResultResponseDto<List<GetProgramsSubmissionHistoryResponseDto>>> GetProgramProgressByUserId(int userID);
        Task<GetProgramQuestionHistoryResponseDto> GetProgramQuestionHistory(UserProgramRequestDto userProgramRequestDto);
        Task<PaginationResponse<ProgramResponseDto>> GetProgramAsync(PaginationRequest request);
        Task<ResultResponseDto<ProgramDetailsDto>> GetProgramDetails(UserProgramRequestDto userProgramRequestDto);
        Task<ResultResponseDto<List<ProgramPillarQuestionDetailsDto>>> GetProgramPillarDetails(StaffProgramGetPillarInfoRequestDto userProgramGetPillarInfoRequestDto);
        Task<ResultResponseDto<string>> AddClientKpisProgramAndPillar(AddClientKpisProgramAndPillar payload,int userID, string tierName);
        Task<ResultResponseDto<List<GetAllKpisResponseDto>>> GetProgramUserKpi(int userID, string tierName);
        Task<ResultResponseDto<CompareProgramResponseDto>> ComparePrograms(CompareProgramsRequestDto c, int userId, string tierName, bool applyPagination = true);
        Task<ResultResponseDto<AiProgramPillarResponseDto>> GetAIProgramPillars(AiProgramPillarRequestDto r, int userID, string tierName);
        Task<Tuple<string, byte[]>> ExportComparePrograms(CompareProgramsRequestDto request, int userId, string tierName);
    }
}
