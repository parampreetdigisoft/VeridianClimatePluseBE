using Microsoft.EntityFrameworkCore;
using VeridianClimatePulse.Common.Implementation;
using VeridianClimatePulse.Common.Interface;
using VeridianClimatePulse.Common.Models;
using VeridianClimatePulse.Data;
using VeridianClimatePulse.Dtos.ClientDto;
using VeridianClimatePulse.Dtos.dashboard;
using VeridianClimatePulse.IServices;
using VeridianClimatePulse.Models;

namespace VeridianClimatePulse.Services
{
    public class SignalDashboardService : ISignalDashboardService
    {
        private const int AmbitionDeliveryIndexModeId = 1;
        private const int DiplomaticRiskModeId = 2;
        private const int InstitutionalReadinessModeId = 3;

        private readonly ApplicationDbContext _context;
        private readonly IAppLogger _appLogger;
        private readonly ICommonService _commonService;

        public SignalDashboardService(ApplicationDbContext context, IAppLogger appLogger, ICommonService commonService)
        {
            _context = context;
            _appLogger = appLogger;
            _commonService = commonService;
        }

        public Task<ResultResponseDto<DashboardModeResponseDto>> GetAmbitionDeliveryIndexDashboard(int climateProgramID, int userId, UserRole userRole)
            => GetDashboardMode(AmbitionDeliveryIndexModeId, climateProgramID, userId, userRole, "Ambition–Delivery Index dashboard generated successfully.");

        public Task<ResultResponseDto<DashboardModeResponseDto>> GetDiplomaticRiskDashboard(int climateProgramID, int userId, UserRole userRole)
            => GetDashboardMode(DiplomaticRiskModeId, climateProgramID, userId, userRole, "Diplomatic Risk & Trust Index dashboard generated successfully.");

        public Task<ResultResponseDto<DashboardModeResponseDto>> GetReadinessScorecardDashboard(int climateProgramID, int userId, UserRole userRole)
            => GetDashboardMode(InstitutionalReadinessModeId, climateProgramID, userId, userRole, "Institutional Readiness Scorecard generated successfully.");

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

                var layerIds = mappings.Select(x => x.LayerID).Distinct().ToList();

                var layers = await LoadLayers(layerIds);

                var accessibleLayerIds = await GetAccessibleLayerIds(userId);
                var kpiResults = await LoadLayerResults(climateProgramID, layerIds);
                var vcpScores = await LoadProgramVcpScores(climateProgramID);
                var primaryMappings = OrderMappings(mappings.Where(x => x.PriorityLevel == 1));
                var secondaryMappings = OrderMappings(mappings.Where(x => x.PriorityLevel != 1));
                var primarySignals = BuildSignalCards(primaryMappings, kpiResults,layers, accessibleLayerIds, vcpScores.Score);

                primarySignals.Insert(0, new SignalCardDto
                {
                    LayerID = 0,
                    LayerCode = "VCP",
                    LayerName = "Program Score",
                    Description = "Represents the program's overall progress score based on the latest assessment.",
                    Descriptor = "Overall assessment of the program's current progress and performance.",
                    StrategicAction = "Review the score category and prioritize actions to strengthen performance and improve overall progress.",
                    Code = "VCP Score",
                    Name = "Program Score",
                    Value = vcpScores.Score ?? 0m,
                    Condition = CommonStaticMethods.GetConditionByScore(vcpScores.Score ?? 0m)
                });

                var secondarySignals = BuildSignalCards(secondaryMappings, kpiResults, layers, accessibleLayerIds, vcpScores.Score);
                var vcpLayer = layers.Values.FirstOrDefault(x => x.LayerCode.Equals("VCP", StringComparison.OrdinalIgnoreCase));

                var vcpInterpretation = vcpLayer != null 
                    ? MatchInterpretationByValue(vcpLayer, vcpScores.Score ?? 0m)
                    : null;
                var vcpCondition = CommonStaticMethods.GetConditionByScore(vcpScores.Score ?? 0m);

