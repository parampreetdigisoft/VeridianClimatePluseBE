using VeridianClimatePulse.Common.Models;
using VeridianClimatePulse.Dtos.AssessmentDto;
using VeridianClimatePulse.Dtos.CommonDto;
using VeridianClimatePulse.Dtos.QuestionDto;
using VeridianClimatePulse.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using VeridianClimatePulse.Dtos.PillarDto;

namespace VeridianClimatePulse.IServices
{
    public interface IQuestionService
    {
        Task<List<GetPillarDTO>> GetPillarsAsync();
        Task<PaginationResponse<GetQuestionResponse>> GetQuestionsAsync(GetQuestionRequestDto requestDto);
        Task<Question> AddQuestionAsync(Question q);
        Task<ResultResponseDto<string>> AddUpdateQuestion(AddUpdateQuestionDto q);
        Task<ResultResponseDto<string>> AddBulkQuestion(AddBulkQuestionsDto q);
        Task<Question> EditQuestionAsync(int id, Question q);
        Task<bool> DeleteQuestionAsync(int id);
        Task<ResultResponseDto<GetPillarQuestionByProgramResponse>> GetQuestionsByProgramIDAsync(StaffProgramPillerRequestDto request, int userId);
        Task<Tuple<string,byte[]>> ExportAssessment(int staffProgramMappingID, int userId, UserRole role);
        Task<ResultResponseDto<List<QuestionsByUserPillarsResponsetDto>>> GetQuestionsHistoryByPillar(GetProgramPillarHistoryRequestDto requestDto, UserRole role);
        Task<ResultResponseDto<GetPillarQuestionByProgramResponse>> GetQuestionsByProgramMappingIdForAnalyst(StaffProgramPillerRequestDto request, int userId);
    }
} 