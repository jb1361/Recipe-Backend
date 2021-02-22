using System;
using System.IO;
using System.Reflection;
using System.Text;
using AutoMapper;
using CsgoHoldem.Api.Authorization.Handlers;
using CsgoHoldem.Api.Authorization.Requirements;
using CsgoHoldem.Api.Config;
using CsgoHoldem.Api.Controllers;
using CsgoHoldem.Api.Models.Context;
using CsgoHoldem.Api.Services;
using CsgoHoldem.Api.Util;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using CsgoHoldem.Api.Middleware;

namespace CsgoHoldem.Api
{
    public class Startup
    {
        readonly string CorsPolicyName = "_CorsPolicy";
        #pragma warning disable 618
        
        
        #pragma warning restore 618
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var appSettings = ConfigureAppSettings(services);
            DataDogTracerServiceProvider.ProvideTracer(appSettings);
            services.AddSingleton<ProblemDetailsFactory, CustomProblemDetailsFactory>();
            services.AddControllers()
                .AddNewtonsoftJson()
                .AddFluentValidation(fv => {
                    fv.ImplicitlyValidateChildProperties = true;
                });
            ConfigureDatabase(services, appSettings);
            ConfigureAuthentication(services, appSettings.JWTSecret);
            ConfigureAuthorization(services);
            services.AddScoped<BaseControllerDependencies>();
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddCors(options =>
            {
                options.AddPolicy(CorsPolicyName,
                    builder =>
                    {
                        builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
                    });
            });
        }

        private void ConfigureDatabase(IServiceCollection services, AppSettings appSettings)
        {
            services.AddDbContextPool<DefaultContext>(options => 
                options.ConfigureMysql(appSettings.ConnectionStrings.DefaultConnection, appSettings.Logging.EnableDatabaseLogging)
            );
        }
        
        private void ConfigureAuthentication(IServiceCollection services, string secret)
        {
            // configure jwt authentication
            
            var key = Encoding.ASCII.GetBytes(secret);
            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
            // configure DI for application services
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IEmailService, EmailService>();
        }

        private void ConfigureAuthorization(IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("UserNotArchived", policy =>
                    policy.Requirements.Add(new NonArchivedUserRequirement()));
            });
            services.AddScoped<IAuthorizationHandler, NonArchivedUserHandler>();
        }

        private AppSettings ConfigureAppSettings(IServiceCollection services)
        {
            var appSettings = Configuration.Get<AppSettings>();
            services.AddSingleton(appSettings);
            return appSettings;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
               
            }
            
            app.useAlreadyExistsMiddeware();
           
            bool exists = Directory.Exists(@"./Storage");
            if(!exists)
                Directory.CreateDirectory(@"./Storage/Images/");
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), @"Storage/")),
                RequestPath = new PathString("/Storage")
            });
            
            
            app.UseCors(CorsPolicyName);
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            app.UseAuthentication();
            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            
            app.UseEndpoints(endpoints => { endpoints.MapControllers().RequireAuthorization("UserNotArchived"); });
            // handle shutdown of processing threads
            applicationLifetime.ApplicationStopping.Register(() => OnShutDown(app.ApplicationServices));
        }

        public void OnShutDown(IServiceProvider serviceProvider)
        {
            // var processingManager = serviceProvider.GetService<ProcessingManager>();
            // processingManager.ShutDown();
        }
    }
}
