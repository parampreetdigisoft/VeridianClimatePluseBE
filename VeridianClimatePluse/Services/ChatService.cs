
using Microsoft.Extensions.Caching.Memory;
using VeridianClimatePulse.Common.Interface;
using VeridianClimatePulse.Common.Models;
using VeridianClimatePulse.Data;
using VeridianClimatePulse.Dtos.chatDto;
using VeridianClimatePulse.IServices;
using VeridianClimatePulse.Models;

namespace VeridianClimatePulse.Services
{
    public class ChatService : IChatService
    {
        #region  constructor
        private readonly ApplicationDbContext _context;
        private readonly IAppLogger _appLogger;
        private readonly IAIAnalyzeService _aIAnalyzeService;
        private readonly IMemoryCache _cache;
        private readonly ICommonService _commonService;
        public ChatService(ApplicationDbContext context, IMemoryCache cache,
            IAppLogger appLogger, IAIAnalyzeService aIAnalyzeService, ICommonService commonService)
        {
            _context = context;
            _appLogger = appLogger;
            _aIAnalyzeService = aIAnalyzeService;
            _cache = cache;
            _commonService = commonService;
        }
        public async Task<ResultResponseDto<List<AIAssistantFAQDto>>> GetAssistantFAQDs(int userId, UserRole userRole)
        {
            try
            {
                var faqs = _context.AIAssistantFAQ
                    .Where(x => x.IsActive)
                    .Select(x => new AIAssistantFAQDto
                    {
                        FAQID = x.FAQID,
                        Related = x.Related,
                        Category = x.Category,
                        QuestionText = x.QuestionText,
                        DisplayOrder = x.DisplayOrder,
                        IsAnsweredFaq =  !string.IsNullOrEmpty(x.AnswerText)
                    }).ToList();

                return ResultResponseDto<List<AIAssistantFAQDto>>.Success(faqs, new[] { "Faqs get successfully" });
            }
            catch (Exception ex)
            {
                await _appLogger.LogAsync("An error occurred while getting the GetAssistantFAQDs request.", ex);
                return ResultResponseDto<List<AIAssistantFAQDto>>.Failure(new[] { "An error occurred while processing your request. Please try again later." });
            }
        }

        public async Task<ResultResponseDto<ChatResponseDto>> AskAboutProgram(ProgramChatRequestDto request, int userId, UserRole userRole)
        {
            try
            {
                if(userRole == UserRole.ProgramUser)
                {
                    var isValidProgram = _context.ClientProgramMappings.Where(x => x.UserID == userId).Any(c => c.ClimateProgramID == request.ClimateProgramID);
                    if (!isValidProgram)
                    {
                        return ResultResponseDto<ChatResponseDto>.Failure(new[] { "You don't have access to this program data." });
                    }
                }

                var r = new ChatProgramAskQuestionRequest
                {
                    ClimateProgramID = request.ClimateProgramID,
                    PillarID = request.PillarID,
                    QuestionText = request.QuestionText,
                    FAQID = request.FAQID,
                    HistoryText = request.HistoryText
                };

                var resutl = await _aIAnalyzeService.ChatProgramAsk(r);
          
                if (resutl == null || resutl.Success != true)
                {
                    return ResultResponseDto<ChatResponseDto>.Failure(
                        new[] { resutl?.Message ?? "Failed to query request from VCP Aevum." }
                    );
                }

                return ResultResponseDto<ChatResponseDto>.Success(new ChatResponseDto
                {
                    ClimateProgramID = request.ClimateProgramID,
                    PillarID = request.PillarID,
                    QuestionText = request.QuestionText,
                    FAQID = request.FAQID,
                    ResponseText = resutl.Result ?? "No response from ."
                });
            }
            catch (Exception ex)
            {
                await _appLogger.LogAsync("An error occurred while processing the AskAboutProgram request.", ex);
                return ResultResponseDto<ChatResponseDto>.Failure(new[] { "An error occurred while processing your request. Please try again later." });
            }
        }

