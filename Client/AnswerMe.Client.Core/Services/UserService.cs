using System.Net;
using AnswerMe.Client.Core.Extensions;
using AnswerMe.Client.Core.Services.Interfaces;
using Blazored.LocalStorage;
using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Requests.User;
using Models.Shared.Responses.Shared;
using Models.Shared.Responses.User;
using OneOf.Types;

namespace AnswerMe.Client.Core.Services;

public class UserService: IUserService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorageService;
    
    public UserService(IHttpClientFactory httpClientFactory, ILocalStorageService localStorageService)
    {
        _localStorageService = localStorageService;
        _httpClient = httpClientFactory.CreateClient(nameof(Enums.HttpClients.AnswerMe));
    }
    
    
    public async Task<ReadResponse<UserResponse>> GetUserAsync()
    {
        await _httpClient.AddAuthHeader(_localStorageService);

        var response = await _httpClient.SendRequestAsync("User", HttpMethod.Get);

        var user = await JsonConverter.ToObject<UserResponse>(response.Content);

        Console.WriteLine(user.FullName);
        
        return new Success<UserResponse>(user);
    }

    public async Task<UpdateResponse> EditUserAsync(EditUserRequest request)
    {
        var requestStringContent = await JsonConverter.ToStringContent(request);

        await _httpClient.AddAuthHeader(_localStorageService);

        await _httpClient.SendRequestAsync($"User", HttpMethod.Patch, requestStringContent);
        
        return new Success();
    }

    public async Task<ReadResponse<UserResponse>> GetUserByIdAsync(Guid id)
    {
        await _httpClient.AddAuthHeader(_localStorageService);

        var response = await _httpClient.SendRequestAsync($"User/{id}", HttpMethod.Get);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var user = await JsonConverter.ToObject<UserResponse>(response.Content);
            
            return new Success<UserResponse>(user);
        }

        return new NotFound();
    }

    public async Task<ReadResponse<BooleanResponse>> IsOnlineAsync(Guid id)
    {
        await _httpClient.AddAuthHeader(_localStorageService);

        var response = await _httpClient.SendRequestAsync($"User/{id}/IsOnline", HttpMethod.Get);
        
        var isOnline = await JsonConverter.ToObject<BooleanResponse>(response.Content);

        return new Success<BooleanResponse>(isOnline);
    }

    public async Task<ReadResponse<BooleanResponse>> ExistsAsync(Guid id)
    {
        await _httpClient.AddAuthHeader(_localStorageService);

        var response = await _httpClient.SendRequestAsync($"User/{id}/Exist", HttpMethod.Get);
        
        var exist = await JsonConverter.ToObject<BooleanResponse>(response.Content);

        return new Success<BooleanResponse>(exist);
    }
}