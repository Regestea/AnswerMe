﻿using AnswerMe.Application.Common.Interfaces;
using AnswerMe.Application.Extensions;
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
    public class GroupMessageController : ControllerBase
    {
        private readonly IGroupMessageService _groupMessageService;
        private readonly IJwtTokenRepository _jwtTokenRepository;

        public GroupMessageController(IGroupMessageService groupMessageService, IJwtTokenRepository jwtTokenRepository)
        {
            _groupMessageService = groupMessageService;
            _jwtTokenRepository = jwtTokenRepository;
        }


        /// <summary>
        /// Get a list of messages in a group.
        /// </summary>
        /// <param name="groupId">The unique identifier of the group.</param>
        /// <param name="paginationRequest">The request for pagination.</param>
        /// <param name="jumpToUnRead">should jump to unRead page</param>
        /// <response code="200">OK: Returns the list of messages in the group.</response>
        /// <response code="403">Forbidden: You don't have permission to access the group.</response>
        /// <response code="404">Not Found: The group or messages could not be found.</response>
        [HttpGet("{groupId:guid}/List")]
        [ProducesResponseType(typeof(PagedListResponse<MessageResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListAsync([FromRoute] Guid groupId,
            [FromQuery] PaginationRequest paginationRequest, [FromQuery] bool jumpToUnRead = false)
        {
            var requestToken = _jwtTokenRepository.GetJwtToken();
            var loggedInUser = _jwtTokenRepository.ExtractUserDataFromToken(requestToken);

            var result =
                await _groupMessageService.GetListAsync(loggedInUser.id, groupId, jumpToUnRead, paginationRequest);

            if (result.IsAccessDenied)
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }

            if (result.IsNotFound)
            {
                return NotFound();
            }

            return Ok(result.AsSuccess.Value);
        }


        /// <summary>
        /// Get details of a specific message.
        /// </summary>
        /// <param name="messageId">The ID of the message to retrieve.</param>
        /// <returns>
        /// Returns the details of the specified message if successful.
        /// Returns NotFound if the message with the given ID is not found.
        /// Returns Forbidden if access to the message is denied.
        /// </returns>
        /// <response code="200">Returns the details of the specified message.</response>
        /// <response code="404">If the message with the given ID is not found, returns a not found status.</response>
        /// <response code="403">If access to the message is denied, returns a forbidden status.</response>
        [HttpGet("{messageId:guid}")]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAsync([FromRoute] Guid messageId)
        {
            var requestToken = _jwtTokenRepository.GetJwtToken();
            var loggedInUser = _jwtTokenRepository.ExtractUserDataFromToken(requestToken);

            var result = await _groupMessageService.GetAsync(loggedInUser.id, messageId);

            if (result.IsNotFound)
            {
                return NotFound();
            }

            if (result.IsAccessDenied)
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }

            return Ok(result.AsSuccess.Value);
        }


        /// <summary>
        /// Send a message to a group.
        /// </summary>
        /// <param name="groupId">The unique identifier of the group.</param>
        /// <param name="request">The message to send.</param>
        /// <response code="200">OK: Returns the sent message.</response>
        /// <response code="403">Forbidden: You don't have permission to send a message to the group.</response>
        /// <response code="404">Not Found: The group or user was not found.</response>
        [HttpPost("{groupId:guid}")]
        [ProducesResponseType(typeof(IdResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SendAsync([FromRoute] Guid groupId, [FromBody] SendMessageRequest request)
        {
            var requestToken = _jwtTokenRepository.GetJwtToken();
            var loggedInUser = _jwtTokenRepository.ExtractUserDataFromToken(requestToken);

            var result = await _groupMessageService.SendAsync(loggedInUser.id, groupId, request);

            if (result.IsAccessDenied)
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }

            if (result.IsSuccess)
            {
                return Ok(result.AsSuccess.Value);
            }

            return NotFound();
        }


        /// <summary>
        /// Edit a message in a group.
        /// </summary>
        /// <param name="messageId">The unique identifier of the message to edit.</param>
        /// <param name="request">The edited message content.</param>
        /// <response code="204">No Content: The message was successfully edited.</response>
        /// <response code="403">Forbidden: You don't have permission to edit the message.</response>
        /// <response code="404">Not Found: The message or user could not be found.</response>
        [HttpPatch("{messageId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)] // No content as the response doesn't contain a body
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> EditAsync([FromRoute] Guid messageId, EditMessageRequest request)
        {
            var requestToken = _jwtTokenRepository.GetJwtToken();
            var loggedInUser = _jwtTokenRepository.ExtractUserDataFromToken(requestToken);

            var result = await _groupMessageService.UpdateAsync(loggedInUser.id, messageId, request);

            if (result.IsAccessDenied)
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }

            if (result.IsNotFound)
            {
                return NotFound();
            }

            return NoContent();
        }

        /// <summary>
        /// Delete a message in a group.
        /// </summary>
        /// <param name="messageId">The unique identifier of the message to delete.</param>
        /// <response code="204">No Content: The message was successfully deleted.</response>
        /// <response code="404">Not Found: The message or user could not be found.</response>
        /// <response code="403">Forbidden: You don't have permission to Delete the message.</response>
        [HttpDelete("{messageId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)] // No content as the response doesn't contain a body
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid messageId)
        {
            var requestToken = _jwtTokenRepository.GetJwtToken();
            var loggedInUser = _jwtTokenRepository.ExtractUserDataFromToken(requestToken);

            var result = await _groupMessageService.DeleteAsync(loggedInUser.id, messageId);

            if (result.IsNotFound)
            {
                return NotFound();
            }

            if (result.IsAccessDenied)
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }

            return NoContent();
        }


        /// <summary>
        /// Edit media content of a message in a group.
        /// </summary>
        /// <param name="messageId">The unique identifier of the message.</param>
        /// <param name="mediaId">The unique identifier of the media to edit.</param>
        /// <param name="request">The edited media content.</param>
        /// <response code="204">No Content: The media was successfully edited.</response>
        /// <response code="403">Forbidden: You don't have permission to edit the media.</response>
        /// <response code="400">Bad Request: The request is invalid.</response>
        /// <response code="404">Not Found: The media, message, or user could not be found.</response>
        [HttpPatch("{messageId:guid}/Media/{mediaId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)] // No content as the response doesn't contain a body
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> EditMediaAsync([FromRoute] Guid messageId, [FromRoute] Guid mediaId,
            EditMessageMediaRequest request)
        {
            var requestToken = _jwtTokenRepository.GetJwtToken();
            var loggedInUser = _jwtTokenRepository.ExtractUserDataFromToken(requestToken);

            var result = await _groupMessageService.UpdateMediaAsync(loggedInUser.id, messageId, mediaId, request);

            if (result.IsNotFound)
            {
                return NotFound();
            }

            if (result.IsAccessDenied)
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }

            if (result.IsValidationFailure)
            {
                var validationError = result.AsValidationFailure;
                ModelState.AddModelError(validationError.Field, validationError.Error);
                return BadRequest(ModelState);
            }

            return NoContent();
        }

        /// <summary>
        /// Delete media content from a message in a group.
        /// </summary>
        /// <param name="messageId">The unique identifier of the message.</param>
        /// <param name="mediaId">The unique identifier of the media to delete.</param>
        /// <response code="204">No Content: The media was successfully deleted.</response>
        /// <response code="404">Not Found: The media, message, or user could not be found.</response>
        [HttpDelete("{messageId:guid}/Media/{mediaId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)] // No content as the response doesn't contain a body
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteMediaAsync([FromRoute] Guid messageId, [FromRoute] Guid mediaId)
        {
            var requestToken = _jwtTokenRepository.GetJwtToken();
            var loggedInUser = _jwtTokenRepository.ExtractUserDataFromToken(requestToken);

            var result = await _groupMessageService.DeleteMediaAsync(loggedInUser.id, messageId, mediaId);

            if (result.IsNotFound)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}