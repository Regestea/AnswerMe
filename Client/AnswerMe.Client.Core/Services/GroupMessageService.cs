using System.Net;
using AnswerMe.Client.Core.Extensions;
using AnswerMe.Client.Core.Services.Interfaces;
using Blazored.LocalStorage;
using Models.Shared.OneOfTypes;
using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Requests.Message;
using Models.Shared.Requests.Shared;
using Models.Shared.Responses.Message;
using Models.Shared.Responses.Shared;
using OneOf.Types;

namespace AnswerMe.Client.Core.Services;

public class GroupMessageService : IGroupMessageService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorageService;

    public GroupMessageService(IHttpClientFactory httpClientFactory, ILocalStorageService localStorageService)
    {
        _localStorageService = localStorageService;
        _httpClient = httpClientFactory.CreateClient(nameof(Enums.HttpClients.AnswerMe));
    }


    public async Task<ReadResponse<PagedListResponse<MessageResponse>>> GetGroupMessagesAsync(Guid roomId,
        bool jumpToUnRead, PaginationRequest request)
    {
        await _httpClient.AddAuthHeader(_localStorageService);

        var response =
            await _httpClient.SendRequestAsync(
                $"GroupMessage/{roomId}/List?"
                    .AddPagination(request)
                    .AddQuery("jumpToUnRead", jumpToUnRead.ToString()), HttpMethod.Get);

        var messages = await JsonConverter.ToObject<PagedListResponse<MessageResponse>>(response.Content);

        return new Success<PagedListResponse<MessageResponse>>(messages);
    }

   

    public async Task<ReadResponse<MessageResponse>> GetGroupMessageAsync(Guid messageId)
    {
        await _httpClient.AddAuthHeader(_localStorageService);

        var response = await _httpClient.SendRequestAsync($"GroupMessage/{messageId}", HttpMethod.Get);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var message = await JsonConverter.ToObject<MessageResponse>(response.Content);

            return new Success<MessageResponse>(message);
        }

        if (response.StatusCode == HttpStatusCode.Forbidden)
        {
            return new AccessDenied();
        }

        return new NotFound();
    }

    public async Task<CreateResponse<IdResponse>> SendMessageAsync(Guid roomId, SendMessageRequest request)
    {
        await _httpClient.AddAuthHeader(_localStorageService);

        var requestStringContent = await JsonConverter.ToStringContent(request);

        var response =
            await _httpClient.SendRequestAsync($"GroupMessage/{roomId}", HttpMethod.Post, requestStringContent);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var messageId = await JsonConverter.ToObject<IdResponse>(response.Content);

            return new Success<IdResponse>(messageId);
        }

        return new NotFound();
    }

    public async Task<UpdateResponse> EditMessageTextAsync(Guid messageId, EditMessageRequest request)
    {
        await _httpClient.AddAuthHeader(_localStorageService);

        var requestStringContent = await JsonConverter.ToStringContent(request);

        var response =
            await _httpClient.SendRequestAsync($"GroupMessage/{messageId}", HttpMethod.Patch, requestStringContent);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            return new Success();
        }

        return new NotFound();
    }

    public async Task<DeleteResponse> DeleteMessageAsync(Guid messageId)
    {
        await _httpClient.AddAuthHeader(_localStorageService);

        var response = await _httpClient.SendRequestAsync($"GroupMessage/{messageId}", HttpMethod.Delete);

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return new Success();
        }

        return new NotFound();
    }

    public async Task<UpdateResponse> EditMessageMediaAsync(Guid messageId, Guid mediaId,
        EditMessageMediaRequest request)
    {
        await _httpClient.AddAuthHeader(_localStorageService);

        var requestStringContent = await JsonConverter.ToStringContent(request);

        var response = await _httpClient.SendRequestAsync($"GroupMessage/{messageId}/Media/{mediaId}", HttpMethod.Patch,
            requestStringContent);

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return new Success();
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            return await JsonConverter.ToValidationFailedList(response.Content);
        }

        if (response.StatusCode == HttpStatusCode.Forbidden)
        {
            return new AccessDenied();
        }

        return new NotFound();
    }

    public async Task<DeleteResponse> DeleteMessageMediaAsync(Guid messageId, Guid mediaId)
    {
        await _httpClient.AddAuthHeader(_localStorageService);

        var response =
            await _httpClient.SendRequestAsync($"GroupMessage/{messageId}/Media/{mediaId}", HttpMethod.Delete);

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return new Success();
        }

        return new NotFound();
    }
}