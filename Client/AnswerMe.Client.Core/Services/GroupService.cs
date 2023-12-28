using System.Net;
using AnswerMe.Client.Core.Extensions;
using AnswerMe.Client.Core.Services.Interfaces;
using Blazored.LocalStorage;
using Models.Shared.OneOfTypes;
using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Requests.Group;
using Models.Shared.Requests.Shared;
using Models.Shared.Responses.Group;
using Models.Shared.Responses.Shared;
using Models.Shared.Responses.User;
using OneOf.Types;

namespace AnswerMe.Client.Core.Services;

public class GroupService : IGroupService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorageService;

    public GroupService(IHttpClientFactory httpClientFactory, ILocalStorageService localStorageService)
    {
        _localStorageService = localStorageService;
        _httpClient = httpClientFactory.CreateClient(nameof(Enums.HttpClients.AnswerMe));
    }

    public async Task<ReadResponse<GroupResponse>> GetByIdAsync(Guid groupId)
    {
        await _httpClient.AddAuthHeader(_localStorageService);

        var response = await _httpClient.SendRequestAsync($"Group/{groupId}", HttpMethod.Get);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var group = await JsonConverter.ToObject<GroupResponse>(response.Content);

            return new Success<GroupResponse>(group);
        }

        if (response.StatusCode == HttpStatusCode.Forbidden)
        {
            return new AccessDenied();
        }

        return new NotFound();
    }

    public async Task<ReadResponse<PagedListResponse<GroupResponse>>> GetGroupsAsync(
        PaginationRequest paginationRequest)
    {
        await _httpClient.AddAuthHeader(_localStorageService);

        var response = await _httpClient.SendRequestAsync($"Group/List", HttpMethod.Get);

        var groups = await JsonConverter.ToObject<PagedListResponse<GroupResponse>>(response.Content);

        return new Success<PagedListResponse<GroupResponse>>(groups);
    }

    public async Task<ReadResponse<PagedListResponse<PreviewGroupUserResponse>>> UserListAsync(Guid groupId,
        PaginationRequest paginationRequest)
    {
        await _httpClient.AddAuthHeader(_localStorageService);

        var response = await _httpClient.SendRequestAsync($"Group/{groupId}/User/List", HttpMethod.Get);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var users = await JsonConverter.ToObject<PagedListResponse<PreviewGroupUserResponse>>(response.Content);

            return new Success<PagedListResponse<PreviewGroupUserResponse>>(users);
        }

        if (response.StatusCode == HttpStatusCode.Forbidden)
        {
            return new AccessDenied();
        }

        return new NotFound();
    }

    public async Task<CreateResponse<IdResponse>> SetUserAsAdminAsync(Guid groupId, Guid userId)
    {
        await _httpClient.AddAuthHeader(_localStorageService);

        var response = await _httpClient.SendRequestAsync($"Group/{groupId}/Admin/{userId}", HttpMethod.Post);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var idResponse = await JsonConverter.ToObject<IdResponse>(response.Content);

            return new Success<IdResponse>(idResponse);
        }

        if (response.StatusCode == HttpStatusCode.Forbidden)
        {
            return new AccessDenied();
        }

        return new NotFound();
    }

    public async Task<DeleteResponse> RemoveUserFromAdminsAsync(Guid groupId, Guid userId)
    {
        await _httpClient.AddAuthHeader(_localStorageService);

        var response = await _httpClient.SendRequestAsync($"Group/{groupId}/Admin/{userId}", HttpMethod.Delete);

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return new Success();
        }

        if (response.StatusCode == HttpStatusCode.Forbidden)
        {
            return new AccessDenied();
        }

        return new NotFound();
    }

    public async Task<CreateResponse<IdResponse>> CreateAsync(CreateGroupRequest request)
    {
        await _httpClient.AddAuthHeader(_localStorageService);

        var requestStringContent = await JsonConverter.ToStringContent(request);

        var response = await _httpClient.SendRequestAsync($"Group", HttpMethod.Post, requestStringContent);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var idResponse = await JsonConverter.ToObject<IdResponse>(response.Content);

            return new Success<IdResponse>(idResponse);
        }

        return await JsonConverter.ToValidationFailedList(response.Content);
    }


    public async Task<UpdateResponse> EditAsync(Guid groupId, EditGroupRequest request)
    {
        await _httpClient.AddAuthHeader(_localStorageService);

        var requestStringContent = await JsonConverter.ToStringContent(request);

        var response = await _httpClient.SendRequestAsync($"Group/{groupId}", HttpMethod.Patch, requestStringContent);

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return new Success();
        }

        if (response.StatusCode == HttpStatusCode.Forbidden)
        {
            return new AccessDenied();
        }

        return new NotFound();
    }

    public async Task<CreateResponse<IdResponse>> JoinUserAsync(Guid groupId, Guid joinUserId)
    {
        await _httpClient.AddAuthHeader(_localStorageService);
        
        var response = await _httpClient.SendRequestAsync($"Group/{groupId}/User/{joinUserId}/Join", HttpMethod.Post);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var idResponse = await JsonConverter.ToObject<IdResponse>(response.Content);
            
            return new Success<IdResponse>(idResponse);
        }

        if (response.StatusCode == HttpStatusCode.Forbidden)
        {
            return new AccessDenied();
        }

        return new NotFound();
    }

    public async Task<DeleteResponse> KickUserAsync(Guid groupId, Guid kickUserId)
    {
        await _httpClient.AddAuthHeader(_localStorageService);

        var response = await _httpClient.SendRequestAsync($"Group/{groupId}/User/{kickUserId}/Kick", HttpMethod.Delete);

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return new Success();
        }

        if (response.StatusCode == HttpStatusCode.Forbidden)
        {
            return new AccessDenied();
        }

        return new NotFound();
    }

    public async Task<DeleteResponse> LeaveGroupAsync(Guid groupId)
    {
        await _httpClient.AddAuthHeader(_localStorageService);
        

        var response = await _httpClient.SendRequestAsync($"Group/{groupId}/Leave", HttpMethod.Delete);

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return new Success();
        }
        
        return new NotFound();
    }
}