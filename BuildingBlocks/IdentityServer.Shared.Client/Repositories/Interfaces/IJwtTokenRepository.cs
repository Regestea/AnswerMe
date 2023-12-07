using IdentityServer.Shared.Client.DTOs;

namespace IdentityServer.Shared.Client.Repositories.Interfaces;

/// <summary>
/// Represents a Repository for handling JWT tokens.
/// </summary>
public interface IJwtTokenRepository
{
    /// <summary>
    /// Extracts user data from a given token.
    /// </summary>
    /// <param name="token">The token containing the user data.</param>
    /// <returns>A UserDto object representing the extracted user data.</returns>
    UserDto ExtractUserDataFromToken(string token);

    /// <summary>
    /// Retrieves a JSON Web Token (JWT) token for authentication.
    /// </summary>
    /// <returns>
    /// A string representing the JWT token.
    /// </returns>
    string GetJwtToken();
}