        public async Task<ResultResponseDto<ChatResponseDto>> AskAboutGlobal(ChatGlobalAskQuestionRequestDto request, int userId, UserRole userRole)
        {
            try
            {
                var r = new ChatGlobalAskQuestionRequest
                {  
                    QuestionText = request.QuestionText,
                    FAQID = request.FAQID,
                    HistoryText = request.HistoryText
                };

                var resutl = await _aIAnalyzeService.ChatGlobalAsk(r);

                if (resutl == null || resutl.Success != true)
                {
                    return ResultResponseDto<ChatResponseDto>.Failure(
                        new[] { resutl?.Message ?? "Failed to query request from VCP Aevum." }
                    );
                }

                return ResultResponseDto<ChatResponseDto>.Success(new ChatResponseDto
                {       
                    QuestionText = request.QuestionText,
                    FAQID = request.FAQID,
                    ResponseText = resutl.Result ?? "An error occurred or we do not have an answer for that."
                });
            }
            catch (Exception ex)
            {
                await _appLogger.LogAsync("An error occurred while processing the AskAboutGlobal request.", ex);
                return ResultResponseDto<ChatResponseDto>.Failure(new[] { "An error occurred while processing your request. Please try again later." });
            }
        }

        public async Task<ResultResponseDto<ChatResponseDto>> CrossComparision(CrossComparisionRequestDto request, int userId, UserRole userRole)
        {
            try
            {
                if (userRole == UserRole.ProgramUser)
                {
                    var userClimateProgramIDs = _context.ClientProgramMappings
                        .Where(x=>x.UserID == userId)
                        .Select(x => x.ClimateProgramID)
                        .ToList();

                    var isValidProgram = request.ClimateProgramIDs
                        .All(id => userClimateProgramIDs.Contains(id));

                    if (!isValidProgram)
                    {
                        return ResultResponseDto<ChatResponseDto>
                            .Failure(new[] { "You don't have access to this program data." });
                    }
                }
                var r = new CrossComparisionRequest
                {  
                    ClimateProgramIDs = request.ClimateProgramIDs,
                    QuestionText = request.QuestionText,
                    HistoryText = request.HistoryText
                };

                var resutl = await _aIAnalyzeService.CrossComparision(r);

                if (resutl == null || resutl.Success != true)
                {
                    return ResultResponseDto<ChatResponseDto>.Failure(
                        new[] { resutl?.Message ?? "Failed to query request from VCP Aevum." }
                    );
                }

                return ResultResponseDto<ChatResponseDto>.Success(new ChatResponseDto
                {       
                    QuestionText = request.QuestionText,
                    FAQID = null,
                    ResponseText = resutl.Result ?? "An error occurred or we do not have an answer for that."
                });
            }
            catch (Exception ex)
            {
                await _appLogger.LogAsync("An error occurred while processing the AskAboutGlobal request.", ex);
                return ResultResponseDto<ChatResponseDto>.Failure(new[] { "An error occurred while processing your request. Please try again later." });
            }
        }

