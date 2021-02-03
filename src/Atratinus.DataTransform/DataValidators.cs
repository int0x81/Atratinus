using System.Text.RegularExpressions;

namespace Atratinus.DataTransform
{
    static class DataValidators
    {
        internal static bool IsValidAccessionNumber(string text)
        {
            string pattern = @"^[0-9]{10}-[0-9]{2}-[0-9]{6}$";
            return !string.IsNullOrEmpty(text) && Regex.IsMatch(text, pattern);
        }

        internal static bool IsValidPurposeOfTransaction(string text)
        {
            return !string.IsNullOrWhiteSpace(text);
        }

        internal static bool IsValidFilingDate(string text)
        {
            string pattern = "^[0-9]{8}$";
            return !string.IsNullOrEmpty(text) && Regex.IsMatch(text, pattern);
        }
    }
}
