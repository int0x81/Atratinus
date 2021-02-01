using Atratinus.Core.Models;
using Atratinus.DataTransform.Enums;
using System.Text.RegularExpressions;
using Atratinus.DataTransform.Models;

namespace Atratinus.DataTransform
{
    static class QualityGate
    {
        internal static QualityReport Measure(InvestmentActivity investment)
        {
            var report = new QualityReport();
            ushort badStates = 0;

            if(!IsAccesionNumberCorrect(investment.AccessionNumber))
            {
                report.InvalidAccessionNumber = true;
                report.Quality = QualityLevel.T_TIER;
                investment.DataQualityLevel = QualityLevel.T_TIER.ToString();
            }

            if (!IsFilingDateCorrect(investment.FilingDate))
            {
                report.InvalidFilingDate = true;
                report.Quality = QualityLevel.T_TIER;
                investment.DataQualityLevel = QualityLevel.T_TIER.ToString();
            }

            if (report.Quality == QualityLevel.T_TIER)
                return report;

            if (!IsPurposeOfTransactionCorrect(investment.PurposeOfTransaction))
            {
                badStates++;
                report.InvalidPurposeOfTransaction = true;
            }
                
            switch (badStates)
            {
                case 0:
                    {
                        investment.DataQualityLevel = QualityLevel.S_TIER.ToString();
                        break;
                    }
                case 1:
                    {
                        investment.DataQualityLevel = QualityLevel.A_TIER.ToString();
                        break;
                    }
                case 2:
                    {
                        investment.DataQualityLevel = QualityLevel.B_TIER.ToString();
                        break;
                    }
                case 3:
                    {
                        investment.DataQualityLevel = QualityLevel.C_TIER.ToString();
                        break;
                    }
                default:
                    {
                        investment.DataQualityLevel = QualityLevel.T_TIER.ToString();
                        break;
                    }
            }

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
    }
}
