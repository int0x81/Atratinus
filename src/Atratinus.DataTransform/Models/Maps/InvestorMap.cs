using Atratinus.Core.Models;
using CsvHelper.Configuration;

namespace Atratinus.DataTransform.Models.Maps
{
    public class InvestorMap : ClassMap<Investor>
    {
        public InvestorMap()
        {
            Map(a => a.Id).Index(0);
            Map(a => a.Name).Name("ACTIVIST_INVESTOR_COMPANY_CONFORMED_NAME");
            Map(a => a.FundName).Name("Fund Name");
            Map(a => a.SIC).Name("ACTIVIST_INVESTOR_SIC");
            Map(a => a.FirmType).Name("Firm Type");
            Map(a => a.FundType).Name("Fund Type");
        }
    }
}
