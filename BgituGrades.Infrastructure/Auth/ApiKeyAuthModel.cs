using AspNetCore.Authentication.ApiKey;
using BgituGrades.Domain.Entities;
using System.Security.Claims;

namespace BgituGrades.Infrastructure.Auth
{
    public class ApiKeyAuthModel(ApiKey apiKey) : IApiKey
    {
        private readonly ApiKey _apiKey = apiKey;

        public string Key => _apiKey.Key;
        public string OwnerName => _apiKey.OwnerName;

        public IReadOnlyCollection<Claim> Claims
        {
            get
            {
                var claims = new List<Claim> { new(ClaimTypes.Role, _apiKey.Role ?? "STUDENT") };
                if (_apiKey.GroupId.HasValue)
                    claims.Add(new Claim("group_id", _apiKey.GroupId.Value.ToString()));
                return claims.AsReadOnly();
            }
        }
    }
}