        public async Task<ResultResponseDto<ChatProgramExecutiveSlidesResponse>> GetProgramSlides(int climateProgramID, int userId, UserRole userRole)
        {
            string cacheKey = $"ProgramSlides_{climateProgramID}";

            try
            {
                if (userRole == UserRole.ProgramUser && climateProgramID != 0)
                {
                    var isValidProgram = _context.ClientProgramMappings.Where(x => x.UserID == userId).Any(c => c.ClimateProgramID == climateProgramID);
                    if (!isValidProgram)
                    {
                        return ResultResponseDto<ChatProgramExecutiveSlidesResponse>.Failure(new[] { "You don't have access to this program data." });
                    }
                }

                var programRanking = await _commonService.GetProgramRankings(climateProgramID);
                ProgramRankingResponseDto programResult;
                List<PillarsUserHistroyResponseDto> pillars;

                if (climateProgramID == 0)
                {
                    if (programRanking != null && programRanking.Any())
                    {
                        var averageScore = programRanking.Average(p => p.ProgramAIScore ?? 0);
                        var totalProgramCount = programRanking.Count();

                        pillars = (
                            from p in _context.Pillars.Where(x => x.IsActive && !x.IsDeleted)

                            join x in _context.AIPillarScores
                            on p.PillarID equals x.PillarID into pillarScores

                            from score in pillarScores.DefaultIfEmpty()

                            group score by new { p.PillarID, p.PillarName, p.DisplayOrder, p.ImagePath } into g

                            select new PillarsUserHistroyResponseDto
                            {
                                PillarID = g.Key.PillarID,
                                PillarName = g.Key.PillarName ?? "",
                                DisplayOrder = g.Key.DisplayOrder,
                                PillarScore = g.Where(s => s != null).Average(s => s.AIProgress ?? 0),
                                ImagePath = g.Key.ImagePath
                            }
                        ).ToList();

                        programResult = new ProgramRankingResponseDto
                        {
                            ClimateProgramID = 0,
                            ProgramName = "All Programs Average",
                            ProgramAIScore = averageScore,
                            DataYear = programRanking.FirstOrDefault()?.DataYear,
                            ProgramRank = 1,
                            TotalProgram = totalProgramCount,
                            Pillars = pillars.OrderBy(p => p.DisplayOrder).ToList()
                        };
                    }
                    else
                    {
                        return ResultResponseDto<ChatProgramExecutiveSlidesResponse>.Failure(new[] { "No programs found to calculate average." });
                    }
                }
                else
                {
                    var program = programRanking.FirstOrDefault(x => x.ClimateProgramID == climateProgramID);
                    if (program == null)
                    {
                        return ResultResponseDto<ChatProgramExecutiveSlidesResponse>.Failure(new[] { "Program not found." });
                    }

                    pillars = (
                        from p in _context.Pillars.Where(x => x.IsActive && !x.IsDeleted)

                        join x in _context.AIPillarScores
                            .Where(a => a.ClimateProgramID == program.ClimateProgramID)
                        on p.PillarID equals x.PillarID into pillarScores

                        from score in pillarScores.DefaultIfEmpty()

                        select new PillarsUserHistroyResponseDto
                        {
                            PillarID = p.PillarID,
                            PillarName = p.PillarName ?? "",
                            DisplayOrder = p.DisplayOrder,
                            PillarScore = score != null ? score.AIProgress ?? 0 : 0,
                            ImagePath = p.ImagePath
                        }
                    ).ToList();

                    if (userRole == UserRole.ProgramUser && climateProgramID != 0)
                    {
                        var validPillars = _context.ClientPillarMappings.Where(x => x.UserID == userId).Select(x => x.PillarID);
                        pillars = pillars.Where(x => validPillars.Contains(x.PillarID)).ToList();
                    }

                    programResult = new ProgramRankingResponseDto
                    {
                        ClimateProgramID = program.ClimateProgramID,
                        ProgramName = program.ProgramName,
                        ProgramAIScore = program.ProgramAIScore,
                        DataYear = program.DataYear,
                        ProgramRank = program.ProgramRank,
                        TotalProgram = program.TotalPrograms,
                        Pillars = pillars.OrderBy(p => p.DisplayOrder).ToList()
                    };
                }
                if (_cache.TryGetValue(cacheKey, out ChatProgramExecutiveSlidesResponse cachedResult))
                {
                    cachedResult.Result.Program = programResult;

                    return ResultResponseDto<ChatProgramExecutiveSlidesResponse>.Success(
                        cachedResult,
                        new List<string>
                        {
                            "Program executive slides fetched successfully from cache."
                        }
                    );
                }

                var response = new ChatProgramExecutiveSlidesResponse
                {
                    Success = true,
                    Message = "Program executive slides fetched successfully.",
                    Result = new ProgramExecutiveSlidesResult
                    {
                        Program = programResult,
                        RecentPerformance = new PerformanceSummary(),
                        CombinedRisks = new List<CombinedRiskItem>(),
                        EarlyWarnings = new List<EarlyWarningItem>()
                    }
                };

                return ResultResponseDto<ChatProgramExecutiveSlidesResponse>.Success(
                    response,
                    new List<string>
                    {
                         "Program executive slides fetched successfully."
                    }
                );
            }
            catch (Exception ex)
            {
                await _appLogger.LogAsync(
                    "An error occurred while processing the GetProgramSlides request.",
                    ex
                );

                return ResultResponseDto<ChatProgramExecutiveSlidesResponse>.Failure(
                    new[]
                    {
                        "An error occurred while processing your request. Please try again later."
                    }
                );
            }
        }


        #endregion
    }
}
