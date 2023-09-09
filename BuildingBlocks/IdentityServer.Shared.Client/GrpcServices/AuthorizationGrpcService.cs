using IdentityServer.Api.Protos;

namespace IdentityServer.Shared.Client.GrpcServices
{
    public class AuthorizationGrpcServices
    {
        private readonly AuthorizationService.AuthorizationServiceClient _authorizationGrpcService;

        public AuthorizationGrpcServices(AuthorizationService.AuthorizationServiceClient authorizationGrpcService)
        {
            _authorizationGrpcService = authorizationGrpcService;
        }


        public async Task<ValidateTokenResponse> ValidateTokenAsync(string token)
        {
            return await _authorizationGrpcService.ValidateJwtBearerTokenAsync(new ValidateTokenRequest() { Token = token });
        }
    }
}
