using VeridianClimatePulse.Common.Models.views;
using VeridianClimatePulse.Dtos.PillarDto;
using VeridianClimatePulse.Dtos.ProgramDto;
using VeridianClimatePulse.Models;

namespace VeridianClimatePulse.Common.Interface
{
    public interface ICommonService
    {
        Task<List<EvaluationProgramProgressResultDto>> GetProgramProgressAsync(int userId,int role, int climateProgramID = 0);
        Task<List<EvaluationProgramProgressHistoryResultDto>> GetProgramProgressHistoryAsync(int userId, int role);
        Task<List<GetProgramsProgressAdminDto>> GetProgramProgressForAdmin(int userId, int role, int year);
        Task<List<ProgramRankingResultDto>> GetProgramRankings(int climateProgramID, int year);
        Task<List<GetPillarDTO>> GetPillars();
        void ClearPillarCache();
        Task<List<GetDashboardModeResult>> GetDashboardModeResults(int userId, int role, int dashboardModeID, int climateProgramID = 0);
    }
}
