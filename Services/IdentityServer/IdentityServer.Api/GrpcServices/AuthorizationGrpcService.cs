using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Grpc.Core;
using IdentityServer.Api.Protos;
using Microsoft.IdentityModel.Tokens;

namespace IdentityServer.Api.GrpcServices
{
    public class AuthorizationGrpcService : AuthorizationService.AuthorizationServiceBase
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthorizationGrpcService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration)); ;
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor)); ;
        }


        public override Task<ValidateTokenResponse> ValidateJwtBearerToken(ValidateTokenRequest request, ServerCallContext context)
        {
            var httpContextRequest = _httpContextAccessor.HttpContext?.Request;

            string hostUrl = $"{httpContextRequest?.Scheme}://{httpContextRequest?.Host}";

            var jwtSecretKey = _configuration.GetValue<string>("JWT:SecretKey");
            byte[] secretKey = Encoding.UTF8.GetBytes(jwtSecretKey ?? throw new ArgumentNullException(nameof(jwtSecretKey)));
            var tokenHandler = new JwtSecurityTokenHandler();
            var response = new ValidateTokenResponse();
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = hostUrl,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                ValidateLifetime = true
            };

            try
            {
                var claimsPrincipal = tokenHandler.ValidateToken(request.Token, tokenValidationParameters, out _);

                response.Valid = true;

                var roles = claimsPrincipal.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();

                var rolesString = string.Join(",", roles);
                response.Roles = rolesString;
                response.Valid = true;

                return Task.FromResult(response);
            }
            catch
            {
                response.Valid= false;

                response.Roles = "";

                return Task.FromResult(response);
            }

        }
    }
}
