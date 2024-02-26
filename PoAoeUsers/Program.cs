using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PoAoeUsers.Components;
using PoAoeUsers.Components.Account;
using PoAoeUsers.Data;
using PoAoeUsers.Models;
using PoAoeUsers.Services;

namespace PoAoeUsers
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            _ = builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            _ = builder.Services.AddCascadingAuthenticationState();
            _ = builder.Services.AddScoped<IdentityUserAccessor>();
            _ = builder.Services.AddScoped<IdentityRedirectManager>();
            _ = builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

            _ = builder.Services.AddAuthentication(options =>
                {
                    options.DefaultScheme = IdentityConstants.ApplicationScheme;
                    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
                })
                .AddIdentityCookies();




            // Bind Google settings
            _ = builder.Services.Configure<GoogleAuthConfig>(builder.Configuration.GetSection("Google"));

            _ = builder.Services.AddAuthentication().AddGoogle(googleOptions =>
            {
                // Resolve Google configuration options
                GoogleAuthConfig? googleConfig = builder.Configuration.GetSection("Google").Get<GoogleAuthConfig>();
                googleOptions.ClientId = googleConfig.ClientId;
                googleOptions.ClientSecret = googleConfig.ClientSecret;
                googleOptions.SignInScheme = IdentityConstants.ExternalScheme;
            });







            string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            _ = builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connectionString));
            _ = builder.Services.AddDatabaseDeveloperPageExceptionFilter();








            _ = builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();





            _ = builder.Services.AddServerSideBlazor().AddCircuitOptions(options => { options.DetailedErrors = true; });




            string? sendGridApiKey = builder.Configuration["SendGridKey"];
            _ = builder.Services.AddSingleton<IEmailSender<ApplicationUser>>(provider => new IdentityNoOpEmailSender(sendGridApiKey));





            _ = builder.Services.AddScoped<ListItemService>();
            _ = builder.Services.AddHttpContextAccessor();
            _ = builder.Services.AddHttpClient();


            WebApplication app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                _ = app.UseMigrationsEndPoint();
            }
            else
            {
                _ = app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                _ = app.UseHsts();
            }

            _ = app.UseHttpsRedirection();

            _ = app.UseStaticFiles();
            _ = app.UseAntiforgery();

            _ = app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            // Add additional endpoints required by the Identity /Account Razor components.
            _ = app.MapAdditionalIdentityEndpoints();

            app.Run();
        }
    }
}
