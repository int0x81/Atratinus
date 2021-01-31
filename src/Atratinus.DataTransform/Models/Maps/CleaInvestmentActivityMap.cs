using Atratinus.Core.Models;
using CsvHelper.Configuration;

namespace Atratinus.DataTransform.Models.Maps
{
    public class CleaInvestmentActivityMap : ClassMap<InvestmentActivity>
    {
        public CleaInvestmentActivityMap()
        {
            Map(a => a.AccessionNumber).Name("ACCESSION_NUMBER");
            Map(a => a.ActivistInvestorCIK).Name("ACTIVIST_INVESTOR_CIK");
            Map(a => a.ActivistInvestorCity).Name("ACTIVIST_INVESTOR_CITY");
            Map(a => a.ActivistInvestorFiscalYearEnd).Name("ACTIVIST_INVESTOR_FISCAL_YEAR_END");
            Map(a => a.ActivistInvestorIRSNumber).Name("ACTIVIST_INVESTOR_IRS_NUMBER");
            Map(a => a.ActivistInvestorName).Name("ACTIVIST_INVESTOR_COMPANY_CONFORMED_NAME");
            Map(a => a.ActivistInvestorSIC).Name("ACTIVIST_INVESTOR_SIC");
            Map(a => a.ActivistInvestorStateOfIncorporation).Name("ACTIVIST_INVESTOR_STATE_OF_INCORPORATION");
            Map(a => a.ClassOfSecurities).Name("CLASS_OF_SECURITIES");
            Map(a => a.CUSIP).Name("CUSIP");
            Map(a => a.EventDate).Name("EVENT_DATE");
            Map(a => a.FilingDate).Name("FILING_DATE");
            Map(a => a.PublicDocumentCount).Name("PUBLIC_DOCUMENT_COUNT");
            Map(a => a.Source).Name("SOURCE");
            Map(a => a.SubjectCompanyCIK).Name("SUBJECT_COMPANY_CIK");
            Map(a => a.SubjectCompanyCity).Name("SUBJECT_COMPANY_CITY");
            Map(a => a.SubjectCompanyCUSIP).Name("SUBJECT_COMPANY_CUSIP");
            Map(a => a.SubjectCompanyFiscalYearEnd).Name("SUBJECT_COMPANY_FISCAL_YEAR_END");
            Map(a => a.SubjectCompanyIRSNumber).Name("SUBJECT_COMPANY_IRS_NUMBER");
            Map(a => a.SubjectCompanyName).Name("SUBJECT_COMPANY_COMPANY_CONFORMED_NAME");
            Map(a => a.SubjectCompanySIC).Name("SUBJECT_COMPANY_SIC");
            Map(a => a.SubjectCompanyStateOfIncorporation).Name("SUBJECT_COMPANY_STATE_OF_INCORPORATION");
            Map(a => a.SubmissionType).Name("SUBMISSION_TYPE");
        }
    }
}
