using System.Net;
using System.Web;
using AnswerMe.Client.Core.Extensions;
using AnswerMe.Client.Core.Services.Interfaces;
using Blazored.LocalStorage;
using Models.Shared.OneOfTypes;
using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Requests.Group;
using Models.Shared.Requests.Shared;
using Models.Shared.Responses.Group;
using Models.Shared.Responses.PrivateRoom;
using Models.Shared.Responses.Shared;
using OneOf.Types;

namespace AnswerMe.Client.Core.Services;

public class GroupInviteService : IGroupInviteService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorageService;

    public GroupInviteService(IHttpClientFactory httpClientFactory, ILocalStorageService localStorageService)
    {
        _localStorageService = localStorageService;
        _httpClient = httpClientFactory.CreateClient(nameof(Enums.HttpClients.AnswerMe));
    }


    public async Task<ReadResponse<PreviewGroupInviteResponse>> GetGroupInvitePreviewAsync(TokenRequest request)
    {
        await _httpClient.AddAuthHeader(_localStorageService);
        

        var response = await _httpClient.SendRequestAsync($"GroupInvite/Preview?".AddQuery(nameof(request.Token),HttpUtility.UrlEncode(request.Token)), HttpMethod.Get);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return new NotFound();
        }
        
        var group = await JsonConverter.ToObject<PreviewGroupInviteResponse>(response.Content);
        
        return new Success<PreviewGroupInviteResponse>(group);
    }

    public async Task<CreateResponse<TokenResponse>> CreateAsync(Guid groupId, CreateInviteTokenRequest request)
    {
        await _httpClient.AddAuthHeader(_localStorageService);

        var requestStringContent = await JsonConverter.ToStringContent(request);

        var response =
            await _httpClient.SendRequestAsync($"GroupInvite/{groupId}", HttpMethod.Post, requestStringContent);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var token = await JsonConverter.ToObject<TokenResponse>(response.Content);
            return new Success<TokenResponse>(token);
        }

        if (response.StatusCode == HttpStatusCode.Forbidden)
        {
            return new AccessDenied();
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            return await JsonConverter.ToValidationFailedList(response.Content);
        }

        return new NotFound();
    }

    public async Task<CreateResponse<IdResponse>> JoinGroupAsync(string inviteToken)
    {
        await _httpClient.AddAuthHeader(_localStorageService);
        var request = new TokenRequest() { Token = inviteToken };
        var requestStringContent = await JsonConverter.ToStringContent(request);

        var response = await _httpClient.SendRequestAsync($"GroupInvite/Join", HttpMethod.Post,requestStringContent);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var idResponse = await JsonConverter.ToObject<IdResponse>(response.Content);
            return new Success<IdResponse>(idResponse);
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            return await JsonConverter.ToValidationFailedList(response.Content);
        }

        return new NotFound();
    }
}