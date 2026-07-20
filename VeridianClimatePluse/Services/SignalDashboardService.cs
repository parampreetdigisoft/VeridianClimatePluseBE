using VeridianClimatePulse.Common.Implementation;
using VeridianClimatePulse.Common.Interface;
using VeridianClimatePulse.Common.Models;
using VeridianClimatePulse.Common.Models.views;
using VeridianClimatePulse.Data;
using VeridianClimatePulse.Dtos.dashboard;
using VeridianClimatePulse.IServices;
using VeridianClimatePulse.Models;
using Microsoft.EntityFrameworkCore;

namespace VeridianClimatePulse.Services
{
    public class SignalDashboardService : ISignalDashboardService
    {
        private const int HealthStressTestModeId = 1;
        private const int EarlyWarningModeId = 2;
        private const int ResilienceModeId = 3;

        private readonly ApplicationDbContext _context;
        private readonly IAppLogger _appLogger;
        private readonly ICommonService _commonService;

        public SignalDashboardService(ApplicationDbContext context, IAppLogger appLogger, ICommonService commonService)
        {
            _context = context;
            _appLogger = appLogger;
            _commonService = commonService;
        }

        public Task<ResultResponseDto<DashboardModeResponseDto>> GetPeaceStressTestDashboard(int climateProgramID, int userId, UserRole userRole)
            => GetDashboardMode(HealthStressTestModeId, climateProgramID, userId, userRole, "Health stress test dashboard generated successfully.");

        public Task<ResultResponseDto<DashboardModeResponseDto>> GetEarlyWarningDashboard(int climateProgramID, int userId, UserRole userRole)
            => GetDashboardMode(EarlyWarningModeId, climateProgramID, userId, userRole, "Early warning dashboard generated successfully.");

        public Task<ResultResponseDto<DashboardModeResponseDto>> GetResilienceScorecard(int climateProgramID, int userId, UserRole userRole)
            => GetDashboardMode(ResilienceModeId, climateProgramID, userId, userRole, "Resilience scorecard generated successfully.");

        private async Task<ResultResponseDto<DashboardModeResponseDto>> GetDashboardMode(
            int dashboardModeId,
            int climateProgramID,
            int userId,
            UserRole userRole,
            string successMessage)
        {
            try
            {
                if (userRole == UserRole.ProgramUser && !await ValidateProgramAccess(climateProgramID, userId))
                {
                    return ResultResponseDto<DashboardModeResponseDto>.Failure(new[] { "You don't have access to this program data." });
                }

                var dashboardMode = await _context.DashboardModes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.DashboardModeID == dashboardModeId);

                if (dashboardMode == null)
                {
                    return ResultResponseDto<DashboardModeResponseDto>.Failure(new[] { "Dashboard configuration not found." });
                }

                var mappings = await LoadActiveMappings(dashboardModeId);
                if (!mappings.Any())
                {
                    return ResultResponseDto<DashboardModeResponseDto>.Failure(new[] { "Dashboard KPI mappings not found." });
                }

                var interpretations = await _context.DashboardInterpretations
                    .AsNoTracking()
                    .Where(x => x.DashboardModeID == dashboardModeId)
                    .ToListAsync();

                var spResults = await _commonService.GetDashboardModeResults(userId, (int)userRole, dashboardModeId, climateProgramID);
                var spResultsByQuestion = spResults
                    .Where(x => x.QuestionID.HasValue)
                    .GroupBy(x => x.QuestionID!.Value)
                    .ToDictionary(g => g.Key, g => g.First());



                var isProgramUser = userRole == UserRole.ProgramUser;

                var Questions = mappings.Select(q => MapQuestionScore(q, spResultsByQuestion, interpretations, isProgramUser)).ToList();


                var response = new DashboardModeResponseDto
                {
                    ClimateProgramID = climateProgramID,
                    DashboardModeID = dashboardModeId,
                    ModeName = dashboardMode.ModeName,
                    Description = dashboardMode.Description,
                    Questions = Questions,
                    DashboardInterpretations = interpretations

                };

                return ResultResponseDto<DashboardModeResponseDto>.Success(response, new[] { successMessage });
            }
            catch (Exception ex)
            {
                await _appLogger.LogAsync($"Error in GetDashboardMode for mode {dashboardModeId}", ex);
                return ResultResponseDto<DashboardModeResponseDto>.Failure(new[] { "There is an error, please try later" });
            }
        }

        private static DashboardQuestionScoreDto MapQuestionScore(
            DashboardModeKPIMapping question,
            IReadOnlyDictionary<int, GetDashboardModeResult> spResultsByQuestion,
            IReadOnlyList<DashboardInterpretation> interpretations,
            bool isProgramUser)
        {
            var hasData = spResultsByQuestion.TryGetValue(question.QuestionID, out var result);
            var dto = new DashboardQuestionScoreDto
            {
                QuestionID = question.QuestionID,
                QuestionDescription = question.Description ?? ""
            };

            if (!hasData)
            {
                return dto;
            }

            dto.AiScore = result!.AiQuestionScore;
            dto.AiTotalScore = result.AiTotalScore;
            dto.AiTotalAns = result.AiTotalAns;
            dto.AiTotalNA = result.AiTotalNA;
            dto.AiTotalUnknown = result.AiTotalUnknown;
            dto.AiTotalUnknown = result.AiTotalUnknown;
            dto.AiUpdatedAt = result.AiUpdatedAt;

            if (!isProgramUser)
            {
                dto.EvaluationScore = result.QuestionScore;
                dto.EvaluationTotalScore = result.TotalScore;
                dto.EvaluationTotalAns = result.TotalAns;
                dto.EvaluationTotalNA = result.TotalNA;
                dto.EvaluationTotalUnknown = result.TotalUnknown;
                dto.EvaluationUpdatedAt = result.UpdatedAt;
            }

            var scoreForInterpretation = isProgramUser ? result.AiQuestionScore : result.QuestionScore;
            ApplyInterpretation(dto, interpretations, scoreForInterpretation.GetValueOrDefault());

            return dto;
        }

        private static void ApplyInterpretation(DashboardQuestionScoreDto dto, IReadOnlyList<DashboardInterpretation> interpretations, decimal score)
        {
            var interpretation = MatchInterpretation(interpretations, score);
        
        }

        private static DashboardInterpretation? MatchInterpretation(IReadOnlyList<DashboardInterpretation> interpretations, decimal score)
        {
            return interpretations.FirstOrDefault(x =>
                (!x.MinRange.HasValue || score >= x.MinRange.Value) &&
                (!x.MaxRange.HasValue || score <= x.MaxRange.Value));
        }

        private async Task<bool> ValidateProgramAccess(int climateProgramID, int userId)
        {
            return await _context.ClientProgramMappings
                .AsNoTracking()
                .AnyAsync(x => x.UserID == userId && x.ClimateProgramID == climateProgramID && x.IsActive);
        }

        private async Task<List<DashboardModeKPIMapping>> LoadActiveMappings(int dashboardModeId)
        {
            return await _context.DashboardModeKPIMappings
                .AsNoTracking()
                .Where(x => x.DashboardModeID == dashboardModeId && x.IsActive && !x.IsDeleted)
                .ToListAsync();
        }
    }
}
