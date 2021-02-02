using Atratinus.DataTransform.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Atratinus.DataTransform.Models
{
    public class FullQualityReport
    {
        readonly Dictionary<string, List<string>> invalidationCollections = new Dictionary<string, List<string>>();

        uint dataWithUsefulSubmissionType = 0;
        uint dataSubmittedAfterSECReform = 0;
        uint dataToBeConsidered = 0;

        int amountSTier = 0;

        int amountATier = 0;

        int amountBTier = 0;

        int amountCTier = 0;

        int amountTTier = 0;

        internal void Merge(FullQualityReport report)
        {
            foreach(var invalidationCollection in report.invalidationCollections)
            {
                if (invalidationCollections.TryGetValue(invalidationCollection.Key, out var collection))
                    collection.AddRange(invalidationCollection.Value);
                else
                    invalidationCollections.Add(invalidationCollection.Key, invalidationCollection.Value);
            }

            dataWithUsefulSubmissionType += report.dataWithUsefulSubmissionType;
            dataSubmittedAfterSECReform += report.dataSubmittedAfterSECReform;
            dataToBeConsidered += report.dataToBeConsidered;

            amountSTier += report.amountSTier;
            amountATier += report.amountATier;
            amountBTier += report.amountBTier;
            amountCTier += report.amountCTier;
            amountTTier += report.amountTTier;
        }

        internal void ConsiderQualityReport(string fileName, QualityReport report)
        {
            foreach(var invalidation in report.Invalidations)
            {
                if(invalidationCollections.TryGetValue(invalidation, out var collection))
                    collection.Add(fileName);
                else
                {
                    var newCollection = new List<string>
                    {
                        fileName
                    };
                    invalidationCollections.Add(invalidation, newCollection);
                }
            }

            if (report.UsefulSubmissionType)
                dataWithUsefulSubmissionType++;

            if (report.SubmittedAfterSECReform)
                dataSubmittedAfterSECReform++;

            if(report.ShouldBeConsidered)
            {
                dataToBeConsidered++;
                switch (report.Quality)
                {
                    case QualityLevel.S_TIER: amountSTier++; break;
                    case QualityLevel.A_TIER: amountATier++; break;
                    case QualityLevel.B_TIER: amountBTier++; break;
                    case QualityLevel.C_TIER: amountCTier++; break;
                    case QualityLevel.T_TIER: amountTTier++; break;
                }
            }
        }

        internal void BuildAndSaveReport(string folderPath, uint amountEDGARFiles)
        {
            string report = "";
            report += $"Quality report {DateTime.Now}";
            report += Environment.NewLine;
            report += $"Analyzed {amountEDGARFiles} EDGAR files";
            report += Environment.NewLine;
            report += $"Files that are not SC 13D: {amountEDGARFiles - dataWithUsefulSubmissionType} ({GetFormattedPercentageString((int)(amountEDGARFiles - dataWithUsefulSubmissionType), dataToBeConsidered)}%)";
            report += Environment.NewLine;
            report += $"Files submitted before SEC reform: {amountEDGARFiles - dataSubmittedAfterSECReform} ({GetFormattedPercentageString((int)(amountEDGARFiles - dataSubmittedAfterSECReform), dataToBeConsidered)}%)";
            report += Environment.NewLine;
            report += "------------------------------------";
            report += Environment.NewLine;
            report += $"S-TIER data: {amountSTier} ({GetFormattedPercentageString(amountSTier, dataToBeConsidered)}%)";
            report += Environment.NewLine;
            report += $"A-TIER data: {amountATier} ({GetFormattedPercentageString(amountATier, dataToBeConsidered)}%)";
            report += Environment.NewLine;
            report += $"B-TIER data: {amountBTier} ({GetFormattedPercentageString(amountBTier, dataToBeConsidered)}%)";
            report += Environment.NewLine;
            report += $"C-TIER data: {amountCTier} ({GetFormattedPercentageString(amountCTier, dataToBeConsidered)}%)";
            report += Environment.NewLine;
            report += $"T-TIER data: {amountTTier} ({GetFormattedPercentageString(amountTTier, dataToBeConsidered)}%)";
            report += Environment.NewLine;
            report += "------------------------------------";
            report += Environment.NewLine;
            foreach (var invalidation in invalidationCollections)
                report += BuildInvalidationSection(invalidation.Key,invalidation.Value, amountEDGARFiles);

            string path = Path.Combine(folderPath, "qualityReport.txt");

            File.WriteAllText(path, report);
        }

        static string BuildInvalidationSection(string invalidationName, IEnumerable<string> invalidations, uint amountEDGARFiles)
        {
            var section = "";
            var amountOfInvalidations = invalidations.Count();
            section += $"Files with invalid field {invalidationName}: {amountOfInvalidations} ({GetFormattedPercentageString(amountOfInvalidations, amountEDGARFiles)}%). ";
            if (invalidations.Any())
            {
                var randMax = amountOfInvalidations == 1 ? 1 : amountOfInvalidations - 1;
                section += $"Suggestion: Investigate file {Path.GetFileName(invalidations.ElementAt(new Random().Next(0, randMax)))}";
            }
            section += Environment.NewLine;
            return section;
        }

        static string GetFormattedPercentageString(int dividend, uint divisor)
        {
            return string.Format("{0:0.00}", (double)dividend / divisor * 100);
        }
    }
}
