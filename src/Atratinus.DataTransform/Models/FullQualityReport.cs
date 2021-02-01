using Atratinus.DataTransform.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Atratinus.DataTransform.Models
{
    public class FullQualityReport
    {
        readonly List<string> filesWithInvalidAccessionNumber = new List<string>();

        readonly List<string> filesWithInvalidPurposeOfTransaction = new List<string>();

        readonly List<string> filesWithInvalidFilingDate = new List<string>();

        int amountSTier = 0;

        int amountATier = 0;

        int amountBTier = 0;

        int amountCTier = 0;

        int amountTTier = 0;

        internal void Merge(FullQualityReport report)
        {
            filesWithInvalidPurposeOfTransaction.AddRange(report.filesWithInvalidPurposeOfTransaction);
            filesWithInvalidFilingDate.AddRange(report.filesWithInvalidFilingDate);

            amountSTier += report.amountSTier;
            amountATier += report.amountATier;
            amountBTier += report.amountBTier;
            amountCTier += report.amountCTier;
            amountTTier += report.amountTTier;
        }

        internal void ConsiderQualityReport(string fileName, QualityReport report)
        {
            if (report.InvalidAccessionNumber)
                filesWithInvalidAccessionNumber.Add(fileName);

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
                case QualityLevel.T_TIER: amountTTier++; break;
            }
        }

        internal void BuildAndSaveReport(string folderPath, uint amountEDGARFiles)
        {
            string report = "";
            report += $"Quality report {DateTime.Now}";
            report += Environment.NewLine;
            report += $"Analyzed {amountEDGARFiles} EDGAR files";
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
            report += $"T-TIER data: {amountTTier} ({GetFormattedPercentageString(amountTTier, amountEDGARFiles)}%)";
            report += Environment.NewLine;
            report += "------------------------------------";
            report += Environment.NewLine;
            report += $"Files with invalid accession number: {filesWithInvalidAccessionNumber.Count} ({GetFormattedPercentageString(filesWithInvalidAccessionNumber.Count, amountEDGARFiles)}%). ";
            if(filesWithInvalidAccessionNumber.Count > 0)
            {
                var randMax = filesWithInvalidAccessionNumber.Count == 1 ? 1 : filesWithInvalidAccessionNumber.Count - 1;
                report += $"Suggestion: Investigate file {Path.GetFileName(filesWithInvalidAccessionNumber.ElementAt(new Random().Next(0, randMax)))}";
            }
            report += Environment.NewLine;
            report += $"Files with invalid purpose of transaction: {filesWithInvalidPurposeOfTransaction.Count} ({GetFormattedPercentageString(filesWithInvalidPurposeOfTransaction.Count, amountEDGARFiles)}%). ";
            if(filesWithInvalidPurposeOfTransaction.Count > 0)
            {
                var randMax = filesWithInvalidPurposeOfTransaction.Count == 1 ? 1 : filesWithInvalidPurposeOfTransaction.Count - 1;
                report += $"Suggestion: Investigate file {Path.GetFileName(filesWithInvalidPurposeOfTransaction.ElementAt(new Random().Next(0, randMax)))}";
            }
            report += Environment.NewLine;
            report += $"Files with invalid filed date: {filesWithInvalidFilingDate.Count} ({GetFormattedPercentageString(filesWithInvalidFilingDate.Count, amountEDGARFiles)}%). ";
            if(filesWithInvalidFilingDate.Count > 0)
            {
                var randMax = filesWithInvalidFilingDate.Count == 1 ? 1 : filesWithInvalidFilingDate.Count - 1;
                report += $"Suggestion: Investigate file {Path.GetFileName(filesWithInvalidFilingDate.ElementAt(new Random().Next(0, randMax)))}";
            }
            report += Environment.NewLine;

            string path = Path.Combine(folderPath, "qualityReport.txt");

            File.WriteAllText(path, report);
        }

        static string GetFormattedPercentageString(int dividend, uint divisor)
        {
            return string.Format("{0:0.00}", (double)dividend / divisor * 100);
        }
    }
}
