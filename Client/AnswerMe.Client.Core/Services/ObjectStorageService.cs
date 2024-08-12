using System.Net;
using AnswerMe.Client.Core.Extensions;
using AnswerMe.Client.Core.Services.Interfaces;
using Blazored.LocalStorage;
using Models.Shared.OneOfTypes;
using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Requests.Shared;
using Models.Shared.Requests.Upload;
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
    
    public async Task<CreateResponse<ChunkUploadResponse>> UploadChunkAsync(FileChunkRequest request)
    {
        await _httpClient.AddAuthHeader(_localStorageService);
      
        var requestStringContent = await JsonConverter.ToStringContent(request);
        
        var response = await _httpClient.SendRequestAsync($"ObjectStorage", HttpMethod.Post,requestStringContent);

        if (response.StatusCode == HttpStatusCode.Created)
        {
            var uploadResponse = await JsonConverter.ToObject<ChunkUploadResponse>(response.Content);

            return new Success<ChunkUploadResponse>(uploadResponse);
        }

        return await JsonConverter.ToValidationFailedList(response.Content);
    }

    public async Task<CreateResponse<TokenResponse>> FinalizeUploadAsync(TokenRequest request)
    {
        await _httpClient.AddAuthHeader(_localStorageService);
        
        var requestStringContent = await JsonConverter.ToStringContent(request);
        
        var response = await _httpClient.SendRequestAsync($"ObjectStorage/Finalize", HttpMethod.Post,requestStringContent);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var tokenResponse = await JsonConverter.ToObject<TokenResponse>(response.Content);

            return new Success<TokenResponse>(tokenResponse);
        }

        return await JsonConverter.ToValidationFailedList(response.Content);
    }

    public async Task<CreateResponse<TokenResponse>> GetUploadTokenAsync(UploadRequest request)
    {
        await _httpClient.AddAuthHeader(_localStorageService);

        var requestStringContent = await JsonConverter.ToStringContent(request);
        
        var response = await _httpClient.SendRequestAsync($"ObjectStorage/RequestUploadToken", HttpMethod.Post,requestStringContent);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var tokenResponse = await JsonConverter.ToObject<TokenResponse>(response.Content);

            return new Success<TokenResponse>(tokenResponse);
        }

        return await JsonConverter.ToValidationFailedList(response.Content);
    }
}