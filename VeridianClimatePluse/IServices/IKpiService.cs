using VeridianClimatePulse.Common.Models;
using VeridianClimatePulse.Dtos.ClientDto;
using VeridianClimatePulse.Dtos.CommonDto;
using VeridianClimatePulse.Dtos.kpiDto;
using VeridianClimatePulse.Enums;
using VeridianClimatePulse.Models;

namespace VeridianClimatePulse.IServices
{
    public interface IKpiService
    {
        Task<PaginationResponse<GetAnalyticalLayerResultDto>> GetAnalyticalLayerResults(GetAnalyticalLayerRequestDto request, int userId, UserRole role, TieredAccessPlan userPlan = TieredAccessPlan.Pending);
        Task<ResultResponseDto<List<AnalyticalLayer>>> GetAllKpi(int userId, UserRole role);
        Task<ResultResponseDto<CompareProgramResponseDto>> ComparePrograms(CompareProgramsRequestDto c, int userId, UserRole role, bool applyPagination = true);

        Task<Tuple<string, byte[]>> ExportComparePrograms(CompareProgramsRequestDto request, int userId, UserRole role);
        Task<ResultResponseDto<GetMutiplekpiLayerResultsDto>> GetMutiplekpiLayerResults(GetMutiplekpiLayerRequestDto request, int userId, UserRole role, TieredAccessPlan userPlan = TieredAccessPlan.Pending);
    }
}
