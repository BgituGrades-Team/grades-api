using BgituGrades.Domain.Enums;
using BgituGrades.Application.DTOs;

namespace BgituGrades.Application.Interfaces
{
    public interface IKeyService
    {
        Task<List<KeyDTO>> GetAllKeysAsync(CancellationToken cancellationToken);
        Task<KeyDTO> GetKeyAsync(string key, CancellationToken cancellationToken);
        Task<KeyDTO> GenerateKeyAsync(Role role, int? groupId, CancellationToken cancellationToken);
        Task<bool> DeleteKeyAsync(string key, CancellationToken cancellationToken);
    }
}
