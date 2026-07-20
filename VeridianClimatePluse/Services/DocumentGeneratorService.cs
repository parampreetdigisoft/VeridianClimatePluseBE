
using AssessmentPlatform.Dtos.AiDto;
using AssessmentPlatform.Models;
using VeridianClimatePulse.Common.Interface;
using VeridianClimatePulse.Dtos.AiDto;
using VeridianClimatePulse.IServices;
using VeridianClimatePulse.Models;
using static VeridianClimatePulse.Services.AIComputationService;


namespace VeridianClimatePulse.Services
{
    /// <summary>
    /// Facade that delegates to <see cref="PdfGeneratorService"/> or
    /// <see cref="DocxGeneratorService"/> based on the requested <see cref="DocumentFormat"/>.
    ///
    /// Register as: services.AddScoped&lt;IDocumentGeneratorService, DocumentGeneratorService&gt;()
    /// </summary>
    public sealed class DocumentGeneratorService : IDocumentGeneratorService
    {
        private readonly Common.Interface.IPdfGeneratorService _pdf;
        private readonly IDocxGeneratorService _docx;

        public DocumentGeneratorService(
            Common.Interface.IPdfGeneratorService pdf,
            IDocxGeneratorService docx)
        {
            _pdf = pdf;
            _docx = docx;
        }

        public Task<byte[]> GenerateProgramDetails(
            AiProgramSummeryDto program,
            List<AiProgramPillarResponse> pillars,
            List<KpiChartItem> kpis,
            List<PeerProgramHistoryReportDto> peerProgram,
            UserRole userRole,
        VeridianClimatePulse.IServices.DocumentFormat format = VeridianClimatePulse.IServices.DocumentFormat.Pdf)
        {
             var result = format == VeridianClimatePulse.IServices.DocumentFormat.Docx
                ? _docx.GenerateProgramDetailsDocx(program, pillars, kpis, peerProgram, userRole)
                : _pdf.GenerateProgramDetailsPdf(program, pillars, kpis, peerProgram, userRole);

            return result;
        }

        public Task<byte[]> GeneratePillarDetails(
            AiProgramPillarResponse pillarData,
            UserRole userRole,
            VeridianClimatePulse.IServices.DocumentFormat format = VeridianClimatePulse.IServices.DocumentFormat.Pdf)
            => format == VeridianClimatePulse.IServices.DocumentFormat.Docx
                ? _docx.GeneratePillarDetailsDocx(pillarData, userRole)
                : _pdf.GeneratePillarDetailsPdf(pillarData, userRole);

        public Task<byte[]> GenerateAllProgramsDetails(
            List<AiProgramSummeryDto> programs,
            Dictionary<int, List<AiProgramPillarResponse>> pillarsDict,
            List<KpiChartItem> kpis,
            UserRole userRole,
            VeridianClimatePulse.IServices.DocumentFormat format = VeridianClimatePulse.IServices.DocumentFormat.Pdf)
            => format == VeridianClimatePulse.IServices.DocumentFormat.Docx
                ? _docx.GenerateAllProgramsDetailsDocx(programs, pillarsDict, kpis, userRole)
                : _pdf.GenerateAllProgramsDetailsPdf(programs, pillarsDict, kpis, userRole);
    }
}
