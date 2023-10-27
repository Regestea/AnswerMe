﻿using AnswerMe.Application.Common.Interfaces;
using AnswerMe.Application.DTOs;
using IdentityServer.Shared.Client.Attributes;
using IdentityServer.Shared.Client.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models.Shared.Requests.User;
using Models.Shared.Responses.Shared;

namespace AnswerMe.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizeByIdentityServer]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;

        private readonly IJwtTokenRepository _jwtTokenRepository;

        public UserController(IUserRepository userRepository, IJwtTokenRepository jwtTokenRepository)
        {
            _userRepository = userRepository;
            _jwtTokenRepository = jwtTokenRepository;
        }


        /// <summary>
        /// Get the current user's profile.
        /// </summary>
        /// <response code="200">OK: The user's profile information.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUser()
        {
            var requestToken = _jwtTokenRepository.GetJwtToken();
            var loggedInUser = _jwtTokenRepository.ExtractUserDataFromToken(requestToken);

            var userResponse = await _userRepository.GetByIdAsync(loggedInUser.id);

            if (userResponse.IsT1)
            {
                var addUserResponse = await _userRepository.AddAsync(new AddUserDto()
                {
                    id = loggedInUser.id,
                    IdName = loggedInUser.IdName,
                    PhoneNumber = loggedInUser.PhoneNumber
                });
                userResponse = await _userRepository.GetByIdAsync(loggedInUser.id);
            }

            return Ok(userResponse.AsT0);
        }

        /// <summary>
        /// Get a user by ID.
        /// </summary>
        /// <param name="id">The unique identifier of the user to retrieve.</param>
        /// <response code="200">OK: The user's profile information.</response>
        /// <response code="400">Bad Request: Invalid user or user not found.</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetUserById([FromRoute] Guid id)
        {
            var userResponse = await _userRepository.GetByIdAsync(id);

            if (userResponse.IsT0)
            {
                return Ok(userResponse.AsT0);
            }

            return BadRequest(userResponse.AsT1);
        }

        /// <summary>
        /// Edit the current user's profile.
        /// </summary>
        /// <response code="204">No Content: User profile updated successfully.</response>
        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> EditUser(EditUserRequest request)
        {
            var requestToken = _jwtTokenRepository.GetJwtToken();
            var loggedInUser = _jwtTokenRepository.ExtractUserDataFromToken(requestToken);

            await _userRepository.EditAsync(loggedInUser.id, request);

            return NoContent();
        }

        /// <summary>
        /// Check if a user is online.
        /// </summary>
        /// <param name="userId">The ID of the user to check online status.</param>
        /// <returns>True if the user is online; otherwise, false. 404 (Not Found) if the user doesn't exist.</returns>
        [HttpGet("{userId:guid}/IsOnline")]
        [ProducesResponseType(typeof(BooleanResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> IsOnline([FromRoute] Guid userId)
        {
            var result = await _userRepository.IsOnlineAsync(userId);

            if (result.IsT0)
            {
                return Ok(result.AsT0.Value);
            }

            return NotFound();
        }

        /// <summary>
        /// Check if a user exists.
        /// </summary>
        /// <param name="userId">The ID of the user to check existence.</param>
        /// <returns>True if the user exists; otherwise, false.</returns>
        [HttpGet("{userId:guid}/Exist")]
        [ProducesResponseType(typeof(BooleanResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Exist([FromRoute] Guid userId)
        {
            var result = await _userRepository.ExistAsync(userId);

            return Ok(result.AsT0.Value);
        }
    }
}