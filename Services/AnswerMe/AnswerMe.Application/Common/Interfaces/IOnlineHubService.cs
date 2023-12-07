using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Responses.Message;
using Models.Shared.Responses.Shared;

namespace AnswerMe.Application.Common.Interfaces;

/// <summary>
/// Represents an online hub service.
/// </summary>
public interface IOnlineHubService
{
    /// <summary>
    /// Checks if the user with the specified user ID is currently online.
    /// </summary>
    /// <param name="userId">The unique identifier for the user whose online status needs to be checked.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a ReadResponse object with the online status of the user.</returns>
    Task<ReadResponse<BooleanResponse>> IsOnlineAsync(Guid userId);
}