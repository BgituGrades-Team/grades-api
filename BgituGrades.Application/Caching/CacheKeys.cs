namespace BgituGrades.Application.Caching
{
    public static class CacheKeys
    {
        public static string Work(int id) => $"work:{id}";
        public static string WorkAll() => $"work:all";
        public static string KeyByLookUpHash(string lookUpHash) => $"key:{lookUpHash}";
        public static string KeyVerified(string lookUpHash) => $"key:verified:{lookUpHash}";
        public static string KeyAll() => $"key:all";
        public static string Group(int id) => $"group:{id}";
        public static string GroupByPeriod(int year, int semester) => $"group:period:{year}:{semester}";
        public static string GroupAll() => $"group:all";
        public static string ReportByRequestHash(string hash) => $"report:hash:{hash}";
        public static string DisicplineAll() => "discipline:all";
        public static string DisciplineByGroup(int groupId) => $"discipline:group:{groupId}";
    }
}
