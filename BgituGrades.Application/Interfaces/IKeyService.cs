using BgituGrades.Domain.Enums;
using BgituGrades.Application.Models.Key;

namespace BgituGrades.Application.Interfaces
{
    public interface IKeyService
    {
        Task<List<KeyResponse>> GetKeysAsync(CancellationToken cancellationToken);
        Task<KeyResponse> GetKeyAsync(string key, CancellationToken cancellationToken);
        Task<KeyResponse> GenerateKeyAsync(Role role, int? groupId, CancellationToken cancellationToken);
        Task<bool> DeleteKeyAsync(string key, CancellationToken cancellationToken);
    }
}
