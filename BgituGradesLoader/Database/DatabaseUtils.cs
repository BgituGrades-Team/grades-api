using System.Text.RegularExpressions;

namespace BgituGradesLoader.Database
{
    public static partial class DatabaseUtils
    {
        private const string LECTURE_VALUE = "LECTURE";
        private const string PRACTICE_VALUE = "PRACTICE";
        private const string EXTRA_SYMBOLS = " .,-";

        [GeneratedRegex(@"\s+", RegexOptions.Compiled)]
        private static partial Regex NormalizeSpacesRegex();

        [GeneratedRegex(@"[\s.,-]+", RegexOptions.Compiled)]
        private static partial Regex NormalizeFilterRegex();

        public static string GetPairType(bool isLecture)
        {
            if (isLecture)
                return LECTURE_VALUE;
            return PRACTICE_VALUE;
        }

        public static string NormalizeDisciplineForDatabase(string? disciplineName)
        {
            if (string.IsNullOrEmpty(disciplineName))
                return string.Empty;
            disciplineName = NormalizeSpacesRegex().Replace(disciplineName.Trim(), " ");

            disciplineName = disciplineName.Trim(' ', ',');

            disciplineName = Regex.Replace(disciplineName, @"\s*-\s*", "-");
            return disciplineName;
        }

        public static string NormalizeDisciplineForFiltering(this string? disciplineName)
        {
            if (string.IsNullOrEmpty(disciplineName))
                return string.Empty;
            return NormalizeFilterRegex().Replace(disciplineName, "").ToLower();
        }

        public static int CountExtraSymbols(this string? disciplineName)
        {
            int count = 0;
            if (disciplineName == null)
                return count;

            foreach (char symbol in disciplineName)
                if (EXTRA_SYMBOLS.Contains(symbol))
                    count++;
            return count;
        }
    }
}
