using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using TextEditorMVC.Services;

namespace TextEditorMVC
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

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddSingleton<IWordLoader, WordLoader>();
            services.AddScoped<IWordRecognition, WordRecognition>();
            services.AddScoped<IUserFunctionality, UserFunctionality>();
            services.AddScoped<ITextRecognition, TextRecognition>();


            var key = Configuration.GetSection("PrivateKey").Value;

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(c => c.Events = new CookieAuthenticationEvents
                {
                    OnValidatePrincipal = context =>
                    {
                        if (ValidateToken(context.Principal.Identity.Name, key, out JwtSecurityToken token))
                        {
                            var userService = context.HttpContext.RequestServices.GetRequiredService<IAuthenticationService>();

                            var username = token.Claims.First(claim => claim.Type == "unique_name").Value;
                            var user = userService.GetUser(username);
                            ;
                            if (user == null)
                            {
                                context.RejectPrincipal();
                                return Task.CompletedTask;
                            }
                            return Task.CompletedTask;
                        }
                        context.RejectPrincipal();
                        return Task.CompletedTask;
                    }
                });
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
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private static bool ValidateToken(string authToken, string key, out JwtSecurityToken token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = GetValidationParameters(key);

            try
            {
                tokenHandler.ValidateToken(authToken, validationParameters, out SecurityToken resultToken);
                token = (JwtSecurityToken)resultToken;
                return true;
            }
            catch /*(Exception exception)*/
            {
                token = null;
                return false;
            }
        }

        private static TokenValidationParameters GetValidationParameters(string key)
        {
            return new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
            };
        }
    }
}
