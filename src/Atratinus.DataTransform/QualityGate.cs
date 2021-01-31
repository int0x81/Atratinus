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

            if (!IsPurposeOfTransactionCorrect(investment.PurposeOfTransaction))
            {
                badStates++;
                report.InvalidPurposeOfTransaction = true;
            }   

            if (!IsFillingDateCorrect(investment.FilingDate))
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
                default:
                    {
                        quality = QualityLevel.C_TIER;
                        break;
                    }
            }

            investment.DataQualityLevel = quality.ToString();
            report.Quality = quality;
            return report;
        }

        static bool IsPurposeOfTransactionCorrect(string purpose)
        {
            return !string.IsNullOrEmpty(purpose);
        }

        static bool IsFillingDateCorrect(string filingDate)
        {
            string pattern = "^[0-9]{8}$";
            return !string.IsNullOrEmpty(filingDate) && Regex.IsMatch(filingDate, pattern);
        }
    }
}
