using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using AnswerMe.Client.Core.DTOs.User;
using AnswerMe.Client.Core.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace AnswerMe.Client.Core.Auth
{
    public class AuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorageService;
        private readonly OnlineHubService _onlineService;
        private readonly PvHubService _pvHubService;

        public AuthStateProvider(ILocalStorageService localStorageService, OnlineHubService onlineService, PvHubService pvHubService)
        {
            _localStorageService = localStorageService;
            _onlineService = onlineService;
            _pvHubService = pvHubService;
        }


        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            string authToken = await _localStorageService.GetItemAsStringAsync("authToken");
            var identity = new ClaimsIdentity();

            if (!string.IsNullOrEmpty(authToken))
            {
                try
                {
                    identity = new ClaimsIdentity(ParseClaimsFromJwt(authToken), "jwt");
                }
                catch
                {
                    await _localStorageService.RemoveItemAsync("authToken");
                    identity = new ClaimsIdentity();
                }
            }
           
            var user = new ClaimsPrincipal(identity);
            var state = new AuthenticationState(user);
            
            await _onlineService.SetToken(_localStorageService);
            await _pvHubService.SetToken(_localStorageService);

            NotifyAuthenticationStateChanged(Task.FromResult(state));

            return state;

        }

        public async Task<UserDto> ExtractUserDataFromLocalTokenAsync()
        {
             var authToken = await _localStorageService.GetItemAsStringAsync("authToken");
             var tokenHandler = new JwtSecurityTokenHandler();
             var jwtToken = tokenHandler.ReadJwtToken(authToken);
             var id = jwtToken.Claims.FirstOrDefault(c => c.Type == nameof(UserDto.id))?.Value;
             var idName = jwtToken.Claims.FirstOrDefault(c => c.Type == nameof(UserDto.IdName))?.Value;
             var phoneNumber = jwtToken.Claims.FirstOrDefault(c => c.Type == nameof(UserDto.PhoneNumber))?.Value;
             
            var user = new UserDto()
            {
                id = Guid.Parse(id),
                IdName = idName,
                PhoneNumber = phoneNumber
            };

            return user;

        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2:
                    base64 += "==";
                    break;
                case 3:
                    base64 += "=";
                    break;
            }

            return Convert.FromBase64String(base64);
        }

        private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer
                .Deserialize<Dictionary<string, object>>(jsonBytes);

            var claims = keyValuePairs?.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()!));

            return claims!;
        }

    }
}
