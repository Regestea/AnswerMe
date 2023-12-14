using AnswerMe.Application.Common.Interfaces;
using IdentityServer.Shared.Client.Attributes;
using IdentityServer.Shared.Client.Repositories;
using IdentityServer.Shared.Client.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Shared.Requests.Message;
using Models.Shared.Requests.Shared;
using Models.Shared.Responses.Message;
using Models.Shared.Responses.Shared;

namespace AnswerMe.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizeByIdentityServer]
    public class PrivateMessageController : ControllerBase
    {
        private readonly IPrivateMessageService _privateMessageService;
        private readonly IJwtTokenRepository _jwtTokenRepository;

        public PrivateMessageController(IPrivateMessageService privateMessageService, IJwtTokenRepository jwtTokenRepository)
        {
            _privateMessageService = privateMessageService;
            _jwtTokenRepository = jwtTokenRepository;
        }


        /// <summary>
        /// Get a list of private messages in a room.
        /// </summary>
        /// <param name="roomId">The unique identifier of the room.</param>
        /// <param name="paginationRequest">Pagination parameters.</param>
        /// <response code="200">OK: The list of private messages was successfully retrieved.</response>
        /// <response code="404">Not Found: The room or user could not be found.</response>
        [HttpGet("{roomId:guid}")]
        [ProducesResponseType(typeof(PagedListResponse<MessageResponse>), StatusCodes.Status200OK )]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListAsync([FromRoute] Guid roomId, [FromQuery] PaginationRequest paginationRequest)
        {
            var requestToken = _jwtTokenRepository.GetJwtToken();
            var loggedInUser = _jwtTokenRepository.ExtractUserDataFromToken(requestToken);

            var result = await _privateMessageService.GetListAsync(loggedInUser.id, roomId, paginationRequest);

            if (result.IsNotFound)
            {
                return NotFound();
            }

            return Ok(result.AsSuccess.Value);
        }

        /// <summary>
        /// Send a private message to a room.
        /// </summary>
        /// <param name="roomId">The unique identifier of the room.</param>
        /// <param name="request">The message to send.</param>
        /// <response code="200">OK: The message was successfully sent.</response>
        /// <response code="404">Not Found: The room or user could not be found.</response>
        [HttpPost("{roomId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IdResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SendAsync([FromRoute] Guid roomId, [FromBody] SendMessageRequest request)
        {
            var requestToken = _jwtTokenRepository.GetJwtToken();
            var loggedInUser = _jwtTokenRepository.ExtractUserDataFromToken(requestToken);

            var result = await _privateMessageService.SendAsync(loggedInUser.id, roomId, request);

            if (result.IsSuccess)
            {
                return Ok(result.AsSuccess.Value);
            }

            return NotFound();
        }

        /// <summary>
        /// Edit a private message.
        /// </summary>
        /// <param name="messageId">The unique identifier of the message to edit.</param>
        /// <param name="request">The edited message content.</param>
        /// <response code="204">No Content: The message was successfully edited.</response>
        /// <response code="404">Not Found: The message or user could not be found.</response>
        [HttpPatch("{messageId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> EditAsync([FromRoute] Guid messageId, EditMessageRequest request)
        {
            var requestToken = _jwtTokenRepository.GetJwtToken();
            var loggedInUser = _jwtTokenRepository.ExtractUserDataFromToken(requestToken);

            var result = await _privateMessageService.UpdateAsync(loggedInUser.id, messageId, request);

            if (result.IsSuccess)
            {
                return NoContent();
            }

            return NotFound();
        }



        /// <summary>
        /// Delete a private message.
        /// </summary>
        /// <param name="messageId">The unique identifier of the message to delete.</param>
        /// <response code="204">No Content: The message was successfully deleted.</response>
        /// <response code="404">Not Found: The message or user could not be found.</response>
        [HttpDelete("{messageId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid messageId)
        {
            var requestToken = _jwtTokenRepository.GetJwtToken();
            var loggedInUser = _jwtTokenRepository.ExtractUserDataFromToken(requestToken);

            var result = await _privateMessageService.DeleteAsync(loggedInUser.id, messageId);

            if (result.IsNotFound)
            {
                return NotFound();
            }

            return NoContent();
        }

        /// <summary>
        /// Edit media associated with a private message.
        /// </summary>
        /// <param name="messageId">The unique identifier of the message containing the media.</param>
        /// <param name="mediaId">The unique identifier of the media to edit.</param>
        /// <param name="request">The edited media content.</param>
        /// <response code="204">No Content: The media was successfully edited.</response>
        /// <response code="400">Bad Request: The request contains validation errors.</response>
        /// <response code="403">Forbidden: The user is not allowed to edit the media.</response>
        /// <response code="404">Not Found: The message, media, or user could not be found.</response>
        [HttpPatch("{messageId:guid}/Media/{mediaId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> EditMediaAsync([FromRoute] Guid messageId, [FromRoute] Guid mediaId, EditMessageMediaRequest request)
        {
            var requestToken = _jwtTokenRepository.GetJwtToken();
            var loggedInUser = _jwtTokenRepository.ExtractUserDataFromToken(requestToken);

            var result = await _privateMessageService.UpdateMediaAsync(loggedInUser.id, messageId, mediaId, request);

            if (result.IsNotFound)
            {
                return NotFound();
            }

            if (result.IsValidationFailure)
            {
                ModelState.AddModelError(result.AsValidationFailure.Field, result.AsValidationFailure.Error);
                return BadRequest(ModelState);
            }

            if (result.IsAccessDenied)
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }

            return NoContent();
        }

        /// <summary>
        /// Delete media associated with a private message.
        /// </summary>
        /// <param name="messageId">The unique identifier of the message containing the media.</param>
        /// <param name="mediaId">The unique identifier of the media to delete.</param>
        /// <response code="204">No Content: The media was successfully deleted.</response>
        /// <response code="404">Not Found: The media or user could not be found.</response>
        [HttpDelete("{messageId:guid}/Media/{mediaId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteMediaAsync([FromRoute] Guid messageId, [FromRoute] Guid mediaId)
        {
            var requestToken = _jwtTokenRepository.GetJwtToken();
            var loggedInUser = _jwtTokenRepository.ExtractUserDataFromToken(requestToken);

            var result = await _privateMessageService.DeleteMediaAsync(loggedInUser.id, messageId, mediaId);

            if (result.IsNotFound)
            {
                return NotFound();
            }

            return NoContent();
        }


    }
}
