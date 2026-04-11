using AutoMapper;
using BgituGrades.Application.Features;
using BgituGrades.Application.Interfaces;
using BgituGrades.Application.Models.Key;
using BgituGrades.Domain.Entities;
using BgituGrades.Domain.Enums;
using BgituGrades.Domain.Interfaces;
using System.Security.Cryptography;

namespace BgituGrades.Application.Services
{
    
    public class KeyService(IKeyRepository keyRepository, ITokenHasher hasher, IMapper mapper) : IKeyService
    {
        private readonly IKeyRepository _keyRepository = keyRepository;
        private readonly IMapper _mapper = mapper;
        private readonly ITokenHasher _hasher = hasher;
        public async Task<KeyResponse> GenerateKeyAsync(Role role, int? groupId, CancellationToken cancellationToken)
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
                ExpiryDate = role == Role.STUDENT ? DateTime.UtcNow.AddDays(30) : null
            };

            var createdKey = await _keyRepository.CreateKeyAsync(apiKey, cancellationToken: cancellationToken);
            var response = _mapper.Map<KeyResponse>(createdKey);
            return response;
        }

        public async Task<bool> DeleteKeyAsync(string key, CancellationToken cancellationToken)
        {
            var lookupHash = _hasher.ComputeLookupHash(key);
            return await _keyRepository.DeleteKeyAsync(lookupHash, cancellationToken: cancellationToken);
        }

        public async Task<List<KeyResponse>> GetKeysAsync(CancellationToken cancellationToken)
        {
            var storedKeys = await _keyRepository.GetKeysAsync(cancellationToken: cancellationToken);
            var response = _mapper.Map<List<KeyResponse>>(storedKeys);
            return response;
        }

        public async Task<KeyResponse> GetKeyAsync(string key, CancellationToken cancellationToken)
        {
            var storedKey = await _keyRepository.GetAsync(key, cancellationToken: cancellationToken);
            var response = _mapper.Map<KeyResponse>(storedKey);
            return response;
        }
    }
}
