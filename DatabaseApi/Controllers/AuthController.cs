using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DatabaseApi.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly UserManager<IdentityUser> _userManager;

        private static readonly string SECRET_TOKEN_ENV_NAME = "JWT_SECRET_KEY";
        private static readonly string INCOMPLETE_CREDENTIALS_MESSAGE = "Email and password are required.";
        private static readonly string INVALID_CREDENTIALS_MESSAGE = "Invalid email or password.";

        public AuthController(IUserStore<IdentityUser> userStore, UserManager<IdentityUser> userManager)
        {
            _userStore = userStore;
            _userManager = userManager;
        }

        /// <summary>
        /// Handles the login api endpoint.
        /// </summary>
        /// <param name="email">The email of the user that is trying to log in.</param>
        /// <param name="password">The password of the user that is trying to log in.</param>
        /// <returns>A http response based on the outcome of the login process.</returns>
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return BadRequest(INCOMPLETE_CREDENTIALS_MESSAGE);
            }

            IdentityUser? user = await _userStore.FindByNameAsync(email, CancellationToken.None);

            if (user == null)
            {
                return Unauthorized(INVALID_CREDENTIALS_MESSAGE);
            }

            bool passwordValid = await _userManager.CheckPasswordAsync(user, password);

            if (!passwordValid)
            {
                return Unauthorized(INVALID_CREDENTIALS_MESSAGE);
            }

            try
            {
                string token = CreateJwtToken(user);

                return Ok(
                    new
                    {
                        Token = token
                    }
                );
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while generating the token.");
            }
        }

        /// <summary>
        /// Handles the registration api endpoint.
        /// </summary>
        /// <param name="email">The email of the new account.</param>
        /// <param name="password">The password of the new account.</param>
        /// <returns>An http response based on the outcome of the registration process.</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return BadRequest(INCOMPLETE_CREDENTIALS_MESSAGE);
            }

            IdentityUser user = new()
            {
                UserName = email,
                Email = email
            };
            IdentityResult result = await _userStore.CreateAsync(user, CancellationToken.None);

            if (!result.Succeeded)
            {
                return BadRequest();
            }

            await _userManager.AddPasswordAsync(user, password);

            return Created();
        }

        /// <summary>
        /// Creates a JWT token for the user.
        /// </summary>
        /// <param name="user">The user to create the JWT token for.</param>
        /// <returns>The JWT token.</returns>
        /// <exception cref="Exception">Throws if the environment variable for the secret key is not set.</exception>
        private static string CreateJwtToken(IdentityUser user)
        {
            string? secretKey = Environment.GetEnvironmentVariable(SECRET_TOKEN_ENV_NAME);

            if (secretKey == null)
            {
                throw new Exception($"Environment variable '{SECRET_TOKEN_ENV_NAME}' is not set.");
            }

            Claim[] claims =
            [
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            ];

            SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(secretKey));
            SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha256);
            JwtSecurityToken token = new(
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
