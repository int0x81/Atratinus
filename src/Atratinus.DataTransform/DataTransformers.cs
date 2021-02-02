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
    }
}