                var narratives = primarySignals
                    .Where(x => !x.LayerCode.Equals("VCP", StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(x => x.IsAlert)
                    .ThenByDescending(x => x.Value)
                    .Take(4)
                    .Select(x => new NarrativeDto
                    {
                        Headline = $"{x.LayerName} ({x.Condition})",
                        Detail = string.IsNullOrWhiteSpace(x.Descriptor) ? x.Narrative : x.Descriptor
                    })
                    .ToList();

                var allSignals = primarySignals.Concat(secondarySignals).ToList();

                return ResultResponseDto<DashboardModeResponseDto>.Success(
                    new DashboardModeResponseDto
                    {
                        ClimateProgramID = climateProgramID,
                        DashboardModeID = dashboardModeId,
                        ModeName = dashboardMode.ModeName ?? string.Empty,
                        Description = dashboardMode.Description,
                        Year = DateTime.Now.Year,
                        Vcp = vcpScores.Score ?? 0m,
                        ProgramScore = vcpScores.Score ?? 0m,
                        VcpDirectionalMovement = 0m,
                        VcpCondition = vcpCondition,
                        VcpDescriptor = vcpInterpretation?.Descriptor ?? string.Empty,
                        VcpStrategicAction = vcpInterpretation?.Descriptor ?? string.Empty,
                        PrimarySignals = primarySignals,
                        SecondarySignals = secondarySignals,
                        Signals = allSignals,
                        Narratives = narratives
                    }, 
                    new[] { successMessage });
            }
            catch (Exception ex)
            {
                await _appLogger.LogAsync($"Error in GetDashboardMode for mode {dashboardModeId}", ex);
                return ResultResponseDto<DashboardModeResponseDto>.Failure(new[] { "There is an error, please try later" });
            }
        }

        private async Task<Dictionary<int, LayerScoreResult>> LoadLayerResults(int climateProgramID, IEnumerable<int> layerIds)
        {
            var ids = layerIds.Distinct().ToList();
            if (!ids.Any())
            {
                return new Dictionary<int, LayerScoreResult>();
            }

            var rows = await _context.AnalyticalLayerResults
                .AsNoTracking()
                .Where(x =>
                    x.ClimateProgramID == climateProgramID &&
                    ids.Contains(x.LayerID) &&
                    x.AiLastUpdated.HasValue)
                .Select(x => new
                {
                    x.LayerID,
                    x.AiCalValue5,
                    x.AiInterpretationID,
                    x.AiLastUpdated
                })
                .ToListAsync();
            
            return rows
            .GroupBy(x => x.LayerID)
            .ToDictionary(
                g => g.Key,
                g =>
                {
                    var score = g
                        .OrderByDescending(x => x.AiLastUpdated)
                        .First();
                    return new LayerScoreResult
                    {
                        Value = Math.Round(score.AiCalValue5 ?? 0m, 2),
                        InterpretationId = score.AiInterpretationID
                    };
                });
        }

        private static FiveLevelInterpretationDto? ResolveInterpretation(AnalyticalLayer? layer, int? interpretationId)
        {
            if (layer == null || !interpretationId.HasValue)
            {
                return null;
            }

            var match = layer.FiveLevelInterpretations
                .FirstOrDefault(x => x.InterpretationID == interpretationId.Value);

            return match == null ? null : ToInterpretationDto(match);
        }

        private async Task<Dictionary<int, AnalyticalLayer>> LoadLayers(IEnumerable<int> layerIds)
        {
            var ids = layerIds.Distinct().ToList();
            var layers = await _context.AnalyticalLayers
                .AsNoTracking()
                .Include(x => x.FiveLevelInterpretations)
                .Where(x => !x.IsDeleted && ids.Contains(x.LayerID))
                .ToListAsync();

            return layers.ToDictionary(x => x.LayerID);
        }
        private async Task<HashSet<int>> GetAccessibleLayerIds(int userId)
        {
            var layerIds = await _context.ClientPillarMappings
                .AsNoTracking()
                .Where(x => x.UserID == userId && x.IsActive)
                .Join(
                    _context.AnalyticalLayerPillarMappings.AsNoTracking(),
                    up => up.PillarID,
                    lp => lp.PillarID,
                    (up, lp) => lp.LayerID)
                .Distinct()
                .ToListAsync();

            return layerIds.ToHashSet();
        }

        private static List<DashboardModeKPIMapping> OrderMappings(IEnumerable<DashboardModeKPIMapping> mappings)
        {
            return mappings
                .OrderBy(x => x.DisplayOrder ?? int.MaxValue)
                .ToList();
        }

