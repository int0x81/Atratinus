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
                return report;
            }

            if (!IsPurposeOfTransactionCorrect(investment.PurposeOfTransaction))
            {
                badStates++;
                report.InvalidPurposeOfTransaction = true;
            }   

            if (!IsFilingDateCorrect(investment.FilingDate))
            {
                badStates++;
                report.InvalidFilingDate = true;
            }
                

            QualityLevel quality;
            switch (badStates)
            {
                case 0:
                    {
                        quality = QualityLevel.S_TIER;
                        break;
                    }
                case 1:
                    {
                        quality = QualityLevel.A_TIER;
                        break;
                    }
                case 2:
                    {
                        quality = QualityLevel.B_TIER;
                        break;
                    }
                case 3:
                    {
                        quality = QualityLevel.C_TIER;
                        break;
                    }
                default:
                    {
                        quality = QualityLevel.T_TIER;
                        break;
                    }
            }

            investment.DataQualityLevel = quality.ToString();
            report.Quality = quality;
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
