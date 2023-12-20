using System.Net;
using AnswerMe.Client.Core.Extensions;
using AnswerMe.Client.Core.Services.Interfaces;
using Blazored.LocalStorage;
using Models.Shared.OneOfTypes;
using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Requests.ObjectStorage;
using Models.Shared.Responses.Group;
using Models.Shared.Responses.ObjectStorage;
using Models.Shared.Responses.Shared;
using OneOf.Types;

namespace AnswerMe.Client.Core.Services;

public class ObjectStorageService : IObjectStorageService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorageService;

    public ObjectStorageService(IHttpClientFactory httpClientFactory, ILocalStorageService localStorageService)
    {
        _localStorageService = localStorageService;
        _httpClient = httpClientFactory.CreateClient(nameof(Enums.HttpClients.ObjectStorage));
    }
    
    public async Task<CreateResponse<ChunkUploadMultiResponse>> UploadChunkAsync(FileChunkRequest request)
    {
        await _httpClient.AddAuthHeader(_localStorageService);

        var requestStringContent = await JsonConverter.ToStringContent(request);
        
        var response = await _httpClient.SendRequestAsync($"ObjectStorage", HttpMethod.Post,requestStringContent);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var uploadResponse = await JsonConverter.ToObject<ChunkUploadMultiResponse>(response.Content);

            return new Success<ChunkUploadMultiResponse>(uploadResponse);
        }

        return await JsonConverter.ToValidationFailedList(response.Content);
    }

    public async Task<CreateResponse<TokenResponse>> GetUploadTokenAsync(ProfileImageUploadRequest request)
    {
        await _httpClient.AddAuthHeader(_localStorageService);

        var requestStringContent = await JsonConverter.ToStringContent(request);
        
        var response = await _httpClient.SendRequestAsync($"ObjectStorage/Profile", HttpMethod.Post,requestStringContent);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var tokenResponse = await JsonConverter.ToObject<TokenResponse>(response.Content);

            return new Success<TokenResponse>(tokenResponse);
        }

        return await JsonConverter.ToValidationFailedList(response.Content);
    }

    public async Task<CreateResponse<TokenResponse>> GetUploadTokenAsync(ImageUploadRequest request)
    {
        await _httpClient.AddAuthHeader(_localStorageService);

        var requestStringContent = await JsonConverter.ToStringContent(request);
        
        var response = await _httpClient.SendRequestAsync($"ObjectStorage/Image", HttpMethod.Post,requestStringContent);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var tokenResponse = await JsonConverter.ToObject<TokenResponse>(response.Content);

            return new Success<TokenResponse>(tokenResponse);
        }

        return await JsonConverter.ToValidationFailedList(response.Content);
    }

    public async Task<CreateResponse<TokenResponse>> GetUploadTokenAsync(AudioUploadRequest request)
    {
        await _httpClient.AddAuthHeader(_localStorageService);

        var requestStringContent = await JsonConverter.ToStringContent(request);
        
        var response = await _httpClient.SendRequestAsync($"ObjectStorage/Audio", HttpMethod.Post,requestStringContent);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var tokenResponse = await JsonConverter.ToObject<TokenResponse>(response.Content);

            return new Success<TokenResponse>(tokenResponse);
        }

        return await JsonConverter.ToValidationFailedList(response.Content);
    }

    public async Task<CreateResponse<TokenResponse>> GetUploadTokenAsync(VideoUploadRequest request)
    {
        await _httpClient.AddAuthHeader(_localStorageService);

        var requestStringContent = await JsonConverter.ToStringContent(request);
        
        var response = await _httpClient.SendRequestAsync($"ObjectStorage/Video", HttpMethod.Post,requestStringContent);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var tokenResponse = await JsonConverter.ToObject<TokenResponse>(response.Content);

            return new Success<TokenResponse>(tokenResponse);
        }

        return await JsonConverter.ToValidationFailedList(response.Content);
    }

    public async Task<CreateResponse<TokenResponse>> GetUploadTokenAsync(OtherUploadRequest request)
    {
        await _httpClient.AddAuthHeader(_localStorageService);

        var requestStringContent = await JsonConverter.ToStringContent(request);
        
        var response = await _httpClient.SendRequestAsync($"ObjectStorage/Other", HttpMethod.Post,requestStringContent);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var tokenResponse = await JsonConverter.ToObject<TokenResponse>(response.Content);

            return new Success<TokenResponse>(tokenResponse);
        }

        return await JsonConverter.ToValidationFailedList(response.Content);
    }
}