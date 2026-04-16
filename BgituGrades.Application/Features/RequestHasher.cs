using BgituGrades.Application.Models.Report;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace BgituGrades.Application.Features
{
    public class RequestHasher
    {
        public static string ComputeRequestCacheKey(ReportRequest request)
        {
            var normalized = new
            {
                request.ReportType,
                request.Host,
                GroupIds = request.GroupIds?.OrderBy(x => x).ToArray(),
                DisciplineIds = request.DisciplineIds?.OrderBy(x => x).ToArray(),
                StudentIds = request.StudentIds?.OrderBy(x => x).ToArray(),
            };

            var json = JsonSerializer.Serialize(normalized);
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(json));
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }
    }
}
