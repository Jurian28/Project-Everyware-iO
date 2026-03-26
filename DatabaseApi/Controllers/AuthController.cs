using DatabaseApi.Models;
using DatabaseApi.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SharedClassLibrary.Jwt;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DatabaseApi.Controllers;

/// <summary>
/// Controller responsible for handling user authentication, including login, registration, logout, and token refreshing.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController(UserManager<User> userManager, ApplicationDbContext applicationDbContext) : Controller
{
    private static readonly string INCOMPLETE_CREDENTIALS_MESSAGE = "Email and password are required.";
    private static readonly string INVALID_CREDENTIALS_MESSAGE = "Invalid email or password.";
    private static readonly string ACCOUNT_ALREADY_EXISTS_MESSAGE = "An account with this email already exists.";

    private readonly UserManager<User> _userManager = userManager;
    private readonly ApplicationDbContext _applicationDbContext = applicationDbContext;

    /// <summary>
    /// Handles the login api endpoint.
    /// </summary>
    /// <param name="loginDto">The authentication data containing the user's email and password.</param>
    /// <returns>A http response based on the outcome of the login process, containing access and refresh tokens if successful.</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] AuthInputDto loginDto)
    {
        if (string.IsNullOrEmpty(loginDto.Email) || string.IsNullOrEmpty(loginDto.Password))
        {
            return BadRequest(INCOMPLETE_CREDENTIALS_MESSAGE);
        }

        User? user = await _userManager.FindByEmailAsync(loginDto.Email);

        if (user == null)
        {
            return Unauthorized(INVALID_CREDENTIALS_MESSAGE);
        }

        bool passwordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);

        if (!passwordValid)
        {
            return Unauthorized(INVALID_CREDENTIALS_MESSAGE);
        }

        string accessToken = await CreateJwtToken(user);
        string refreshToken = CreateRefreshToken();

        _applicationDbContext.RefreshTokens.Add(new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            Expires = DateTime.Now.AddDays(7)
        });

        await _applicationDbContext.SaveChangesAsync();

        return Ok(new
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
        });
    }

    /// <summary>
    /// Handles the registration api endpoint.
    /// </summary>
    /// <param name="registerDto">The registration data containing the new user's email and password.</param>
    /// <returns>An http response based on the outcome of the registration process, containing access and refresh tokens if successful.</returns>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] AuthInputDto registerDto)
    {
        if (string.IsNullOrEmpty(registerDto.Email) || string.IsNullOrEmpty(registerDto.Password))
        {
            return BadRequest(INCOMPLETE_CREDENTIALS_MESSAGE);
        }

        if (await _userManager.FindByEmailAsync(registerDto.Email) != null)
        {
            return BadRequest(ACCOUNT_ALREADY_EXISTS_MESSAGE);
        }

        User user = new()
        {
            UserName = registerDto.Email,
            Email = registerDto.Email
        };
        IdentityResult result = await _userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded)
        {
            return BadRequest(string.Join(" ", result.Errors.Select(e => e.Description)));
        }

        string accessToken = await CreateJwtToken(user);
        string refreshToken = CreateRefreshToken();

        _applicationDbContext.RefreshTokens.Add(new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            Expires = DateTime.Now.AddDays(7)
        });
        await _applicationDbContext.SaveChangesAsync();

        return StatusCode(201, new
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        });
    }

    /// <summary>
    /// Handles the logout api endpoint by revoking the user's existing refresh token.
    /// </summary>
    /// <returns>An HTTP OK response upon successful logout.</returns>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        string? refreshToken = Request.Cookies["RefreshToken"];

        if (!string.IsNullOrEmpty(refreshToken))
        {
            RefreshToken? storedRefreshToken = await _applicationDbContext.RefreshTokens.FirstOrDefaultAsync(token => token.Token == refreshToken);

            if (storedRefreshToken != null)
            {
                _applicationDbContext.RefreshTokens.Remove(storedRefreshToken);
                await _applicationDbContext.SaveChangesAsync();
            }
        }

        return Ok();
    }

    /// <summary>
    /// Handles the token refresh api endpoint by generating a new access and refresh token.
    /// </summary>
    /// <returns>An HTTP response containing the new tokens if successful, or Unauthorized if the refresh token is invalid or expired.</returns>
    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh()
    {
        string? refreshToken = Request.Cookies["RefreshToken"];

        if (string.IsNullOrEmpty(refreshToken))
        {
            return Unauthorized();
        }

        RefreshToken? storedRefreshToken = await _applicationDbContext.RefreshTokens.FirstOrDefaultAsync(token => token.Token == refreshToken);

        if (storedRefreshToken == null || storedRefreshToken.Expires < DateTime.Now)
        {
            return Unauthorized();
        }

        User? user = await _userManager.FindByIdAsync(storedRefreshToken.UserId);

        if (user == null)
        {
            return Unauthorized();
        }

        string newAccessToken = await CreateJwtToken(user);
        string newRefreshToken = CreateRefreshToken();

        _applicationDbContext.RefreshTokens.Remove(storedRefreshToken);
        _applicationDbContext.RefreshTokens.Add(new RefreshToken
        {
            Token = newRefreshToken,
            UserId = user.Id,
            Expires = DateTime.Now.AddDays(7)
        });

        await _applicationDbContext.SaveChangesAsync();

        return Ok(new
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
        });
    }

    /// <summary>
    /// Creates a JWT token for the user.
    /// </summary>
    /// <param name="user">The user to create the JWT token for.</param>
    /// <returns>The JWT token string.</returns>
    /// <exception cref="Exception">Throws if the environment variable for the secret key is not set.</exception>
    private async Task<string> CreateJwtToken(User user)
    {
        List<Claim> claims =
        [
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(ClaimTypes.NameIdentifier, user.Id)
        ];

        if (user.UserName != null)
        {
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
        }

        IList<string> roles = await _userManager.GetRolesAsync(user);

        foreach (string role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        string? secretKey = Environment.GetEnvironmentVariable(JwtOptions.SECRET_KEY) ?? throw new Exception($"Environment variable '{JwtOptions.SECRET_KEY}' is not set.");
        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(secretKey));
        SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha256);
        JwtSecurityToken token = new(
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Generates a cryptographically secure random refresh token.
    /// </summary>
    /// <returns>A base64 encoded string representing the refresh token.</returns>
    private static string CreateRefreshToken()
    {
        byte[] bytes = new byte[64];

        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(bytes);
        }

        return Convert.ToBase64String(bytes);
    }
}
