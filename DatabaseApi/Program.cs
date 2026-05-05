using DatabaseApi.Models;
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

// app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowAll");

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
        db.Database.Migrate();
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
