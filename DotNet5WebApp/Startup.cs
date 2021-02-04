using DotNet5WebApp.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
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

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

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


    //public class RemoveVersionFromParameter : Swashbuckle.AspNetCore.SwaggerGen.IOperationFilter
    //{
    //    public void Apply(Operation operation, OperationFilterContext context)
    //    {
    //        var versionParameter = operation.Parameters.Single(p => p.Name == "version");
    //        operation.Parameters.Remove(versionParameter);
    //    }
    //}

    //public class ReplaceVersionWithExactValueInPath : Swashbuckle.AspNetCore.SwaggerGen.IDocumentFilter
    //{
    //    public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
    //    {
    //        swaggerDoc.Paths = swaggerDoc.Paths
    //            .ToDictionary(
    //                path => path.Key.Replace("v{version}", swaggerDoc.Info.Version),
    //                path => path.Value
    //            );
    //    }
    //}
}
