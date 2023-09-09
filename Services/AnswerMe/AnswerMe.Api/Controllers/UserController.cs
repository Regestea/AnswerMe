using IdentityServer.Api.Repositories;
using IdentityServer.Shared.Client.Attributes;
using IdentityServer.Shared.Client.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models.Shared.Requests.User;

namespace AnswerMe.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;

        private readonly IJwtTokenRepository _jwtTokenRepository;

        public UserController(IUserRepository userRepository, IJwtTokenRepository jwtTokenRepository)
        {
            _userRepository = userRepository;
            _jwtTokenRepository = jwtTokenRepository;
        }


        [HttpGet]
        [AuthorizeByIdentityServer]
        public async Task<IActionResult> GetUser()
        {
            var requestToken = _jwtTokenRepository.GetJwtToken();
            var loggedInUser = _jwtTokenRepository.ExtractUserDataFromToken(requestToken);

            var userResponse = await _userRepository.GetUserByIdAsync(loggedInUser.id);

            if (userResponse.IsT1)
            {
                var addUserResponse = await _userRepository.AddUserAsync(new AddUserDto()
                {
                    id = loggedInUser.id,
                    IdName = loggedInUser.IdName,
                    PhoneNumber = loggedInUser.PhoneNumber
                });
                userResponse = await _userRepository.GetUserByIdAsync(loggedInUser.id);
            }

            return Ok(userResponse.AsT0);
        }


        [HttpGet("{id}")]
        [AuthorizeByIdentityServer]
        public async Task<IActionResult> GetUserById([FromRoute] Guid id)
        {
            var userResponse = await _userRepository.GetUserByIdAsync(id);

            if (userResponse.IsT0)
            {
                return Ok(userResponse.AsT0);
            }

            return BadRequest(userResponse.AsT1);

        }

        [HttpPatch]
        [AuthorizeByIdentityServer]
        public async Task<IActionResult> EditUser(EditUserRequest request)
        {
            var requestToken = _jwtTokenRepository.GetJwtToken();
            var loggedInUser = _jwtTokenRepository.ExtractUserDataFromToken(requestToken);

             await _userRepository.EditUserAsync(loggedInUser.id, request);

             return NoContent();
        }


    }
}
