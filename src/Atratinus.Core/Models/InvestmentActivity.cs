using Atratinus.Core.Enums;

namespace Atratinus.Core.Models
{
    public class InvestmentActivity
    {
        public string AccessionNumber { get; set; }

        public string ActivistInvestorCIK { get; set; }

        public string ActivistInvestorCity { get; set; }

        public string ActivistInvestorName { get; set; }

        public string ActivistInvestorFiscalYearEnd { get; set; }

        public string ActivistInvestorIRSNumber { get; set; }

        public string ActivistInvestorSIC { get; set; }

        public string ActivistInvestorStateOfIncorporation { get; set; }

        public string ClassOfSecurities { get; set; }

        public string CUSIP { get; set; }

        public string EventDate { get; set; }

        public string FilingDate { get; set; }

        public string PublicDocumentCount { get; set; }

        public string SubjectCompanyCIK { get; set; }

        public string SubjectCompanyCity { get; set; }

        public string SubjectCompanyName { get; set; }

        public string SubjectCompanyCUSIP { get; set; }

        public string SubjectCompanyFiscalYearEnd { get; set; }

        public string SubjectCompanyIRSNumber { get; set; }

        public string SubjectCompanySIC { get; set; }

        public string SubjectCompanyStateOfIncorporation { get; set; }

        public SubmissionType? SubmissionType { get; set; }

        public string Source { get; set; }

        public TypeOfReportingPerson? TypeOfReportingPerson { get; set; }

        public string ActivistInvestorId { get; set; }

        public string ActivistInvestorFirmType { get; set; }

        public string ActivistInvestorFundType { get; set; }

        public string PurposeOfTransaction { get; set; }

        public int PurposeOfTransactionTypeId { get; set; }

        public string PurposeOfTransactionType { get; set; }

        public bool PurposeAnalyzedByML { get; set; }

        /// <summary>
        /// Meta data that states the quality of this datum.
        /// </summary>
        public string DataQualityLevel { get; set; }
    }
}
