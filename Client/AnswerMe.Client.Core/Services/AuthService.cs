using System.Net;
using AnswerMe.Client.Core.Extensions;
using AnswerMe.Client.Core.Services.Interfaces;
using Blazored.LocalStorage;
using Models.Shared.OneOfTypes;
using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Requests.User;
using Models.Shared.Responses.Shared;
using OneOf.Types;

namespace AnswerMe.Client.Core.Services;

public class AuthService:IAuthService
{
    private readonly HttpClient _httpClient;
    ILocalStorageService _localStorageService;

    public AuthService(IHttpClientFactory httpClientFactory, ILocalStorageService localStorageService)
    {
        _localStorageService = localStorageService;
        _httpClient = httpClientFactory.CreateClient(nameof(Enums.HttpClients.IdentityServer));
    }
    
    public async Task<CreateResponse<IdResponse>> Register(RegisterUserRequest request)
    {
        var requestStringContent = await JsonConverter.ToStringContent(request);

        var response = await _httpClient.SendRequestAsync("Auth/Register", HttpMethod.Post, requestStringContent);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var idResponse =await JsonConverter.ToObject<IdResponse>(response.Content);
            return new Success<IdResponse>(idResponse);
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            return await JsonConverter.ToValidationFailedList(response.Content);
        }

        return new Error<string>("unknown issue");
    }

    public async Task<ReadResponse<TokenResponse>> Login(LoginUserRequest request)
    {
        var requestStringContent = await JsonConverter.ToStringContent(request);

        var response = await _httpClient.SendRequestAsync("Auth/Login", HttpMethod.Post, requestStringContent);
        Console.WriteLine(_httpClient.BaseAddress);
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var token =await JsonConverter.ToObject<TokenResponse>(response.Content);
            return new Success<TokenResponse>(token);
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            return new AccessDenied();
        }

        return new NotFound();
    }
}