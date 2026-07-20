using VeridianClimatePulse.Common.Models;
using VeridianClimatePulse.Dtos.chatDto;
using VeridianClimatePulse.Models;

namespace VeridianClimatePulse.IServices
{
    public interface IChatService
    {
        Task<ResultResponseDto<List<AIAssistantFAQDto>>> GetAssistantFAQDs(int userId, UserRole userRole);
        Task<ResultResponseDto<ChatResponseDto>> AskAboutProgram(ProgramChatRequestDto request, int userId, UserRole userRole);
        Task<ResultResponseDto<ChatResponseDto>> AskAboutGlobal(ChatGlobalAskQuestionRequestDto request, int userId, UserRole userRole);
        Task<ResultResponseDto<ChatResponseDto>> CrossComparision(CrossComparisionRequestDto request, int userId, UserRole userRole);
        Task<ResultResponseDto<ChatProgramExecutiveSlidesResponse>> GetProgramSlides(int climateProgramID, int userId, UserRole userRole);
    }
}
