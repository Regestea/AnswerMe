﻿using System.ComponentModel.DataAnnotations;
using AnswerMe.Application.Common.Interfaces;
using IdentityServer.Shared.Client.Attributes;
using IdentityServer.Shared.Client.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Models.Shared.OneOfTypes;
using Models.Shared.Requests.Group;
using Models.Shared.Responses.Group;
using Models.Shared.Responses.Shared;

namespace AnswerMe.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizeByIdentityServer]
    public class GroupInviteController : ControllerBase
    {
        private readonly IGroupInviteRepository _groupInviteRepository;
        private readonly IJwtTokenRepository _jwtTokenRepository;

        public GroupInviteController(IGroupInviteRepository groupInviteRepository, IJwtTokenRepository jwtTokenRepository)
        {
            _groupInviteRepository = groupInviteRepository;
            _jwtTokenRepository = jwtTokenRepository;
        }


        /// <summary>
        /// Get Group Preview based on invite token
        /// </summary>
        /// <param name="inviteToken">The invite token to preview the group.</param>
        /// <response code="200">OK: Returns the result of the group preview based on the invite token.</response>
        /// <response code="404">Not Found: The group preview could not be found.</response>
        [ProducesResponseType(typeof(GroupResponse), StatusCodes.Status200OK)] // Specify the expected response type
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{inviteToken}")]
        public async Task<IActionResult> GetGroupPreview([FromRoute] string inviteToken)
        {
            var requestToken = _jwtTokenRepository.GetJwtToken();
            var loggedInUser = _jwtTokenRepository.ExtractUserDataFromToken(requestToken);

            var result = await _groupInviteRepository.GetGroupPreviewAsync(inviteToken);

            if (result.IsT0)
            {
                return Ok(result);
            }

            return NotFound();
        }

        /// <summary>
        /// Create an Invite Token for a Group
        /// </summary>
        /// <param name="groupId">The unique identifier of the group for which to create an invite token.</param>
        /// <param name="inviteTokenRequest">The request object for creating the invite token.</param>
        /// <response code="200">OK: Returns the created invite token.</response>
        /// <response code="400">Bad Request: Invalid input data.</response>
        /// <response code="403">Forbidden: You don't have permission to create an invite token.</response>
        /// <response code="404">Not Found: The group or user was not found.</response>
        [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)] // Specify the expected response type
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Specify another expected response type
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPost("{groupId:guid}")]
        public async Task<IActionResult> Create([FromRoute] Guid groupId, [FromBody] CreateInviteTokenRequest inviteTokenRequest)
        {
            var requestToken = _jwtTokenRepository.GetJwtToken();
            var loggedInUser = _jwtTokenRepository.ExtractUserDataFromToken(requestToken);

            var result = await _groupInviteRepository.CreateAsync(loggedInUser.id, groupId, inviteTokenRequest);

            if (result.IsT0)
            {
                return Ok(result.AsT0.Value);
            }

            if (result.IsT2)
            {
                ModelState.AddModelError(result.AsT2.Field, result.AsT2.Error);
                return BadRequest(ModelState);
            }

            if (result.IsT4)
            {
                return StatusCode(403);
            }

            return NotFound();
        }

        /// <summary>
        /// Join a Group using an Invite Token
        /// </summary>
        /// <param name="inviteToken">The invite token to join the group.</param>
        /// <response code="200">OK: Returns the result of joining the group using the invite token.</response>
        /// <response code="400">Bad Request: Invalid input data.</response>
        /// <response code="404">Not Found: The invite token or group could not be found.</response>
        [HttpPost("Token/{inviteToken}/Join")]
        [ProducesResponseType(typeof(IdResponse), StatusCodes.Status200OK)] // Specify the expected response type
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Specify another expected response type
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> JoinGroup([FromRoute] string inviteToken)
        {
            var requestToken = _jwtTokenRepository.GetJwtToken();
            var loggedInUser = _jwtTokenRepository.ExtractUserDataFromToken(requestToken);

            var result = await _groupInviteRepository.JoinGroupAsync(loggedInUser.id, inviteToken);

            if (result.IsT0)
            {
                return Ok(result.AsT0.Value);
            }

            if (result.IsT2)
            {
                ModelState.AddModelError(result.AsT2.Field, result.AsT2.Error);
                return BadRequest(ModelState);
            }

            return NotFound();
        }

    }
}