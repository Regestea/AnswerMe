using System.Collections.ObjectModel;
using AnswerMe.Application.Common.Interfaces;
using IdentityServer.Shared.Client.Attributes;
using IdentityServer.Shared.Client.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Shared.Requests.Group;
using Models.Shared.Requests.Shared;
using Models.Shared.Responses.Group;
using Models.Shared.Responses.Shared;

namespace AnswerMe.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizeByIdentityServer]
    public class GroupController : ControllerBase
    {
        private readonly IJwtTokenRepository _jwtTokenRepository;
        private readonly IGroupRepository _groupRepository;

        public GroupController(IJwtTokenRepository jwtTokenRepository, IGroupRepository groupRepository)
        {
            _jwtTokenRepository = jwtTokenRepository;
            _groupRepository = groupRepository;
        }


        /// <summary>
        /// Get group by groupId
        /// </summary>
        /// <response code="200">Success: Get single group</response>
        /// <response code="404">NotFound: Group not found</response>
        /// <response code="403">AccessDenied: You don't have permission to access to this content</response>
        [ProducesResponseType(typeof(GroupResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpGet("{groupId:guid}")]
        public async Task<IActionResult> GetGroupAsync([FromRoute] Guid groupId)
        {
            var requestToken = _jwtTokenRepository.GetJwtToken();
            var loggedInUser = _jwtTokenRepository.ExtractUserDataFromToken(requestToken);
            var result = await _groupRepository.GetAsync(loggedInUser.id, groupId);

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
        /// Get logged in user group list
        /// </summary>
        /// <response code="200">Success: Get list of group</response>
        [ProducesResponseType(typeof(PagedListResponse<GroupResponse>), StatusCodes.Status200OK)]
        [HttpGet("List")]
        public async Task<IActionResult> GetGroupListAsync([FromQuery] PaginationRequest paginationRequest)
        {
            var requestToken = _jwtTokenRepository.GetJwtToken();
            var loggedInUser = _jwtTokenRepository.ExtractUserDataFromToken(requestToken);

            var result = await _groupRepository.GetListAsync(loggedInUser.id, paginationRequest);

            return Ok(result.AsSuccess.Value);
        }

        /// <summary>
        /// Get list of user in group
        /// </summary>
        /// <response code="200">Success: Get list of user that in a group</response>
        /// <response code="404">NotFound: Group not found</response>
        /// <response code="403">AccessDenied: You don't have permission to access to this content</response>
        [ProducesResponseType(typeof(PreviewGroupUserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpGet("{groupId:guid}/User/List")]
        public async Task<IActionResult> GetGroupUserListAsync([FromRoute] Guid groupId,
            [FromQuery] PaginationRequest paginationRequest)
        {
            var requestToken = _jwtTokenRepository.GetJwtToken();
            var loggedInUser = _jwtTokenRepository.ExtractUserDataFromToken(requestToken);

            var result = await _groupRepository.UserListAsync(loggedInUser.id, groupId, paginationRequest);

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
        /// Create group
        /// </summary>
        /// <response code="200">Success: Group Id</response>
        [ProducesResponseType(typeof(IdResponse), StatusCodes.Status200OK)]
        [HttpPost]
        public async Task<IActionResult> CreateGroupAsync(CreateGroupRequest request)
        {
            var requestToken = _jwtTokenRepository.GetJwtToken();
            var loggedInUser = _jwtTokenRepository.ExtractUserDataFromToken(requestToken);

            var result = await _groupRepository.CreateAsync(loggedInUser.id, request);
            return Ok(result.AsSuccess.Value);
        }


        /// <summary>
        /// Edit Group
        /// </summary>
        /// <response code="204">Success: No Content</response>
        /// <response code="403">AccessDenied: You don't have permission to access to this content</response>
        /// <response code="404">NotFound: Group not found</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPatch("{groupId:guid}")]
        public async Task<IActionResult> EditGroupAsync([FromRoute] Guid groupId, EditGroupRequest request)
        {
            var requestToken = _jwtTokenRepository.GetJwtToken();
            var loggedInUser = _jwtTokenRepository.ExtractUserDataFromToken(requestToken);

            var result = await _groupRepository.EditAsync(loggedInUser.id, groupId, request);

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
        /// Join User To Group
        /// </summary>
        /// <response code="200">Success: User added to group</response>
        /// <response code="403">AccessDenied: You don't have permission to access to this content</response>
        /// <response code="404">NotFound: Group or user not found</response>
        [ProducesResponseType(typeof(IdResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPost("{groupId:guid}/User/{userId:guid}/Join")]
        public async Task<IActionResult> JoinUserToGroupAsync([FromRoute] Guid groupId, [FromRoute] Guid userId)
        {
            var requestToken = _jwtTokenRepository.GetJwtToken();
            var loggedInUser = _jwtTokenRepository.ExtractUserDataFromToken(requestToken);

            var result = await _groupRepository.JoinUserAsync(loggedInUser.id, groupId, userId);

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
        /// Kick User From Group
        /// </summary>
        /// <response code="204">Success: User kicked</response>
        /// <response code="403">AccessDenied: You don't have permission to access to this content</response>
        /// <response code="404">NotFound: Group or user not found</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{groupId:guid}/User/{userId:guid}/Kick")]
        public async Task<IActionResult> KickUserFromGroupAsync([FromRoute] Guid groupId, [FromRoute] Guid userId)
        {
            var requestToken = _jwtTokenRepository.GetJwtToken();
            var loggedInUser = _jwtTokenRepository.ExtractUserDataFromToken(requestToken);

            var result = await _groupRepository.KickUserAsync(loggedInUser.id, groupId, userId);

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
        /// Set a User as Group Admin
        /// </summary>
        /// <param name="groupId">The unique identifier of the group in which to set the user as an admin.</param>
        /// <param name="userId">The unique identifier of the user to set as an admin.</param>
        /// <response code="200">OK: Returns the result of setting the user as an admin.</response>
        /// <response code="403">Forbidden: You don't have permission to set the user as an admin.</response>
        /// <response code="404">Not Found: The group or user was not found.</response>
        [HttpPost("{groupId:guid}/Admin/{userId:guid}")]
        [ProducesResponseType(typeof(IdResponse), StatusCodes.Status200OK)] // Specify the expected response type
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SetUserAsGroupAdminsAsync([FromRoute] Guid groupId, [FromRoute] Guid userId)
        {
            var requestToken = _jwtTokenRepository.GetJwtToken();
            var loggedInUser = _jwtTokenRepository.ExtractUserDataFromToken(requestToken);

            var result = await _groupRepository.SetUserAsAdminAsync(loggedInUser.id, groupId, userId);

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
        /// Remove a User from Group Admins
        /// </summary>
        /// <param name="groupId">The unique identifier of the group from which to remove the user as an admin.</param>
        /// <param name="userId">The unique identifier of the user to remove as an admin.</param>
        /// <response code="204">No Content: User removed from group admins successfully.</response>
        /// <response code="403">Forbidden: You don't have permission to remove the user as an admin.</response>
        /// <response code="404">Not Found: The group or user was not found.</response>
        [HttpDelete("{groupId:guid}/Admin/{userId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveUserFromGroupAdminsAsync(Guid groupId, Guid userId)
        {
            var requestToken = _jwtTokenRepository.GetJwtToken();
            var loggedInUser = _jwtTokenRepository.ExtractUserDataFromToken(requestToken);

            var result = await _groupRepository.RemoveUserFromAdminsAsync(loggedInUser.id, groupId, userId);

            if (result.IsSuccess)
            {
                return NoContent();
            }

            if (result.IsAccessDenied)
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }

            return NotFound();
        }


        /// <summary>
        /// Leave a group.
        /// </summary>
        /// <param name="groupId">The ID of the group to leave.</param>
        /// <returns>No content on success, BadRequest on failure.</returns>
        [HttpDelete("{groupId:guid}/Leave")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> LeaveGroupAsync([FromRoute] Guid groupId)
        {
            var requestToken = _jwtTokenRepository.GetJwtToken();
            var loggedInUser = _jwtTokenRepository.ExtractUserDataFromToken(requestToken);

            await _groupRepository.LeaveGroupAsync(loggedInUser.id, groupId);

            return NoContent();
        }
    }
}