        private async Task<ProgramVcpScores> LoadProgramVcpScores(int climateProgramID)
        {
            var scores = await _context.AIProgramScores
                .AsNoTracking()
                .Where(x =>
                    x.ClimateProgramID == climateProgramID &&
                    x.IsVerified)
                .Select(x => new { x.Year, x.AIProgress })
                .FirstOrDefaultAsync();

            return new ProgramVcpScores
            {
                Score = scores != null ? scores.AIProgress : 0m
            };
        }
        private List<SignalCardDto> BuildSignalCards(
           IEnumerable<DashboardModeKPIMapping> mappings,
           IReadOnlyDictionary<int, LayerScoreResult> kpiResults,
           IReadOnlyDictionary<int, AnalyticalLayer> layers,
           IReadOnlySet<int> accessibleLayerIds,
           decimal? vcpOverride = null)
        {
            var cards = new List<SignalCardDto>();
            foreach (var mapping in mappings)
            {
                if (!layers.TryGetValue(mapping.LayerID, out var layer))
                {
                    continue;
                }

                kpiResults.TryGetValue(mapping.LayerID, out var kpiResult);

                var value = kpiResult?.Value ?? 0m;

                if (vcpOverride.HasValue &&
                    layer.LayerCode.Equals("VCP", StringComparison.OrdinalIgnoreCase))
                {
                    value = vcpOverride.Value;
                }

                var interpretation = ResolveInterpretation(layer, kpiResult?.InterpretationId);
                var condition = interpretation?.Condition ?? ResolveConditionByValue(layer, value);
                var isAlert = IsAlertCondition(condition);

                cards.Add(new SignalCardDto
                {
                    LayerID = layer.LayerID,
                    LayerCode = layer.LayerCode,
                    LayerName = layer.LayerName,
                    Description = CommonStaticMethods.StripHtml(layer.Purpose),
                    Code = layer.LayerCode,
                    Name = layer.LayerName,
                    Value = value,
                    Condition = condition,
                    Descriptor = interpretation?.Descriptor ?? string.Empty,
                    Narrative = interpretation?.Descriptor ?? string.Empty,
                    InterpretationID = interpretation?.InterpretationID ?? 0,
                    IsAlert = isAlert,
                    IsAccessible = accessibleLayerIds.Contains(layer.LayerID),
                    Interpretations = MapInterpretations(layer),
                    DisplayOrder = mapping.DisplayOrder

                });
            }

            return cards;
         
        }


        private static List<FiveLevelInterpretationDto> MapInterpretations(AnalyticalLayer layer)
        {
            return layer.FiveLevelInterpretations
                .OrderByDescending(x => x.MaxRange)
                .Select(x => new FiveLevelInterpretationDto
                {
                    InterpretationID = x.InterpretationID,
                    LayerID = x.LayerID,
                    MinRange = x.MinRange,
                    MaxRange = x.MaxRange,
                    Condition = x.Condition ?? string.Empty,
                    Descriptor = x.Descriptor ?? string.Empty
                })
                .ToList();
        }
      
        private static FiveLevelInterpretationDto? MatchInterpretationByValue(AnalyticalLayer layer, decimal value)
        {
            var match = layer.FiveLevelInterpretations.FirstOrDefault(x =>
                (!x.MinRange.HasValue || value >= x.MinRange.Value) &&
                (!x.MaxRange.HasValue || value <= x.MaxRange.Value));
            return match == null ? null : ToInterpretationDto(match);
        }

        private static FiveLevelInterpretationDto ToInterpretationDto(FiveLevelInterpretation interpretation)
        {
            return new FiveLevelInterpretationDto
            {
                InterpretationID = interpretation.InterpretationID,
                LayerID = interpretation.LayerID,
                MinRange = interpretation.MinRange,
                MaxRange = interpretation.MaxRange,
                Condition = interpretation.Condition ?? string.Empty,
                Descriptor = interpretation.Descriptor ?? string.Empty
            };
        }
        private static string ResolveConditionByValue(AnalyticalLayer? layer, decimal value)
        {
            return MatchInterpretationByValue(layer ?? new AnalyticalLayer(), value)?.Condition ?? "";
        }

        private static bool IsAlertCondition(string condition)
        {
            var normalized = condition.ToLowerInvariant();
            return normalized.Contains("critical") ||
                   normalized.Contains("high") ||
                   normalized.Contains("elevated") ||
                   normalized.Contains("watch");
        }

        private sealed class ProgramVcpScores
        {
            public decimal? Score { get; init; }
        }
        private sealed class LayerScoreResult
        {
            public decimal Value { get; init; }
            public int? InterpretationId { get; init; }
        }

    }
}
