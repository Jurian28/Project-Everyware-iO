using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace SharedClassLibrary.Jwt;

/// <summary>
/// Provides configuration options and utilities for JSON Web Token (JWT) authentication.
/// </summary>
public class JwtOptions
{
    /// <summary>
    /// The name of the environment variable containing the jwt secret key.
    /// </summary>
    public static readonly string SECRET_KEY = "JWT_SECRET_KEY";

    /// <summary>
    /// Configures the provided <see cref="JwtBearerOptions"/> with token validation parameters 
    /// and an event handler to extract the JWT token from the "AccessToken" cookie.
    /// </summary>
    /// <param name="options">The <see cref="JwtBearerOptions"/> instance to configure.</param>
    /// <exception cref="InvalidOperationException">Thrown when the JWT secret key environment variable is not set.</exception>
    public static void GetJwtOptions(JwtBearerOptions options)
    {
        string secretKey = Environment.GetEnvironmentVariable(SECRET_KEY) ?? throw new InvalidOperationException("JWT secret key environment variable not set.");

        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        };

        options.Events = new JwtBearerEvents()
        {
            OnMessageReceived = context =>
            {
                string? token = context.Request.Cookies["AccessToken"];

                if (!string.IsNullOrEmpty(token))
                {
                    context.Token = token;
                }

                return Task.CompletedTask;
            }
        };
    }
}
