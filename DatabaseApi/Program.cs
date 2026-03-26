using DatabaseApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedClassLibrary.Jwt;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:5000");

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<JwtHandler>();

builder.Services.AddHttpClient("ApiClient")
    .AddHttpMessageHandler<JwtHandler>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer("Bearer", JwtOptions.GetJwtOptions);

builder.Services.AddAuthorization();

string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (Environment.GetEnvironmentVariable("RUNNING_IN_DOCKER") == "true")
{
    string databasePassword = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? throw new InvalidOperationException("Database password environment variable not set.");
    connectionString = builder.Configuration.GetConnectionString("DockerConnection") + $"Password={databasePassword}";
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

if (Environment.GetEnvironmentVariable("RUNNING_IN_DOCKER") != "true")
{
    using IServiceScope scope = app.Services.CreateScope();
    ApplicationDbContext db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    db.Database.Migrate();
}

app.Run();
