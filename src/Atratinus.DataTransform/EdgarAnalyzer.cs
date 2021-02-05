using Atratinus.Core.Models;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Atratinus.DataTransform
{
    class EdgarAnalyzer
    {
        const string ITEM_PATTERN = @"^\s*(Item|item|ITEM)";
        const string ITEM_04_HEADER_PATTERN = @"(Item|item|ITEM)\s+4([.]|[:])?\s+(Purpose|PURPOSE|purpose)\s+(Of|OF|of)\s+((The|THE|the)\s+)?(Transaction|TRANSACTION|transaction)[.]?";
        const string ITEM_05_HEADER_PATTERN = @"(Item|item|ITEM)\s+5([.]|[:])?\s+(Interest|INTEREST|interest)";

        internal static InvestmentActivity Analyze(string file)
        {
            var activity = new InvestmentActivity
            {
                AccessionNumber = Path.GetFileNameWithoutExtension(file)
            };

            using StreamReader reader = new StreamReader(file, System.Text.Encoding.UTF8);

            string fileContent = reader.ReadToEnd();
            string beautified = Prepare(fileContent);

            activity.PurposeOfTransaction = GetPurposeOfTransaction(beautified, file);

            return activity;
        }

        static string GetPurposeOfTransaction(string fileContent, string fileName)
        {
            int[] start = GetStartOfPurposeOfTransaction(fileContent, fileName);
         
            int[] end = GetEndOfPurposeOfTransaction(fileContent, fileName);

            if (start.Length != end.Length || start.Length == 0)
                return null;

            for(int c = 0; c < start.Length; c++)
            {
                if (start[c] >= end[c])
                    continue;

                var ugly = fileContent[start[c]..end[c]];

                if (string.IsNullOrWhiteSpace(ugly))
                    continue;

                return Beautify(ugly);
            }

            return null;
        }

        private static int[] GetStartOfPurposeOfTransaction(string fileContent, string fileName)
        {
            var matches = Regex.Matches(fileContent, ITEM_04_HEADER_PATTERN);

            int[] startIndices = new int[matches.Count];

            for (int c = 0; c < matches.Count; c++)
                startIndices[c] = matches[c].Index + matches[c].Length;

            return startIndices;
        }

        private static int[] GetEndOfPurposeOfTransaction(string fileContent, string fileName)
        {
            var matches = Regex.Matches(fileContent, ITEM_05_HEADER_PATTERN);

            int[] endIndices = new int[matches.Count];

            for (int c = 0; c < matches.Count; c++)
                endIndices[c] = matches[c].Index;

            return endIndices;
        }

        static string Beautify(string ugly)
        {
            ugly = RemovePageTags(ugly);

            ugly = RemovePagination(ugly);

            ugly = RemoveHyphenSeries(ugly);

            return ugly.Trim();
        }

        static string RemoveHyphenSeries(string ugly)
        {
            string pattern = @"-{4,}";

            return Regex.Replace(ugly, pattern, string.Empty);
        }

        static string RemovePagination(string ugly)
        {
            string pattern = @"(Page|PAGE|page)\s+[0-9]+\s+(of|OF|Of)";

            return Regex.Replace(ugly, pattern, string.Empty);
        }

        static string RemovePageTags(string ugly)
        {
            string pattern = @"<(Page|PAGE|page)(>|\/>)";

            return Regex.Replace(ugly, pattern, string.Empty);
        }

        static string Prepare(string ugly)
        {
            string beautiful = ReplaceNoBreakSpaces(ugly);
            beautiful = ShrinkWhitespace(beautiful);
            beautiful = ReplaceNewLines(beautiful);
            return beautiful;
        }

        static string ReplaceNewLines(string ugly)
        {
            string pattern = @"\\n";
            return Regex.Replace(ugly, pattern, " ").Replace(@"\n", " ").Replace(Environment.NewLine, " ");
        }

        static string ReplaceNoBreakSpaces(string ugly)
        {
            string pattern = @"[\u00A0]+";
            return Regex.Replace(ugly, pattern, " ");
        }

        static string ShrinkWhitespace(string ugly)
        {
            string pattern = @"\s{2,}";

            return Regex.Replace(ugly, pattern, " ");
        }
    }
}
