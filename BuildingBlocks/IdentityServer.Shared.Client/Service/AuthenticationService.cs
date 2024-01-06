using IdentityServer.Api.Protos;
using IdentityServer.Shared.Client.GrpcServices;
using IdentityServer.Shared.Client.Repositories.Interfaces;

namespace IdentityServer.Shared.Client.Service;

public class AuthenticationService : IAuthenticationService
    {
        private readonly AuthorizationGrpcServices _authorizationGrpcServices;
        private readonly IJwtCacheRepository _tokenCache;

        public AuthenticationService(AuthorizationGrpcServices authorizationGrpcServices, IJwtCacheRepository tokenCache)
        {
            _authorizationGrpcServices = authorizationGrpcServices ?? throw new ArgumentNullException(nameof(authorizationGrpcServices));
            _tokenCache = tokenCache ?? throw new ArgumentNullException(nameof(tokenCache));
        }

        public async Task<bool> IsAuthenticatedAsync(string? jwtToken, string? requiredRoles = null)
        {
            if (string.IsNullOrWhiteSpace(jwtToken))
            {
                return false;
            }
            
            var response = await _tokenCache.GetAsync<ValidateTokenResponse>(jwtToken);

            if (response == null)
            {
                response = await _authorizationGrpcServices.ValidateTokenAsync(jwtToken);
                if (response.Valid)
                {
                    await _tokenCache.SetAsync(jwtToken, response, TimeSpan.FromHours(3));
                }
            }

            return response?.Valid == true && (string.IsNullOrWhiteSpace(requiredRoles) || HasRequiredRoles(response.Roles, requiredRoles));
        }

        private bool HasRequiredRoles(string userRoles, string requiredRoles)
        {
            var roleList = userRoles.Split(",");
            var requiredRoleList = requiredRoles.Split("|");

            foreach (var requiredRoleGroup in requiredRoleList)
            {
                var requiredRoleSubList = requiredRoleGroup.Split(",");
                var hasRequiredRole = true;

                foreach (var requiredRole in requiredRoleSubList)
                {
                    if (!roleList.Any(x => x == requiredRole))
                    {
                        hasRequiredRole = false;
                        break;
                    }
                }

                if (hasRequiredRole)
                {
                    return true;
                }
            }

            return false;
        }
    }