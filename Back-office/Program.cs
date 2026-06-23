using Back_office.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using SharedClassLibrary.Jwt;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:5000");

// Add services to the container.
builder.Services.AddScoped<EventContextFilter>();
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<EventContextFilter>();
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<JwtHandler>();

builder.Services.AddHttpClient("ApiClient")
    .AddHttpMessageHandler<JwtHandler>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer("Bearer", options =>
{
    JwtOptions.GetJwtOptions(options);
    var existingEvents = options.Events;

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = async ctx =>
        {
            if (existingEvents?.OnMessageReceived != null)
                await existingEvents.OnMessageReceived(ctx);

            if (string.IsNullOrEmpty(ctx.Token))
                ctx.Token = ctx.Request.Cookies["AccessToken"];
        },
        OnChallenge = ctx =>
        {
            ctx.HandleResponse();
            ctx.Response.Redirect("/Auth/Login");
            return Task.CompletedTask;
        },
        OnForbidden = ctx =>
        {
            ctx.Response.Redirect("/RoleManagement/No-Permission");
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

builder.Services.AddHttpClient("DatabaseApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]);
})
    .AddHttpMessageHandler<JwtHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapGet("/", () =>
{
    Console.WriteLine("[DEBUG] Root hit, redirecting to /Events");
    return Results.Redirect("/Events");
});
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Events}/{action=Index}/{id?}")
    .WithStaticAssets();
app.Run();
