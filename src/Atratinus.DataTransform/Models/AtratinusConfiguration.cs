namespace Atratinus.DataTransform.Models
{
    public class AtratinusConfiguration
    {
        public string EDGARFolder { get; init; }
        public string CleaInvestmentActivitySet { get; init; }
        public string InvestorsFile { get; init; }
        public string SupervisedFile { get; init; }
        public string OutputFolder { get; init; }
        public bool SaveInvestmentActivitiesAsJSON { get; init; }
        public bool OnlyTakeActivistInvestors { get; init; }
    }
}
