﻿using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Requests.Group;
using Models.Shared.Requests.Shared;
using Models.Shared.Responses.Group;
using Models.Shared.Responses.PrivateRoom;
using Models.Shared.Responses.Shared;
using Models.Shared.Responses.User;

namespace AnswerMe.Application.Common.Interfaces;

/// <summary>
/// Represents a repository for managing groups.
/// </summary>
public interface IGroupRepository
{
    /// <summary>
    /// Retrieves group information asynchronously for a logged in user.
    /// </summary>
    /// <param name="loggedInUserId">The unique identifier of the logged in user.</param>
    /// <param name="groupId">The unique identifier of the group.</param>
    /// <returns>A task representing the asynchronous operation with the result containing
    /// the group details wrapped in a ReadResponse object.</returns>
    /// <exception cref="System.Exception">Thrown when an error occurs while retrieving group information.</exception>
    Task<ReadResponse<PreviewGroupResponse>> GetAsync(Guid loggedInUserId, Guid groupId);

    /// <summary>
    /// Retrieves a paged list of groups asynchronously.
    /// </summary>
    /// <param name="loggedInUserId">The ID of the logged in user.</param>
    /// <param name="paginationRequest">The pagination request settings.</param>
    /// <returns>A task representing the asynchronous operation
    /// , containing a read response with a paged list of group responses.</returns>
    Task<ReadResponse<PagedListResponse<GroupResponse>>> GetListAsync(Guid loggedInUserId,
        PaginationRequest paginationRequest);

    /// <summary>
    /// Retrieves a paged list of users in a group asynchronously.
    /// </summary>
    /// <param name="loggedInUserId">The ID of the logged in user.</param>
    /// <param name="groupId">The ID of the group.</param>
    /// <param name="paginationRequest">The pagination request settings.</param>
    /// <returns>A task representing the asynchronous operation
    /// , containing a read response with a paged list of user responses.</returns>
    Task<ReadResponse<PagedListResponse<PreviewGroupUserResponse>>> UserListAsync(Guid loggedInUserId, Guid groupId,
        PaginationRequest paginationRequest);

    /// <summary>
    /// Sets a user as an admin in a group asynchronously.
    /// </summary>
    /// <param name="loggedInUserId">The ID of the logged in user.</param>
    /// <param name="groupId">The ID of the group.</param>
    /// <param name="userId">The ID of the user to set as an admin.</param>
    /// <returns>A task that represents the asynchronous operation. The task result
    /// contains a <see cref="CreateResponse{T}"/> of type <see cref="IdResponse"/>.</returns>
    Task<CreateResponse<IdResponse>> SetUserAsAdminAsync(Guid loggedInUserId, Guid groupId, Guid userId);

    /// <summary>
    /// Is User Admin.
    /// </summary>
    /// <param name="loggedInUserId">The ID of the logged in user.</param>
    /// <param name="groupId">The ID of the group.</param>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>A task that represents the asynchronous operation. The task result
    /// contains a <see cref="ReadResponse{T}"/> of type <see cref="BooleanResponse"/>.</returns>
    Task<ReadResponse<BooleanResponse>> IsAdminAsync(Guid loggedInUserId, Guid groupId, Guid userId);

    /// <summary>
    /// Removes a user from the admin list of a group asynchronously.
    /// </summary>
    /// <param name="loggedInUserId">The ID of the user who is authorized to remove users from the admin list.</param>
    /// <param name="groupId">The ID of the group from which the user should be removed.</param>
    /// <param name="userId">The ID of the user to be removed from the admin list.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a DeleteResponse object.</returns>
    Task<DeleteResponse> RemoveUserFromAdminsAsync(Guid loggedInUserId, Guid groupId, Guid userId);

    /// <summary>
    /// Creates a new group asynchronously.
    /// </summary>
    /// <param name="loggedInUserId">The unique identifier of the logged in user.</param>
    /// <param name="request">The request object containing the data to be used for creating the group.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains
    /// the response object of type <see cref="CreateResponse<IdResponse>"/> which contains the newly created group's ID.</returns>
    Task<CreateResponse<IdResponse>> CreateAsync(Guid loggedInUserId, CreateGroupRequest request);

