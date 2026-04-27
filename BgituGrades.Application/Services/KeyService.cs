using AutoMapper;
using BgituGrades.Application.Caching;
using BgituGrades.Application.DTOs;
using BgituGrades.Application.Interfaces;
using BgituGrades.Domain.Entities;
using BgituGrades.Domain.Enums;
using BgituGrades.Domain.Interfaces;
using Microsoft.Extensions.Caching.Hybrid;
using System.Security.Cryptography;

namespace BgituGrades.Application.Services
{
    public class KeyService(IKeyRepository keyRepository, ITokenHasher hasher, IMapper mapper, ICacheService cacheService) : IKeyService
    {
        private readonly IKeyRepository _keyRepository = keyRepository;
        private readonly IMapper _mapper = mapper;
        private readonly ITokenHasher _hasher = hasher;
        private readonly ICacheService _cacheService = cacheService;

        private static readonly HybridCacheEntryOptions DefaultOptions = new()
        {
            Expiration = TimeSpan.FromMinutes(10),
            LocalCacheExpiration = TimeSpan.FromMinutes(5)
        };
        public async Task<KeyDTO> GenerateKeyAsync(Role role, int? groupId, CancellationToken cancellationToken)
        {
            var newKey = RandomNumberGenerator.GetHexString(64, true);

            var apiKey = new ApiKey
            {
                Key = newKey,
                LookupHash = _hasher.ComputeLookupHash(newKey),
                StoredHash = _hasher.Hash(newKey),
                OwnerName = "bgitugrades",
                Role = role.ToString(),
                GroupId = groupId,
                ExpiryDate = role == Role.STUDENT ? DateTime.UtcNow.AddMonths(3) : null
            };

            var createdKey = await _keyRepository.CreateKeyAsync(apiKey, cancellationToken: cancellationToken);
            var keyDto = _mapper.Map<KeyDTO>(createdKey);
            await _cacheService.RemoveByTagAsync(CacheTags.Key(), ct: cancellationToken);
            return keyDto;
        }

        public async Task<bool> DeleteKeyAsync(string key, CancellationToken cancellationToken)
        {
            var lookupHash = _hasher.ComputeLookupHash(key);
            var result = await _keyRepository.DeleteKeyAsync(lookupHash, cancellationToken: cancellationToken);
            if (result)
                await _cacheService.RemoveByTagAsync(CacheTags.Key(), ct: cancellationToken);
            return result;
        }

        public async Task<List<KeyDTO>> GetAllKeysAsync(CancellationToken cancellationToken)
        {
            return await _cacheService.GetOrCreateAsync(
                key: CacheKeys.KeyAll(),
                factory: async token =>
                {
                    var entities = await _keyRepository.GetKeysAsync(cancellationToken: token);
                    return _mapper.Map<List<KeyDTO>>(entities);
                },
                tags: [CacheTags.Key()],
                options: DefaultOptions, ct: cancellationToken);
        }

        public async Task<KeyDTO> GetKeyAsync(string key, CancellationToken cancellationToken)
        {
            var lookupHash = _hasher.ComputeLookupHash(key);
            return await _cacheService.GetOrCreateAsync(
                key: CacheKeys.KeyByLookUpHash(lookupHash),
                factory: async token =>
                {
                    var entity = await _keyRepository.GetAsync(key, cancellationToken: token);
                    return _mapper.Map<KeyDTO>(entity);
                },
                tags: [CacheTags.Key()],
                options: DefaultOptions, ct: cancellationToken);
        }
    }
}
