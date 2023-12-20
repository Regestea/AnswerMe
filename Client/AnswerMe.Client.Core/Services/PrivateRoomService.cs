using System.Net;
using AnswerMe.Client.Core.Extensions;
using AnswerMe.Client.Core.Services.Interfaces;
using Blazored.LocalStorage;
using Models.Shared.OneOfTypes;
using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Requests.Shared;
using Models.Shared.Responses.PrivateRoom;
using Models.Shared.Responses.Shared;
using Models.Shared.Responses.User;
using OneOf.Types;

namespace AnswerMe.Client.Core.Services;

public class PrivateRoomService:IPrivateRoomService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorageService;
    
    public PrivateRoomService(IHttpClientFactory httpClientFactory, ILocalStorageService localStorageService)
    {
        _localStorageService = localStorageService;
        _httpClient = httpClientFactory.CreateClient(nameof(Enums.HttpClients.AnswerMe));
    }
    
    
    public async Task<ReadResponse<PagedListResponse<PrivateRoomResponse>>> GetPrivateRoomsAsync(PaginationRequest request)
    {
        await _httpClient.AddAuthHeader(_localStorageService);

        var response = await _httpClient.SendRequestAsync($"PrivateRoom".AddPagination(request), HttpMethod.Get);
        
        var privateRooms = await JsonConverter.ToObject<PagedListResponse<PrivateRoomResponse>>(response.Content);

        return new Success<PagedListResponse<PrivateRoomResponse>>(privateRooms);
    }

    public async Task<ReadResponse<PrivateRoomResponse>> GetPrivateRoomByIdAsync(Guid id)
    {
        await _httpClient.AddAuthHeader(_localStorageService);

        var response = await _httpClient.SendRequestAsync($"PrivateRoom/{id}", HttpMethod.Get);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return new NotFound();
        }

        if (response.StatusCode== HttpStatusCode.Forbidden)
        {
            return new AccessDenied();
        }
        
        var privateRoom = await JsonConverter.ToObject<PrivateRoomResponse>(response.Content);

        return new Success<PrivateRoomResponse>(privateRoom);
    }

    public async Task<ReadResponse<RoomLastSeenResponse>> GetRoomLastSeenAsync(Guid contactId, Guid roomId)
    {
        await _httpClient.AddAuthHeader(_localStorageService);

        var response = await _httpClient.SendRequestAsync($"PrivateRoom/{roomId}/User/{contactId}/LastSeen", HttpMethod.Get);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return new NotFound();
        }

        if (response.StatusCode== HttpStatusCode.Forbidden)
        {
            return new AccessDenied();
        }
        
        var lastSeen = await JsonConverter.ToObject<RoomLastSeenResponse>(response.Content);

        return new Success<RoomLastSeenResponse>(lastSeen);
    }
}