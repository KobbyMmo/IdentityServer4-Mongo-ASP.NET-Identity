using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IdentityServerSample.Models;
using Microsoft.Extensions.Logging;
using IdentityServerSample.Extensions;
using IdentityServer4.Services;

namespace IdentityServerSample
{
    using IdentityServerSample.Services;
    using Microsoft.AspNetCore.Identity.MongoDB;

    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public Startup(ILoggerFactory loggerFactory, IHostingEnvironment env)
        {
            var environmentVar = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (environmentVar == null)
            {
                environmentVar = env.EnvironmentName;
            }
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentVar}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ConfigurationOptions>(Configuration);
            services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
                 {
                     builder.AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                 }));
            services.AddSingleton<IProfileService, ProfileService>();
            services.AddMvc();

            services.AddIdentityServer(
                      options =>
                      {
                          options.Events.RaiseSuccessEvents = true;
                          options.Events.RaiseFailureEvents = true;
                          options.Events.RaiseErrorEvents = true;
                      }
                  )
                  .AddMongoRepository()
                  .AddMongoDbForAspIdentity<ApplicationUser, IdentityRole>(Configuration)
                  .AddClients()
                  .AddIdentityApiResources()
                  .AddPersistedGrants()
                .AddDeveloperSigningCredential();

            services.AddAuthentication(o =>
            {

                o.DefaultScheme = IdentityConstants.ApplicationScheme;
                o.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            });
            //.AddGoogle(options =>
            //{
            //    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
            //    options.ClientId = "";
            //    options.ClientSecret = "";
            //})
            // .AddFacebook(options =>
            // {
            //     options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
            //     options.AppSecret = "";
            //     options.SaveTokens = true;
            //     options.AppId = "365884523951157";
            // }).AddTwitter(options =>
            // {
            //     options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
            //     options.ConsumerKey = "";
            //     options.ConsumerSecret = "";
            //     options.RetrieveUserDetails = true;
            // });

            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<ISmsSender, SmsSender>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseIdentityServer();
            app.UseMongoDbForIdentityServer();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
