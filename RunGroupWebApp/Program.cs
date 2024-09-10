using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RunGroopWebApp.Data;
using RunGroupWebApp.Data;
using RunGroupWebApp.Helpers;
using RunGroupWebApp.Interfaces;
using RunGroupWebApp.Models;
using RunGroupWebApp.Repository;
using RunGroupWebApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IClubRepository, ClubRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAppUserClubRepository, AppUserClubRepository>();

builder.Configuration.AddUserSecrets<Program>();
builder.Services.Configure<AzureStorageConfig>(builder.Configuration.GetSection("AzureStorageConfig"));
builder.Services.AddScoped<IAzureBlobService, AzureBlobService>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptions => sqlServerOptions
            .EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null
            )
    );
});


builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();

builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddMemoryCache();
builder.Services.AddSession();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();

builder.Services.Configure<GoogleAuthConfig>(builder.Configuration.GetSection("Authentication:Google"));

builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        var googleConfig = builder.Configuration.GetSection("Authentication:Google").Get<GoogleAuthConfig>();
        options.ClientId = googleConfig.ClientId;
        options.ClientSecret = googleConfig.ClientSecret;
        options.Scope.Add("profile"); // Add this line
        options.Scope.Add("email");   // You probably already have this
        options.ClaimActions.MapJsonKey("picture", "picture", "url");

        options.Events = new OAuthEvents
        {
            OnRemoteFailure = context =>
            {
                var exception = context.Failure;
                if (exception is AuthenticationFailureException authFailure &&
                    authFailure.Message.Contains("Access was denied by the resource owner or by the remote server"))
                {
                    context.Response.Redirect("/Account/AccessDenied");
                    context.HandleResponse(); // Suppress the default error handler
                }
                return Task.CompletedTask;
            }
        };
    });

var app = builder.Build();

if (args.Length == 1 && args[0].ToLower() == "seeddata")
{
    //await Seed.SeedUsersAndRolesAsync(app);
    Seed.SeedData(app);
}


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
app.UseSession();

app.MapControllerRoute( //Tags Helper
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
