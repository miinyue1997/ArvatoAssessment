
using System.Collections.Generic;
using AspNetCoreRateLimit;
using ArvatoAssessment.Common;
using ArvatoAssessment.DataAcess.Entity;
using ArvatoAssessment.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ArvatoAssessment
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
            services.AddOptions();

            //Add memory cache to store data required for rate limiting
            services.AddMemoryCache();

            //Get connection string from appsettings
            DBConnection.ConnectionString = Configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<RequestContext>();

            //get and set the configuration of IP rate limits from appsettings
            //EnableEndpointRateLimiting: if set to true, the count of limit is specific for each api endpoint
            //GeneralRules: define general rules of Endpoint, Period and Limit that applies for all Eg: Endpoint = *, Period = 1s, Limit = 1 (any client has a limit of 1 request per second)
            //IpWhitelist, EndpointWhitelist, ClientWhitelist : use to whitelist specific IP, Endpoint, ClientId
            //HttpStatusCode: define the statuscode to be returned to client
            services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));

            //this should be configured when there's specific policy for different IP to different Endpoint
            services.Configure<IpRateLimitPolicies>(Configuration.GetSection("IpRateLimitPolicies"));

            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();

            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            services.AddSingleton(Configuration);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Azure Functions API", Version = "v1" });

                c.AddSecurityDefinition("Authentication",
                    new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                    {
                        Description = "Enter Authentication Key",
                        Name = "Authentication",
                        In = Microsoft.OpenApi.Models.ParameterLocation.Header
                    });

                c.AddSecurityDefinition("Authorization",
                    new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                    {
                        Description = "Enter Authorization Key",
                        Name = "Authorization",
                        In = Microsoft.OpenApi.Models.ParameterLocation.Header
                    });

                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                    {
                        Reference = new Microsoft.OpenApi.Models.OpenApiReference
                        {
                            Id = "Authentication",
                            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme
                        }
                    }, new List<string>() },
                    {new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                    {
                        Reference = new Microsoft.OpenApi.Models.OpenApiReference
                        {
                            Id = "Authorization",
                            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme
                        }
                    }, new List<string>() }
                });
            });

            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
            }).SetCompatibilityVersion(CompatibilityVersion.Latest);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseMiddleware<RequestLoggingMiddleware>();

            //configure to use ip rate limiting
            app.UseIpRateLimiting();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Azure Functions API");
            });

            app.UseHttpsRedirection();

            app.UseMvc();
        }
    }
}
