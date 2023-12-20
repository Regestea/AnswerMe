using System.Net;
using AnswerMe.Client.Core.Extensions;
using AnswerMe.Client.Core.Services.Interfaces;
using Blazored.LocalStorage;
using Models.Shared.OneOfTypes;
using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Requests.Group;
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


    public async Task<ReadResponse<GroupResponse>> GetGroupInvitePreviewAsync(string inviteToken)
    {
        await _httpClient.AddAuthHeader(_localStorageService);

        var response = await _httpClient.SendRequestAsync($"GroupInvite/{inviteToken}", HttpMethod.Get);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return new NotFound();
        }

        var group = await JsonConverter.ToObject<GroupResponse>(response.Content);

        return new Success<GroupResponse>(group);
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

        var response = await _httpClient.SendRequestAsync($"GroupInvite/{inviteToken}/Join", HttpMethod.Post);

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