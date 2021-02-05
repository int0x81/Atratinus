using Atratinus.Core;
using Atratinus.Core.Models;
using Atratinus.DataTransform.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Atratinus.DataTransform
{
    class Program
    {
        static void Main()
        {
            ConfigureSerilog();

            var config = LoadConfiguration();

            if (config == null)
                return;

            if (!FileHelper.TryCreateDirectory(config.OutputFolder))
                return;

            var files = FileHelper.EnumerateEDGARFiles(config.EDGARFolder);
            var alteryxResult = FileHelper.ReadInAlteryxData(config.CleaInvestmentActivitySet);
            var filesToTake = MatchFiles(files, alteryxResult.InvestmentActivities);
            var amountVirtualCores = Environment.ProcessorCount;
            var tasks = new Task<FileAnalysisBatchResult>[amountVirtualCores];
            int sliceRange;

            if (tasks.Length == 1)
                sliceRange = filesToTake.Count;
            else
                sliceRange = filesToTake.Count / (tasks.Length - 1); //-1 because we need one task that will handle the modulus

            if (filesToTake.Count == 0)
                return;

            var supervised = FileHelper.ReadInSupervised(config.SupervisedFile, config.EDGARFolder);
            
            var investors = FileHelper.ReadInInvestorData(config.InvestorsFile);

            if (alteryxResult == null)
                return;

            Log.Information($"Utilizing {tasks.Length} threads for file analysis");
            Log.Information($"Starting data tranformation: {DateTime.Now}");
            
            for (int c = 1; c < tasks.Length; c++) //Starting at 1; the 0-nth thread will do the modulos
            { 
                var start = (c - 1) * sliceRange;
                var limit = c * sliceRange;
                var taskNumber = c;

                tasks[c] = Task.Run(() => AnalyzeSliceOfFiles(start, limit, filesToTake, alteryxResult.InvestmentActivities, supervised, investors, config));
            }

            tasks[0] = Task.Run(() => AnalyzeSliceOfFiles((tasks.Length - 1) * sliceRange, filesToTake.Count, filesToTake, alteryxResult.InvestmentActivities, supervised, investors, config));

            FinalizeRun(tasks, config, Convert.ToUInt32(files.Length), Convert.ToUInt32(filesToTake.Count), alteryxResult);

            Log.Information($"Finished data tranformation: {DateTime.Now}");
            Log.Information($"You may close this window now...");
        }

        /// <summary>
        /// Returns only the files that can also be found in cleas dataset
        /// </summary>
        private static IReadOnlyList<string> MatchFiles(string[] files, IReadOnlyDictionary<string, InvestmentActivity> alteryxSet)
        {
            var result = new List<string>();

            foreach (var file in files)
            {
                if (alteryxSet.ContainsKey(Path.GetFileNameWithoutExtension(file)))
                    result.Add(file);
            }

            return result.AsReadOnly();
        }

        private static void ConfigureSerilog()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
        }

        private static AtratinusConfiguration LoadConfiguration()
        {
            try
            {
                using StreamReader reader = new StreamReader("settings.json");
                string json = reader.ReadToEnd();
                var config = JsonSerializer.Deserialize<AtratinusConfiguration>(json, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                return config;
            }
            catch (JsonException)
            {
                Log.Error("Unable to load settings. The settings.json file was not in JSON format. " +
                    "If you are using the template, make sure the comments are removed since comments are not allowed in JSON.");
                return null;
            }
            catch (FileNotFoundException)
            {
                Log.Error("The settings.json file was not found. Please make sure it's placed within the same directory as the program.");
                return null;
            }
        }

        private static FileAnalysisBatchResult AnalyzeSliceOfFiles(int start, int limit, IReadOnlyList<string> files, IReadOnlyDictionary<string, InvestmentActivity> originAccessions, 
            IReadOnlyDictionary<string, int> supervised, InvestorHashTableSet investors, AtratinusConfiguration config)
        {
            IList<InvestmentActivity> investments = new List<InvestmentActivity>();
            IList<Supervised> trainingData = new List<Supervised>();
            var report = new FullQualityReport();

            for (int fileIndex = start; fileIndex < limit; fileIndex++)
            {
                var analysis = AnalyzeFile(files[fileIndex], originAccessions, supervised, investors);
                var fileReport = QualityGate.Measure(analysis.Investment, config);
                report.ConsiderQualityReport(files[fileIndex], fileReport);

                if (!fileReport.ShouldBeConsidered)
                    continue;

                if (analysis.Supervised != null)
                    trainingData.Add(analysis.Supervised);

                investments.Add(analysis.Investment);
            }

            return new FileAnalysisBatchResult() { Accessions = investments, TrainingData = trainingData, QualityReport = report };
        }

        private static FileAnalysisResult AnalyzeFile(string file, IReadOnlyDictionary<string, InvestmentActivity> alteryxInvestmentSet, 
            IReadOnlyDictionary<string, int> supervised, InvestorHashTableSet investors)
        {
            InvestmentActivity investmentActivity = EdgarAnalyzer.Analyze(file);
            Supervised trainingDatum = null;
            Helper.MergeAccession(investmentActivity, alteryxInvestmentSet);

            AddFundAndFirmType(investmentActivity, investors);

            if (supervised.TryGetValue(investmentActivity.AccessionNumber, out var typeId))
            {
                trainingDatum = new Supervised(investmentActivity.AccessionNumber, investmentActivity.PurposeOfTransaction, typeId, investmentActivity.TypeOfReportingPerson);
                investmentActivity.PurposeAnalyzedByML = false;
                investmentActivity.PurposeOfTransactionTypeId = typeId;
            }

            return new FileAnalysisResult() { Investment = investmentActivity, Supervised = trainingDatum };
        }
        
        private static void AddFundAndFirmType(InvestmentActivity accession, InvestorHashTableSet investors)
        {
            if (string.IsNullOrEmpty(accession.ActivistInvestorName))
                return;

            var transformed = accession.ActivistInvestorName.Replace(" ", "").ToLower();

            Investor possibleMatch = null;

            if(investors.FirmNameAsKey.TryGetValue(transformed, out var firmNameMatch))
                possibleMatch = firmNameMatch;
            else if (investors.FundNameAsKey.TryGetValue(transformed, out var fundNameMatch))
                possibleMatch = fundNameMatch;

            if (possibleMatch != null)
            {
                accession.ActivistInvestorId = possibleMatch.Id.Substring(2); //CLEA 2.xxx system
                accession.ActivistInvestorFundType = possibleMatch.FundType;
                accession.ActivistInvestorFirmType = possibleMatch.FirmType;

                return;
            }
        }

        private static void FinalizeRun(Task<FileAnalysisBatchResult>[] tasks, AtratinusConfiguration config, uint enumeratedFiles, uint amountFiles, AlteryxResult alteryxResult)
        {
            Task.WaitAll(tasks);

            List<Supervised> trainingData = new List<Supervised>();
            List<InvestmentActivity> investments = new List<InvestmentActivity>();
            var report = new FullQualityReport();

            for (int c = 0; c < tasks.Length; c++)
            {
                trainingData.AddRange(tasks[c].Result.TrainingData);
                investments.AddRange(tasks[c].Result.Accessions);
                report.Merge(tasks[c].Result.QualityReport);
            }

            Helper.SaveAccessionsAsCSV(investments, config.OutputFolder, false);
            FileHelper.SaveTrainingData(trainingData, config.OutputFolder);
            report.BuildAndSaveReport(config.OutputFolder, enumeratedFiles, amountFiles, alteryxResult);
        }
    }
}
