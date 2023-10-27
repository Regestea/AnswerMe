using IdentityServer.Shared.Client.DTOs;

namespace IdentityServer.Shared.Client.Repositories.Interfaces;

public interface IJwtTokenRepository
{
    UserDto ExtractUserDataFromToken(string token);
    string GetJwtToken();
}