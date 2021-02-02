using Atratinus.Core.Models;
using Atratinus.DataTransform.Enums;
using System.Text.RegularExpressions;
using Atratinus.DataTransform.Models;
using System;

namespace Atratinus.DataTransform
{
    static class QualityGate
    {
        internal static QualityReport Measure(InvestmentActivity investment, AtratinusConfiguration config)
        {
            var report = new QualityReport();
            ushort badStates = 0;

            if(!IsAccesionNumberCorrect(investment.AccessionNumber))
            {
                report.Invalidations.Add("AccessionNumber");
                report.Quality = QualityLevel.T_TIER;
                investment.DataQualityLevel = QualityLevel.T_TIER.ToString();
            }

            if (!IsFilingDateCorrect(investment.FilingDate))
            {
                badStates++;
                report.Invalidations.Add("FilingDate");
            }
            else
            {
                var year = Convert.ToInt32(investment.FilingDate.Substring(0, 4));
                var month = Convert.ToInt32(investment.FilingDate.Substring(4, 2));
                var day = Convert.ToInt32(investment.FilingDate.Substring(6, 2));

                if (new DateTime(year, month, day) < new DateTime(1998, 2, 17))
                    report.SubmittedAfterSECReform = false;
                else
                    report.SubmittedAfterSECReform = true;
            }

            if (!IsPurposeOfTransactionCorrect(investment.PurposeOfTransaction))
            {
                badStates++;
                report.Invalidations.Add("PurposeOfTransaction");
            }

            if(!investment.SubmissionType.HasValue)
            {
                badStates++;
                report.Invalidations.Add("SubmissionType");
                report.UsefulSubmissionType = false;
            }
            else
            {
                if (investment.SubmissionType.Value == Core.Enums.SubmissionType.SC_13D)
                    report.UsefulSubmissionType = true;
                else
                    report.UsefulSubmissionType = false;
            }

            if (report.Quality == QualityLevel.T_TIER)
                return report;

            switch (badStates)
            {
                case 0:
                    {
                        investment.DataQualityLevel = QualityLevel.S_TIER.ToString();
                        report.Quality = QualityLevel.S_TIER;
                        break;
                    }
                case 1:
                    {
                        investment.DataQualityLevel = QualityLevel.A_TIER.ToString();
                        report.Quality = QualityLevel.A_TIER;
                        break;
                    }
                case 2:
                    {
                        investment.DataQualityLevel = QualityLevel.B_TIER.ToString();
                        report.Quality = QualityLevel.B_TIER;
                        break;
                    }
                case 3:
                    {
                        investment.DataQualityLevel = QualityLevel.C_TIER.ToString();
                        report.Quality = QualityLevel.C_TIER;
                        break;
                    }
                default:
                    {
                        investment.DataQualityLevel = QualityLevel.T_TIER.ToString();
                        report.Quality = QualityLevel.T_TIER;
                        break;
                    }
            }

            report.ShouldBeConsidered = TakeInvestment(report, config);
            return report;
        }

        private static bool IsAccesionNumberCorrect(string accessionNumber)
        {
            string pattern = @"^[0-9]{10}-[0-9]{2}-[0-9]{6}$";
            return !string.IsNullOrEmpty(accessionNumber) && Regex.IsMatch(accessionNumber, pattern);
        }

        static bool IsPurposeOfTransactionCorrect(string purpose)
        {
            return !string.IsNullOrEmpty(purpose);
        }

        static bool IsFilingDateCorrect(string filingDate)
        {
            string pattern = "^[0-9]{8}$";
            return !string.IsNullOrEmpty(filingDate) && Regex.IsMatch(filingDate, pattern);
        }

        static bool TakeInvestment(QualityReport report, AtratinusConfiguration config)
        {
            if (!config.TakeFilesBeforeSECReform && !report.SubmittedAfterSECReform)
                return false;

            if (report.Quality == QualityLevel.T_TIER)
                return false;

            if (!report.UsefulSubmissionType)
                return false;

            return true;
        }
    }
}
