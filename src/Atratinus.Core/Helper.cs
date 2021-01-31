using Atratinus.Core.Models;
using CsvHelper;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;

namespace Atratinus.Core
{
    public static class Helper
    {
        public static IList<InvestmentActivity> ReadInAccessions(string folderPath)
        {
            var accessions = new List<InvestmentActivity>();

            var files = Directory.GetFiles(folderPath);

            foreach(var file in files)
            {
                using StreamReader reader = new StreamReader(file);
                string json = reader.ReadToEnd();
                var accession = JsonSerializer.Deserialize<InvestmentActivity>(json);
                accessions.Add(accession);
            }

            return accessions;
        }

        public static void SaveAccessionsAsCSV(IList<InvestmentActivity> accessions, string path, bool savePurpose)
        {
            using var writer = new StreamWriter(Path.Combine(path, "investmentActivities.csv"));
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            if(!savePurpose)
            {
                foreach(var accession in accessions)
                    accession.PurposeOfTransaction = null;
            }

            csv.WriteRecords(accessions);
        }

        public static void MergeAccession(InvestmentActivity accession, IReadOnlyDictionary<string, InvestmentActivity> secondary)
        {
            if(secondary.TryGetValue(accession.AccessionNumber, out var equivalent))
            {
                accession.ActivistInvestorId ??= equivalent.ActivistInvestorId;
                accession.ActivistInvestorCIK ??= equivalent.ActivistInvestorCIK;
                accession.ActivistInvestorCity ??= equivalent.ActivistInvestorCity;
                accession.ActivistInvestorFirmType ??= equivalent.ActivistInvestorFirmType;
                accession.ActivistInvestorFiscalYearEnd ??= equivalent.ActivistInvestorFiscalYearEnd;
                accession.ActivistInvestorFundType ??= equivalent.ActivistInvestorFundType;
                accession.ActivistInvestorIRSNumber ??= equivalent.ActivistInvestorIRSNumber;
                accession.ActivistInvestorName ??= equivalent.ActivistInvestorName;
                accession.ActivistInvestorSIC ??= equivalent.ActivistInvestorSIC;
                accession.ActivistInvestorStateOfIncorporation ??= equivalent.ActivistInvestorStateOfIncorporation;
                accession.ClassOfSecurities ??= equivalent.ClassOfSecurities;
                accession.CUSIP ??= equivalent.CUSIP;
                accession.EventDate ??= equivalent.EventDate;
                accession.FilingDate ??= equivalent.FilingDate;
                accession.PublicDocumentCount ??= equivalent.PublicDocumentCount;
                accession.PurposeOfTransaction ??= equivalent.PurposeOfTransaction;
                accession.PurposeOfTransactionType ??= equivalent.PurposeOfTransactionType;
                accession.Source ??= equivalent.Source;
                accession.SubjectCompanyCIK ??= equivalent.SubjectCompanyCIK;
                accession.SubjectCompanyCity ??= equivalent.SubjectCompanyCity;
                accession.SubjectCompanyCUSIP ??= equivalent.SubjectCompanyCUSIP;
                accession.SubjectCompanyFiscalYearEnd ??= equivalent.SubjectCompanyFiscalYearEnd;
                accession.SubjectCompanyIRSNumber ??= equivalent.SubjectCompanyIRSNumber;
                accession.SubjectCompanyName ??= equivalent.SubjectCompanyName;
                accession.SubjectCompanySIC ??= equivalent.SubjectCompanySIC;
                accession.SubjectCompanyStateOfIncorporation ??= equivalent.SubjectCompanyStateOfIncorporation;
                accession.SubmissionType ??= equivalent.SubmissionType;
            }
        }

        public static void SaveAccessionAsJSON(InvestmentActivity accession, string folder)
        {
            string json = JsonSerializer.Serialize(accession);

            string path = Path.Combine(folder, $"{accession.AccessionNumber}.json");

            File.WriteAllText(path, json);
        }
    }
}
