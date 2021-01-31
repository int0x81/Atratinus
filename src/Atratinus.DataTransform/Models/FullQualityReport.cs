using Atratinus.DataTransform.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Atratinus.DataTransform.Models
{
    public class FullQualityReport
    {
        readonly List<string> filesWithInvalidPurposeOfTransaction = new List<string>();

        readonly List<string> filesWithInvalidFilingDate = new List<string>();

        int amountSTier = 0;

        int amountATier = 0;

        int amountBTier = 0;

        int amountCTier = 0;

        internal void Merge(FullQualityReport report)
        {
            filesWithInvalidPurposeOfTransaction.AddRange(report.filesWithInvalidPurposeOfTransaction);
            filesWithInvalidFilingDate.AddRange(report.filesWithInvalidFilingDate);

            amountSTier += report.amountSTier;
            amountATier += report.amountATier;
            amountBTier += report.amountBTier;
            amountCTier += report.amountCTier;
        }

        internal void ConsiderQualityReport(string fileName, QualityReport report)
        {
            if (report.InvalidPurposeOfTransaction)
                filesWithInvalidPurposeOfTransaction.Add(fileName);

            if (report.InvalidFilingDate)
                filesWithInvalidFilingDate.Add(fileName);

            switch(report.Quality)
            {
                case QualityLevel.S_TIER: amountSTier++; break;
                case QualityLevel.A_TIER: amountATier++; break;
                case QualityLevel.B_TIER: amountBTier++; break;
                case QualityLevel.C_TIER: amountCTier++; break;
            }
        }

        internal void BuildAndSaveReport(string folderPath, uint amountEDGARFiles)
        {
            string report = "";
            report += $"Quality report {DateTime.Now}";
            report += Environment.NewLine;
            report += "------------------------------------";
            report += Environment.NewLine;
            report += $"S-TIER data: {amountSTier} ({GetFormattedPercentageString(amountSTier, amountEDGARFiles)}%)";
            report += Environment.NewLine;
            report += $"A-TIER data: {amountATier} ({GetFormattedPercentageString(amountATier, amountEDGARFiles)}%)";
            report += Environment.NewLine;
            report += $"B-TIER data: {amountBTier} ({GetFormattedPercentageString(amountBTier, amountEDGARFiles)}%)";
            report += Environment.NewLine;
            report += $"C-TIER data: {amountCTier} ({GetFormattedPercentageString(amountCTier, amountEDGARFiles)}%)";
            report += Environment.NewLine;
            report += "------------------------------------";
            report += Environment.NewLine;
            report += $"Files with invalid purpose of transaction: {filesWithInvalidPurposeOfTransaction.Count} ({GetFormattedPercentageString(filesWithInvalidPurposeOfTransaction.Count, amountEDGARFiles)}%). " +
                $"Suggestion: Investigate file {Path.GetFileName(filesWithInvalidPurposeOfTransaction.ElementAt(new Random().Next(0, filesWithInvalidPurposeOfTransaction.Count - 1)))}";
            report += Environment.NewLine;
            report += $"Files with invalid filed date: {filesWithInvalidFilingDate.Count} ({GetFormattedPercentageString(filesWithInvalidFilingDate.Count, amountEDGARFiles)}%). " +
                $"Suggestion: Investigate file {Path.GetFileName(filesWithInvalidFilingDate.ElementAt(new Random().Next(0, filesWithInvalidFilingDate.Count - 1)))}";

            string path = Path.Combine(folderPath, "qualityReport.txt");

            File.WriteAllText(path, report);
        }

        static string GetFormattedPercentageString(int dividend, uint divisor)
        {
            return string.Format("{0:0.00}", ((double)dividend / divisor * 100));
        }
    }
}
