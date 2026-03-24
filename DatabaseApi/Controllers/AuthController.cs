using DatabaseApi.Models;
using DatabaseApi.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DatabaseApi.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/[controller]")]
public class AuthController(UserManager<User> userManager) : Controller
{
    private static readonly string SECRET_TOKEN_ENV_NAME = "JWT_SECRET_KEY";
    private static readonly string INCOMPLETE_CREDENTIALS_MESSAGE = "Email and password are required.";
    private static readonly string INVALID_CREDENTIALS_MESSAGE = "Invalid email or password.";

    private readonly UserManager<User> _userManager = userManager;

    /// <summary>
    /// Handles the login api endpoint.
    /// </summary>
    /// <param name="email">The email of the user that is trying to log in.</param>
    /// <param name="password">The password of the user that is trying to log in.</param>
    /// <returns>A http response based on the outcome of the login process.</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AuthDto authDto)
    {
        if (string.IsNullOrEmpty(authDto.Email) || string.IsNullOrEmpty(authDto.Password))
        {
            return BadRequest(INCOMPLETE_CREDENTIALS_MESSAGE);
        }

        User? user = await _userManager.FindByEmailAsync(authDto.Email);

        if (user == null)
        {
            return Unauthorized(INVALID_CREDENTIALS_MESSAGE);
        }

        bool passwordValid = await _userManager.CheckPasswordAsync(user, authDto.Password);

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
    public async Task<IActionResult> Register([FromBody] AuthDto registerDto)
    {
        if (string.IsNullOrEmpty(registerDto.Email) || string.IsNullOrEmpty(registerDto.Password))
        {
            return BadRequest(INCOMPLETE_CREDENTIALS_MESSAGE);
        }

        if (await _userManager.FindByEmailAsync(registerDto.Email) != null)
        {
            return BadRequest("An account with this email already exists.");
        }

        User user = new()
        {
            UserName = registerDto.Email,
            Email = registerDto.Email
        };
        IdentityResult result = await _userManager.CreateAsync(user);

        if (!result.Succeeded)
        {
            return BadRequest();
        }

        await _userManager.AddPasswordAsync(user, registerDto.Password);

        return Created();
    }

    /// <summary>
    /// Creates a JWT token for the user.
    /// </summary>
    /// <param name="user">The user to create the JWT token for.</param>
    /// <returns>The JWT token.</returns>
    /// <exception cref="Exception">Throws if the environment variable for the secret key is not set.</exception>
    private static string CreateJwtToken(User user)
    {
        Claim[] claims =
        [
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
        ];

        string? secretKey = Environment.GetEnvironmentVariable(SECRET_TOKEN_ENV_NAME) ?? throw new Exception($"Environment variable '{SECRET_TOKEN_ENV_NAME}' is not set.");
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
