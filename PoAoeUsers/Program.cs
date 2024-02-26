using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
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
            builder.Services.Configure<GoogleAuthConfig>(builder.Configuration.GetSection("Google"));

            builder.Services.AddAuthentication().AddGoogle(googleOptions =>
            {
                // Resolve Google configuration options
                var googleConfig = builder.Configuration.GetSection("Google").Get<GoogleAuthConfig>();
                googleOptions.ClientId = googleConfig.ClientId;
                googleOptions.ClientSecret = googleConfig.ClientSecret;
                googleOptions.SignInScheme = IdentityConstants.ExternalScheme;
            });


            //builder.Services.AddAuthentication().AddGoogle(googleOptions =>
            //{
            //    googleOptions.ClientId = "498983659463-9ltus4nv5ghlpobk4kd38hd5dfkd4hgi.apps.googleusercontent.com";
            //    googleOptions.ClientSecret = "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";
            //    googleOptions.SignInScheme = IdentityConstants.ExternalScheme;
            //});

            //  builder.Services.AddScoped<IGoogleAuthService, GoogleAuthService>();
            // builder.Services.Configure<GoogleAuthConfig>(builder.Configuration.GetSection("Google"));

            // 498983659463 - 9ltus4nv5ghlpobk4kd38hd5dfkd4hgi.apps.googleusercontent.com

            // GOCSPX - 81Ykc3ioQEBNShsqxpPwmkP49b - P

            //string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            //_ = builder.Services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseSqlServer(connectionString));
            //_ = builder.Services.AddDatabaseDeveloperPageExceptionFilter();






            string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            _ = builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connectionString));
            _ = builder.Services.AddDatabaseDeveloperPageExceptionFilter();








            _ = builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();





            _ = builder.Services.AddServerSideBlazor().AddCircuitOptions(options => { options.DetailedErrors = true; });


            var sendGridApiKey = builder.Configuration["SendGridKey"];


            //builder.Services.AddSingleton<IEmailSender>(new RealEmailSender(sendGridApiKey));

            //  builder.Services.AddTransient<IEmailSender, RealEmailSender>();
          //  builder.Services.AddTransient<IEmailSender>(provider => new RealEmailSender(sendGridApiKey));

              _ = builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
             //   _ = builder.Services.AddScoped<IEmailSender<ApplicationUser>, RealEmailSender>();




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
