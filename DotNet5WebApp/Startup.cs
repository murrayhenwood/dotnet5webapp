using DotNet5WebApp.Core;
using DotNet5WebApp.Core.Vault;
using DotNet5WebApp.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;


namespace DotNet5WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        //public IConfiguration Configuration { get; }
        public static IConfiguration Configuration { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<VaultOptions>(Configuration.GetSection("Vault"));

            var dbBuilder = new SqlConnectionStringBuilder(
              Configuration.GetConnectionString("Database")
            );

            if (Configuration["database:userID"] != null)
            {
                dbBuilder.UserID = Configuration["database:userID"];
                dbBuilder.Password = Configuration["database:password"];

                Configuration.GetSection("ConnectionStrings")["Database"] = dbBuilder.ConnectionString;
            }

            services.AddDbContext<ProjectContext>(opt =>
                opt.UseSqlServer(Configuration.GetConnectionString("Database")));


            services.AddApiVersioning();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "DotNet5WebApp", Version = "v1" });
                c.SwaggerDoc("v2", new OpenApiInfo { Title = "DotNet5WebApp", Version = "v2" });
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(
                c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "DotNet5WebApp v1");
                    c.RoutePrefix = string.Empty;
                });

            app.UseSerilogRequestLogging();

            //   app.UseHttpsRedirection();

            app.UseRouting();

            app.UseHealthChecks("/hc", new HealthCheckOptions
            {
                Predicate = (options) => { return options.Tags.Any(x => x.Equals("liveness")); },
                ResponseWriter = HealthCheckResponseWriter.TextFormatter
            });
            app.UseHealthChecks("/hc-json", new HealthCheckOptions
            {
                Predicate = (options) => { return options.Tags.Any(x => x.Equals("liveness")); },
                ResponseWriter = HealthCheckResponseWriter.JsonFormatter
            });
            app.UseHealthChecks("/startup", new HealthCheckOptions
            {
                Predicate = (options) => { return options.Tags.Any(x => x.Equals("startup")); },
                ResponseWriter = HealthCheckResponseWriter.JsonFormatter
            });

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
