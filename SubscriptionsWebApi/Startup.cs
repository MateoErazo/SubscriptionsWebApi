﻿using SubscriptionsWebApi.Filters;
using SubscriptionsWebApi.Services;
using SubscriptionsWebApi.Utils;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

[assembly: ApiConventionType(typeof(DefaultApiConventions))]
namespace SubscriptionsWebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(MyGlobalExceptionFilter));
                options.Conventions.Add(new SwaggerGroupByVersion());
            })
                .AddJsonOptions(options => 
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles)
                .AddNewtonsoftJson();

            services.AddDbContext<ApplicationDbContext>( options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(Configuration["JwtKey"])
                    ),
                    ClockSkew = TimeSpan.Zero
                });

            services.AddAutoMapper(typeof(Startup));

            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen( c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { 
                    Title = "SubscriptionsWebApi", 
                    Version = "v1",
                    Description = "This is a web api about authors and books and It was created with educational purposes only.",
                    Contact = new OpenApiContact
                    {
                        Email = "erazojesusmateo@hotmail.com",
                        Name = "Jesús Mateo Erazo Paladinez",
                        Url = new Uri("https://github.com/MateoErazo")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT"
                    }
                });

                c.SwaggerDoc("v2", new OpenApiInfo { 
                    Title = "SubscriptionsWebApi", 
                    Version = "v2",
                    Description = "This is a web api about authors and books and It was created with educational purposes only.",
                    Contact = new OpenApiContact
                    {
                        Email = "erazojesusmateo@hotmail.com",
                        Name = "Jesús Mateo Erazo Paladinez",
                        Url = new Uri("https://github.com/MateoErazo")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT"
                    }
                });


                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name="Authorization",
                    Type=SecuritySchemeType.ApiKey,
                    Scheme="Bearer",
                    BearerFormat = "JWT",
                    In= ParameterLocation.Header
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{}
                    }
                });

                c.OperationFilter<AddHeaderHATEOAS>();

                string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("IsAdmin", policy =>
                {
                    policy.RequireClaim("isAdmin", new string[] {"1"});
                });
            });

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                  builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });

            services.AddDataProtection();

            services.AddTransient<HashService>();

            services.AddTransient<LinksGenerator>();
            services.AddTransient<HATEOASAuthorFilterAttribute>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddApplicationInsightsTelemetry(options =>
            {
              options.ConnectionString = Configuration["ApplicationInsights:ConnectionString"];
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {

      //if (env.IsDevelopment())
      //{
      //    app.UseSwagger();
      //    app.UseSwaggerUI(c =>
      //    {
      //        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SubscriptionsWebApi v1");
      //        c.SwaggerEndpoint("/swagger/v2/swagger.json", "SubscriptionsWebApi v2");
      //    });
      //}

      app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SubscriptionsWebApi v1");
                c.SwaggerEndpoint("/swagger/v2/swagger.json", "SubscriptionsWebApi v2");
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });

        }
    }
}
