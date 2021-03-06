﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CatiLyfe.Backend.Web.Core
{
    using CatiLyfe.Backend.Web.Core.Code;
    using CatiLyfe.Backend.Web.Core.Code.Filters;
    using CatiLyfe.Backend.Web.Models;
    using CatiLyfe.Backend.Web.Core.Code.Trace;
    using CatiLyfe.Common.Logging;
    using CatiLyfe.Common.Security;
    using CatiLyfe.DataLayer;
    using CatiLyfe.DataLayer.Sql;

    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Server.IISIntegration;

    using Swashbuckle.AspNetCore.Swagger;
    using CatiLyfe.Backend.ImageServices;

    public class Startup
    {
        private readonly ILoggerFactory loggerFactory;

        public Startup(IConfiguration configuration, ILoggerFactory logger)
        {
            this.Configuration = configuration;
            this.loggerFactory = logger;
        }

        /// <summary>
        /// The configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Configuration for services.
        /// </summary>
        /// <param name="services">The services collection.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            var constr = this.Configuration.GetConnectionString("default");
            var imageSection = this.Configuration.GetSection("image");

            var authDataLayer = CatiDataLayerFactory.CreateAuthDataLayer(constr);
            var catiData = CatiDataLayerFactory.CreateDataLayer(constr);
            var imageData = CatiDataLayerFactory.CreateImageDataLayer(constr);

            var storageConnection = this.Configuration.GetConnectionString("images");

            var imageWidths = imageSection.GetSection("widths").Get<int[]>();
            var imageUploader = ImageUploaderFactory.Create(imageData, storageConnection, imageWidths);

            var trace = new WebAppTrace(this.loggerFactory);
            trace.TraceInfo("Logger has been initialized.");


            var contentTransformer = new MarkdownProcessor();
            var postTranslator = PostTranslatorFactory.Create(authDataLayer, contentTransformer, imageData, imageUploader);

            // Add the data layers.
            services.AddSingleton<IProgramTrace>(trace);
            services.AddSingleton<ICatiDataLayer>(catiData);
            services.AddSingleton<ICatiAuthDataLayer>(authDataLayer);
            services.AddSingleton<IPostTranslator>(postTranslator);
            services.AddSingleton<ICatiImageDataLayer>(imageData);
            services.AddSingleton<IContentTransformer>(contentTransformer);
            services.AddSingleton<IImageUploader>(imageUploader);
            services.AddSingleton<IAuthorizationHandler, DefaultAuthorizationHandler>().AddAuthorization(
                options =>
                    {
                        options.AddPolicy("default", policy => policy.Requirements.Add(new DefaultAuthorizationRequirement()));
                    });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(
                config =>
                    {
                        config.Cookie.HttpOnly = true;
                        //config.Cookie.Domain = "caticake.azurewebsites.net";
                        config.Cookie.Name = "CatiCookie";
                        config.Cookie.SameSite = SameSiteMode.None;
                        config.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                        config.Cookie.Path = "";
                        config.Events.OnRedirectToLogin = options =>
                            {
                                options.Response.StatusCode = 401;
                                return Task.CompletedTask;
                            };
                        config.Events.OnRedirectToAccessDenied = options =>
                            {
                                options.Response.StatusCode = 401;
                                return Task.CompletedTask;
                            };
                        config.Events.OnRedirectToReturnUrl = options =>
                        {
                            options.Response.StatusCode = 401;
                            return Task.CompletedTask;
                        };
                    });

            services.AddCors(
                options =>
                    {
                        options.AddPolicy(
                            "default",
                            policy =>
                                {
                                    policy.AllowAnyHeader();
                                    policy.AllowAnyMethod();
                                    policy.AllowCredentials();
                                    policy.AllowAnyOrigin();
                                });
                    });

            services.AddMvc(
                config =>
                    {
                        // config.Filters.Add(new AuthorizationFilter(authDataLayer));
                        config.Filters.Add(new CatiExceptionFilter());
                        config.Filters.Add(new ValidationFilter());
                    });

            // Add the documentation
            services.AddSwaggerGen(
                config =>
                    {
                        config.SwaggerDoc("v1", new Info { Title = "Cati Lyfe Api", Version = "0.0.0.0.0.0.1" });
                    });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();
            app.UseCors("default");

            // Enable api documentation
            app.UseSwagger();
            app.UseSwaggerUI(
                config =>
                    {
                        config.SwaggerEndpoint("/swagger/v1/swagger.json", "Cati Lyfe Api");
                    });

            app.UseMvc();
        }
    }
}

