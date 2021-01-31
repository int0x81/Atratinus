using System.Collections.Generic;
using System.IO;

namespace Atratinus.Core
{
    public class Constants
    {
        static readonly string root = @"C:\Users\finnf\source\repos\int0x81\Atratinus\data";
        public static string TRAINING_DATA_FILE_PATH { get; } = Path.Combine(root, "trainingData.json");
        //public static string TEST_DATA_FILE_PATH { get; } = Path.Combine(root, "testData.json");
        //public static string ML_MODEL_PATH { get; } = Path.Combine(root, "trainedModel");
        public static string RESULT_FILE_PATH { get; } = Path.Combine(root, "result.csv");

        public static Dictionary<int, string> PurposeOfInterventionTypes { get; } = new Dictionary<int, string>()
        {
            { 1, "Change capital structure" }, // deal with access cash, underlevarage, dividends/ repurchases, stop/ reduce equity issuance, restructuing debt, recapitalization
            { 2, "Alter business strategy" }, // achieve operational efficiency, achieve strategic partnership, obtain focus (business restructuring and spinning off) 
            { 3, "M&A" }, // M&A already happening: carry out M&A: as target (against a deal/ for better terms), carry out M&A: as acquirer (against a deal/ for better terms) Close M&A deal
            { 4, "Sell target company" }, // sell target company or main asset to third party, take control of/ buy out company and/ or take it private
            { 5, "Improve governance" }, // rescind takeover defenses; oust CEO, board chair; achieve board independence and fair representation; obtain more informations dicslosure/potential fraud; alter excess executive compensation/ pay for performance
            { 6, "Bankruptcy" },
            { 7, "Other" }, // company generally undervaluedm simple investment purposes
            { 8, "Trading Strategy by Activist" }
        };
    }
}
