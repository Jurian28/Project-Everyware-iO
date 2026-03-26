using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace SharedClassLibrary.Jwt;

public class JwtOptions
{
    public static readonly string SECRET_KEY = "JWT_SECRET_KEY";

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
