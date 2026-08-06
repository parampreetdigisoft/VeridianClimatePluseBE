using VeridianClimatePulse.Common.Models;
using VeridianClimatePulse.Dtos.dashboard;
using VeridianClimatePulse.Models;

namespace VeridianClimatePulse.IServices
{
    public interface ISignalDashboardService
    {
        Task<ResultResponseDto<DashboardModeResponseDto>> GetAmbitionDeliveryIndexDashboard(int climateProgramID, int userId, UserRole userRole);
        Task<ResultResponseDto<DashboardModeResponseDto>> GetDiplomaticRiskDashboard(int climateProgramID, int userId, UserRole userRole);
        Task<ResultResponseDto<DashboardModeResponseDto>> GetReadinessScorecardDashboard(int climateProgramID, int userId, UserRole userRole);
    }
}
