using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BgituGrades.Application.Caching
{
    public static class CacheKeys
    {
        public static string Work(int id) => $"work:{id}";
        public static string WorkAll() => $"work:all";
    }
}
