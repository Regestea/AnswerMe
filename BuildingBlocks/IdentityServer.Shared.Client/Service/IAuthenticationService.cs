namespace IdentityServer.Shared.Client.Service;

public interface IAuthenticationService
{
    Task<bool> IsAuthenticatedAsync(string? jwtToken, string? requiredRoles = null);
}