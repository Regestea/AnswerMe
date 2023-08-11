using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IdentityServer.Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Models.Shared.Requests.User;
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


        [HttpPost("/Register")]
        public async Task<IActionResult> Register(RegisterUserRequest request)
        {
            var result = await _userRepository.AddUser(request);

            if (result.IsT0)
            {
                return Ok(result.AsT0.Value);
            }

            if (result.IsT1)
            {
                var errors = result.AsT1;
                foreach (var validationFailed in errors)
                {
                    ModelState.AddModelError(validationFailed.Field,validationFailed.Error);
                }

                return BadRequest(ModelState);
            }


            return StatusCode(500, result.AsT2.Value);
        }

        [HttpPost("/Login")]
        public async Task<IActionResult> Login(LoginUserRequest request)
        {
            var result = await _userRepository.GetUser(request);

            if (result.IsT1)
            {
                ModelState.AddModelError(nameof(request.PhoneNumber),"this phone number not found");
                ModelState.AddModelError(nameof(request.Password),"maybe the password is wrong");

                return BadRequest(ModelState);
            }

            var configuration = HttpContext.RequestServices.GetRequiredService<IConfiguration>();

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                configuration.GetValue<string>("JWT:SecretKey") ??
                throw new ArgumentNullException(nameof(configuration))));

            string hostUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";

            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha512);

            var userClaims = new List<Claim>
            {
                new Claim(nameof(result.AsT0.Value.id), result.AsT0.Value.id.ToString()),
                new Claim(nameof(result.AsT0.Value.IdName), result.AsT0.Value.IdName !),
                new Claim(nameof(result.AsT0.Value.PhoneNumber), result.AsT0.Value.PhoneNumber !),
            };

            var tokenOption = new JwtSecurityToken(
                issuer: hostUrl,
                claims: userClaims,
                expires: DateTime.UtcNow.AddDays(30),
                signingCredentials: signinCredentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOption);


            return Ok(new TokenResponse() { Token = tokenString });
        }
    }
}