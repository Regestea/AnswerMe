using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IdentityServer.Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Models.Shared.OneOfTypes;
using Models.Shared.Requests.User;
using Models.Shared.Responses.Group;
using Models.Shared.Responses.Shared;

namespace IdentityServer.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public AuthController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }


        /// <summary>
        /// Register User
        /// </summary>
        /// <response code="200">Success: Id of Registered User</response>
        /// <response code="403">BadRequest: Error List</response>
        [ProducesResponseType(typeof(IdResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ValidationFailed>), StatusCodes.Status403Forbidden)]
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync(RegisterUserRequest request)
        {
            var result = await _userRepository.AddUserAsync(request);

            if (result.IsSuccess)
            {
                return Ok(result.AsSuccess.Value);
            }

            var errors = result.AsValidationFailureList;
            foreach (var validationFailed in errors)
            {
                ModelState.AddModelError(validationFailed.Field, validationFailed.Error);
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Register User
        /// </summary>
        /// <response code="200">Success: Token Response</response>
        /// <response code="404">BadRequest: User not found</response>
        [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync(LoginUserRequest request)
        {
            var result = await _userRepository.GetUserAsync(request);

            if (result.IsNotFound)
            {
                return NotFound();
            }

            var configuration = HttpContext.RequestServices.GetRequiredService<IConfiguration>();

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                configuration.GetValue<string>("JWT:SecretKey") ??
                throw new ArgumentNullException(nameof(configuration))));

            string hostUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";

            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha512);

            var userClaims = new List<Claim>
            {
                new Claim(nameof(result.AsSuccess.Value.id), result.AsSuccess.Value.id.ToString()),
                new Claim(nameof(result.AsSuccess.Value.IdName), result.AsSuccess.Value.IdName !),
                new Claim(nameof(result.AsSuccess.Value.PhoneNumber), result.AsSuccess.Value.PhoneNumber !),
            };

            var tokenOption = new JwtSecurityToken(
                issuer: hostUrl,
                claims: userClaims,
                expires: DateTime.UtcNow.AddDays(30),
                signingCredentials: signinCredentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOption);


            return Ok(new TokenResponse() { FieldName = "Login Token",Token = tokenString });
        }
    }
}