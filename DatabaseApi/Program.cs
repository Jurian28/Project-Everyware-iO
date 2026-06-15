using DatabaseApi.Models;
using DatabaseApi.Services;
using DatabaseApi.Services.Interfaces;
using DatabaseApi.Models.Seeders;
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
builder.Services.AddScoped<DatabaseSeeder>();
builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins($"{Environment.GetEnvironmentVariable("APP_URL")}:{Environment.GetEnvironmentVariable("MOBILE_APP_PORT")}") 
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (Environment.GetEnvironmentVariable("RUNNING_IN_DOCKER") == "true")
{
    string databasePassword = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? throw new InvalidOperationException("Database password environment variable not set.");
    connectionString = builder.Configuration.GetConnectionString("DockerConnection") + $"Password={databasePassword}";
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
    
builder.Services.AddScoped<ISessionService, SessionService>();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowAll");

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

if (Environment.GetEnvironmentVariable("RUNNING_IN_DOCKER") == "true")
{
    using IServiceScope scope = app.Services.CreateScope();
    ApplicationDbContext db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    try
    {
        Console.WriteLine("[DB] Migrating...");
        await db.Database.MigrateAsync();
        await SeedRoles(app);
        Console.WriteLine("[DB] Done Migrating...");
    }
    catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == 1801)
    {
        Console.WriteLine("[DB] Database already exists, skipping creation.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[DB] An error occurred while migrating the database: {ex.Message}");
        throw ex;
    }
    if (Environment.GetEnvironmentVariable("RUN_SEED") == "true")
    {
        Console.WriteLine("[DB] Seeding...");
        DatabaseSeeder seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
        await seeder.SeedAsync();
        Console.WriteLine("[DB] Done Seeding...");
    }
}

app.Run();

static async Task SeedRoles(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    foreach (var role in new[] { "Organiser", "Admin", "User", "Speaker" })
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}