    /// <summary>
    /// Edits a group asynchronously.
    /// </summary>
    /// <param name="loggedInUserId">The ID of the logged in user.</param>
    /// <param name="groupId">The ID of the group to edit.</param>
    /// <param name="request">The request containing the updated group information.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the response from the update operation.</returns>
    Task<UpdateResponse> EditAsync(Guid loggedInUserId, Guid groupId, EditGroupRequest request);


    /// <summary>
    /// Get last seen information for a specific user in group.
    /// </summary>
    /// <param name="loggedInUserId">The unique identifier of the logged-in user.</param>
    /// <param name="groupId">The unique identifier of the group.</param>
    /// <param name="userId">The unique identifier of the user for whom to retrieve the last seen information.</param>
    /// <returns>A task that represents the asynchronous operation with the result
    /// being a <see cref="ReadResponse{T}"/> containing the last seen information of type <see cref="RoomLastSeenResponse"/>.</returns>
    Task<ReadResponse<RoomLastSeenResponse>> GetLastSeenAsync(Guid loggedInUserId, Guid groupId, Guid userId);

    /// <summary>
    /// Gets the list of contacts that are not in the group
    /// </summary>
    /// <param name="loggedInUserId"></param>
    /// <param name="groupId"></param>
    /// <param name="paginationRequest"></param>
    /// <returns> A task that represents the asynchronous operation with the result being a <see cref="ReadResponse{T}"/> object. </returns>
    Task<ReadResponse<PagedListResponse<UserResponse>>> GetUnAddedContactsAsync(Guid loggedInUserId, Guid groupId,
        PaginationRequest paginationRequest);

    /// <summary>
    /// Joins a user to a group asynchronously.
    /// </summary>
    /// <param name="loggedInUserId">The ID of the currently logged in user.</param>
    /// <param name="groupId">The ID of the group to join.</param>
    /// <param name="joinUserId">The ID of the user to join.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains
    /// a <see cref="CreateResponse{T}"/> object, where T is of type <see cref="IdResponse"/>.</returns>
    Task<CreateResponse<IdResponse>> JoinUserAsync(Guid loggedInUserId, Guid groupId, Guid joinUserId);

    ///<summary>
    /// Gets the number of members in a group
    /// </summary>
    /// <param name="loggedInUserId">The ID of the logged-in user.</param>
    /// <param name="groupId">The ID of the group.</param>
    /// <returns>A <see cref="ReadResponse{T}"/> object, where T is of type <see cref="MemberCountResponse"/>
    /// a <see cref="ReadResponse{T}"/> object, where T is of type <see cref="MemberCountResponse"/>.</returns>
    Task<ReadResponse<MemberCountResponse>> MembersCountAsync(Guid loggedInUserId, Guid groupId);

    /// <summary>
    /// Kicks a user from a group asynchronously.
    /// </summary>
    /// <param name="loggedInUserId">The ID of the user who is logged in and performing the kick action.</param>
    /// <param name="groupId">The ID of the group from which the user will be kicked.</param>
    /// <param name="kickUserId">The ID of the user to be kicked.</param>
    /// <returns>A task representing the asynchronous operation. The task result will contain information about the delete response.</returns>
    Task<DeleteResponse> KickUserAsync(Guid loggedInUserId, Guid groupId, Guid kickUserId);

    /// <summary>
    /// Deletes the user's association with a group asynchronously.
    /// </summary>
    /// <param name="loggedInUserId">The ID of the logged in user.</param>
    /// <param name="groupId">The ID of the group to leave.</param>
    /// <returns>A task representing the asynchronous operation, returning a DeleteResponse object.</returns>
    Task<DeleteResponse> LeaveGroupAsync(Guid loggedInUserId, Guid groupId);
}