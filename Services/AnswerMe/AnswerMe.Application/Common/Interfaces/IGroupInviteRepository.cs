using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Requests.Group;
using Models.Shared.Responses.Group;
using Models.Shared.Responses.Shared;

namespace AnswerMe.Application.Common.Interfaces;

/// <summary>
/// Represents a repository for managing group invites.
/// </summary>
public interface IGroupInviteRepository
{
    /// <summary>
    /// Asynchronously retrieves the preview information about a group using an invite token.
    /// </summary>
    /// <param name="inviteToken">The invite token for the group.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the response containing the group preview information.</returns>
    Task<ReadResponse<PreviewGroupResponse>> GetGroupPreviewAsync(string inviteToken);

    /// <summary>
    /// Creates an invite token asynchronously.
    /// </summary>
    /// <param name="loggedInUserId">The unique identifier of the logged in user.</param>
    /// <param name="groupId">The unique identifier of the group.</param>
    /// <param name="request">The request object containing the details for creating the invite token.</param>
    /// <returns>A task representing the asynchronous operation.
    /// The task result contains the response object
    /// containing the created token.</returns>
    Task<CreateResponse<TokenResponse>> CreateAsync(Guid loggedInUserId, Guid groupId, CreateInviteTokenRequest request);

    /// <summary>
    /// Joins a group asynchronously using the specified user and invite token.
    /// </summary>
    /// <param name="loggedInUserId">The ID of the logged in user.</param>
    /// <param name="inviteToken">The invite token for the group.</param>
    /// <returns>A task representing the asynchronous operation. The task result is a CreateResponse containing an IdResponse.</returns>
    Task<CreateResponse<IdResponse>> JoinGroupAsync(Guid loggedInUserId, string inviteToken);
}