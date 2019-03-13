using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace OAuthSocialSignInSample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // Add Cookie settings
            services.AddAuthentication(options =>
                {
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                // Add Cookie settings
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.LoginPath = "/account/login";
                    options.LogoutPath = "/account/logout";
                    options.SlidingExpiration = true;
                })
                
                // Add GitHub authentication
                .AddGitHub("Github", options =>
                {
                    options.ClientId = ""; // client id from registering github app
                    options.ClientSecret =""; // client secret from registering github app
                    options.Scope.Add("user:email"); // add additional scope to obtain email address
                    options.Events = new OAuthEvents
                    {
                        OnCreatingTicket = OnCreatingGitHubTicket()
                    }; // Event to capture when the authentication ticket is being created
                })


                // Add Google Authentication
                .AddGoogle("Google", options =>
                {
                    options.ClientId = ""; // Google Client id 
                    options.ClientSecret = ""; // Google Client secret
                    options.UserInformationEndpoint = "https://www.googleapis.com/oauth2/v2/userinfo";
                    options.ClaimActions.Clear();
                    options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
                    options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
                    options.ClaimActions.MapJsonKey(ClaimTypes.GivenName, "given_name");
                    options.ClaimActions.MapJsonKey(ClaimTypes.Surname, "family_name");
                    options.ClaimActions.MapJsonKey("urn:google:profile", "link");
                    options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
                    options.Events = new OAuthEvents
                    {
                        OnCreatingTicket = OnCreatingGoogleTicket()
                    }; // Event to capture when the authentication ticket is being created
                })

                // Add Slack Authentication
                .AddSlack("Slack", options =>
                {
                    options.ClientId = ""; // Slack Client Id
                    options.ClientSecret = ""; //Slack Client Secret
                    options.Events = new OAuthEvents {OnCreatingTicket = OnCreatingSlackTicket()};
                });
                



            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            // Enable authentication
            app.UseCookiePolicy();
            app.UseAuthentication();
            
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc();
        }


        private static Func<OAuthCreatingTicketContext, Task> OnCreatingGitHubTicket()
        {
            return async context =>
            {
                var fullName = context.Identity.FindFirst("urn:github:name").Value;
                var email = context.Identity.FindFirst(ClaimTypes.Email).Value;


                //Todo: Add logic here to save info into database

                // this Task.FromResult is purely to make the code compile as it requires a Task result
                await Task.FromResult(true);
            };
        }

        private static Func<OAuthCreatingTicketContext, Task> OnCreatingGoogleTicket()
        {
            return async context =>
            {
                var firstName = context.Identity.FindFirst(ClaimTypes.GivenName).Value;
                var lastName = context.Identity.FindFirst(ClaimTypes.Surname)?.Value;
                var email = context.Identity.FindFirst(ClaimTypes.Email).Value;


                //Todo: Add logic here to save info into database

                // this Task.FromResult is purely to make the code compile as it requires a Task result
                await Task.FromResult(true);
            };
        }

        private static Func<OAuthCreatingTicketContext, Task> OnCreatingSlackTicket()
        {
            return async context =>
            {
                var fullName = context.Identity.FindFirst(ClaimTypes.Name).Value;

                //Todo: Add logic here to save info into database

                // this Task.FromResult is purely to make the code compile as it requires a Task result
                await Task.FromResult(true);
            };
        }
    }
}
