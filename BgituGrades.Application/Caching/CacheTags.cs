namespace BgituGrades.Application.Caching
{
    public class CacheTags
    {
        public static string Work() => "work";
        public static string Key() => "key";
        public static string Group() => "group";
        public static string Report() => "report";
        public static string Disicpline() => "discipline";
        public static string Class() => "class";
        public static List<string> WorkAll() => ["work", All()];
        public static List<string> KeyAll() => ["key", All()];
        public static List<string> GroupAll() => ["group", All()];
        public static List<string> ReportAll() => ["report", All()];
        public static List<string> DisicplineAll() => ["discipline", All()];
        public static List<string> ClassAll() => ["class", All()];
        public static string All() => "all";
    }
}
