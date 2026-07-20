using VeridianClimatePulse.Common.Models;
using VeridianClimatePulse.Dtos.dashboard;
using VeridianClimatePulse.Models;

namespace VeridianClimatePulse.IServices
{
    public interface ISignalDashboardService
    {
        Task<ResultResponseDto<DashboardModeResponseDto>> GetPeaceStressTestDashboard(int climateProgramID, int userId, UserRole userRole);
        Task<ResultResponseDto<DashboardModeResponseDto>> GetEarlyWarningDashboard(int climateProgramID, int userId, UserRole userRole);
        Task<ResultResponseDto<DashboardModeResponseDto>> GetResilienceScorecard(int climateProgramID, int userId, UserRole userRole);
    }
}
