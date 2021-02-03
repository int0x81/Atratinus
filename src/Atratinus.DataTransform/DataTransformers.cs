using Atratinus.Core.Enums;
using System.Text.RegularExpressions;

namespace Atratinus.DataTransform
{
    static class DataTransformers
    {
        internal static SubmissionType? TransformSubmissionType(string input)
        {
            if (string.IsNullOrEmpty(input))
                return null;

            string SC13D_pattern = @"^\s*SC(\s*|_)13D\s*$";
            string SC13DA_pattern = @"^\s*SC(\s*|_)13D[\/]{0,1}A\s*$";

            if (Regex.IsMatch(input, SC13D_pattern))
                return SubmissionType.SC_13D;
            else if (Regex.IsMatch(input, SC13DA_pattern))
                return SubmissionType.SC_13DA;
            else
                return SubmissionType.OTHER;
        }

        internal static TypeOfReportingPerson? TransformTypeOfReportingPerson(string input)
        {
            if (string.IsNullOrEmpty(input) || input.Length != 2)
                return null;

            return (input.ToUpper()) switch
            {
                "BD" => TypeOfReportingPerson.BROKER_DEALER,
                "BK" => TypeOfReportingPerson.BANK,
                "IC" => TypeOfReportingPerson.INCURANCE_COMPANY,
                "IV" => TypeOfReportingPerson.INVESTMENT_COMPANY,
                "IA" => TypeOfReportingPerson.INVESTMENT_ADVISOR,
                "EP" => TypeOfReportingPerson.EMPLOYEE_BENEFIT_PLAN_OR_ENDOWMENT_FUND,
                "HC" => TypeOfReportingPerson.PARENT_HOLDING_COMPANY_CONTROL_PERSON,
                "SA" => TypeOfReportingPerson.SAVINGS_ASSOCIATION,
                "CP" => TypeOfReportingPerson.CHURCH_PLAN,
                "CO" => TypeOfReportingPerson.CORPERATION,
                "PN" => TypeOfReportingPerson.PARTNERSHIP,
                "IN" => TypeOfReportingPerson.INDIVIDUAL,
                "OO" => TypeOfReportingPerson.OTHER,
                _ => null,
            };
        }
    }
}
