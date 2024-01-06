using AnswerMe.Application.Common.Interfaces;
using IdentityServer.Shared.Client.Attributes;
using IdentityServer.Shared.Client.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Shared.Requests.Shared;
using Models.Shared.Responses.Message;
using Models.Shared.Responses.PrivateRoom;
using Models.Shared.Responses.Shared;

namespace AnswerMe.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizeByIdentityServer]
    public class PrivateRoomController : ControllerBase
    {
        private readonly IPrivateRoomRepository _privateRoomRepository;
        private readonly IJwtTokenRepository _jwtTokenRepository;

        public PrivateRoomController(IPrivateRoomRepository privateRoomRepository, IJwtTokenRepository jwtTokenRepository)
        {
            _privateRoomRepository = privateRoomRepository;
            _jwtTokenRepository = jwtTokenRepository;
        }


        /// <summary>
        /// Get a list of private rooms.
        /// </summary>
        /// <param name="paginationRequest">Pagination parameters.</param>
        /// <response code="200">OK: The list of private rooms.</response>
        [HttpGet]
        [ProducesResponseType(typeof(PagedListResponse<PrivateRoomResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetListAsync([FromQuery] PaginationRequest paginationRequest)
        {
            var requestToken = _jwtTokenRepository.GetJwtToken();
            var loggedInUser = _jwtTokenRepository.ExtractUserDataFromToken(requestToken);

            var result = await _privateRoomRepository.GetListAsync(loggedInUser.id, paginationRequest);

            return Ok(result.AsSuccess.Value);
        }

        /// <summary>
        /// Create a private room with the specified contact.
        /// </summary>
        /// <param name="contactId">The ID of the contact to create the private room with.</param>
        /// <returns>
        /// Returns OK status with the created private room details if successful.
        /// Returns NotFound if the operation fails.
        /// </returns>
        /// <response code="200">Returns Id of private room.</response>
        /// <response code="404">If the operation fails, returns a not found status.</response>
        [HttpPost("{contactId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IdResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateAsync(Guid contactId)
        {
            var requestToken = _jwtTokenRepository.GetJwtToken();
            var loggedInUser = _jwtTokenRepository.ExtractUserDataFromToken(requestToken);

            var result =await _privateRoomRepository.CreateAsync(loggedInUser.id, contactId);

            if (result.IsSuccess)
            {
                return Ok(result.AsSuccess.Value);
            }

            return NotFound();
        }

        /// <summary>
        /// Get a private room by ID.
        /// </summary>
        /// <param name="roomId">The unique identifier of the private room to retrieve.</param>
        /// <response code="200">OK: The private room.</response>
        /// <response code="403">Forbidden: The user is not allowed to access the private room.</response>
        /// <response code="404">Not Found: The private room or user could not be found.</response>
        [HttpGet("{roomId:guid}")]
        [ProducesResponseType(typeof(PrivateRoomResponse),StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAsync(Guid roomId)
        {
            var requestToken = _jwtTokenRepository.GetJwtToken();
            var loggedInUser = _jwtTokenRepository.ExtractUserDataFromToken(requestToken);

            var result = await _privateRoomRepository.GetAsync(loggedInUser.id, roomId);

            if (result.IsSuccess)
            {
                return Ok(result.AsSuccess.Value);
            }

            if (result.IsAccessDenied)
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }

            return NotFound();
        }

        /// <summary>
        /// Get the last seen status of a user in a private room.
        /// </summary>
        /// <param name="roomId">The ID of the private room.</param>
        /// <param name="userId">The ID of the user to query the last seen status.</param>
        /// <returns>Last seen status on success, 403 (Forbidden) if unauthorized, 404 (Not Found) if not found.</returns>
        [HttpGet("{roomId:guid}/User/{userId:guid}/LastSeen")]
        [ProducesResponseType(typeof(RoomLastSeenResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetLastSeenStatusAsync([FromRoute] Guid roomId, [FromRoute] Guid userId)
        {
            var requestToken = _jwtTokenRepository.GetJwtToken();
            var loggedInUser = _jwtTokenRepository.ExtractUserDataFromToken(requestToken);

            var result = await _privateRoomRepository.GetLastSeenAsync(loggedInUser.id, roomId, userId);

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

    }
}
