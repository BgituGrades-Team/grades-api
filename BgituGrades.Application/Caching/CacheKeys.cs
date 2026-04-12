namespace BgituGrades.Application.Caching
{
    public static class CacheKeys
    {
        public static string Work(int id) => $"work:{id}";
        public static string WorkAll() => $"work:all";
        public static string KeyByLookUpHash(string lookUpHash) => $"key:{lookUpHash}";
        public static string KeyAll() => $"key:all";
        public static string Group(int id) => $"group:{id}";
        public static string GroupByPeriod(int year, int semester) => $"group:period:{year}:{semester}";
        public static string GroupAll() => $"group:all";
    }
}
