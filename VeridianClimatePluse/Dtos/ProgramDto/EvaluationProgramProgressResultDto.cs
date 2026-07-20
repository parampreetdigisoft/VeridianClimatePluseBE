namespace VeridianClimatePulse.Dtos.ProgramDto
{
    public class EvaluationProgramProgressResultDto
    {
        public int PillarID { get; set; }
        public double Weight { get; set; }
        public bool Reliability { get; set; }
        public int ClimateProgramID { get; set; }
        public int TotalScore { get; set; }
        public int TotalAns { get; set; }
        public decimal ScoreProgress { get; set; }
        public decimal AIProgress { get; set; }
        public decimal NormalizedValue { get; set; }
        public int TotalAssessments { get; set; }
        public int UserID { get; set; }
    }

    public class ProgramRankingResultDto
    {
        public int ClimateProgramID { get; set; }
        public string ProgramName { get; set; }
        public int TotalProgram { get; set; }
        public string Location { get; set; }
        public int ProgramsRank { get; set; }
        public int TotalProgramInRegion { get; set; }
        public int RegionRank { get; set; }
        public decimal? ProgramAIScore { get; set; }
        public int? DataYear { get; set; }
    }

    public class EvaluationProgramProgressHistoryResultDto
    {
        public int PillarID { get; set; }
        public double Weight { get; set; }
        public bool Reliability { get; set; }
        public int ClimateProgramID { get; set; }
        public int TotalScore { get; set; }
        public int TotalAns { get; set; }
        public decimal ScoreProgress { get; set; }
        public int Year { get; set; }
        public decimal NormalizedValue { get; set; }
        public int TotalAssessments { get; set; }
        public int UserID { get; set; }
    }
}
