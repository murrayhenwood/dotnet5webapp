using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Filters;
using System;
using System.Collections.Generic;
using System.IO;

namespace DotNet5WebApp.Core
{

    public class CommonHostBuilder
    {
        public static IHostBuilder Foundation<TStartUp>(string[] args) where TStartUp : class
        {

            return Host.CreateDefaultBuilder(args)
                 .ConfigureAppConfiguration((context, configBuilder) =>
                 {
                     var builtConfig = configBuilder.Build();

                     if (builtConfig["vaultAddress"] != null)
                     {
                         Console.WriteLine("Vault address found in environment, importing config");

                         var vaultFileProvider = new VaultFileProvider(
                             vaultAddress: builtConfig["vaultAddress"],
                             vaultToken: builtConfig["vaultToken"],
                             secretPath: builtConfig["secretPath"],
                             providerPath: builtConfig["providerPath"]);

                          configBuilder.AddJsonStream(vaultFileProvider.MemoryStream);
                     }
                 })
                 .UseSerilog((hostBuilderContext, loggerConfiguration) =>
                 {
                     if (hostBuilderContext.Configuration.GetSection("datadog_api_key").Exists())
                     {
                         loggerConfiguration
                           //  .Filter.ByExcluding(Matching.WithProperty("Path", "/hc"))
                               .Filter.ByExcluding(Matching.WithProperty("RequestPath", "/hc"))
                           //  .Filter.ByExcluding(Matching.WithProperty("Path", "/hc-json"))
                               .Filter.ByExcluding(Matching.WithProperty("RequestPath", "/hc-json"))
                           //  .Filter.ByExcluding(Matching.WithProperty("Path", "/startup"))
                               .Filter.ByExcluding(Matching.WithProperty("RequestPath", "/startup"))
                           //  .Filter.ByExcluding(Matching.WithProperty("Path", "/favicon.ico"))
                               .Filter.ByExcluding(Matching.WithProperty("RequestPath", "/favicon.ico"))

                               .MinimumLevel.Debug()
                               .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                               .Enrich.FromLogContext()
                               .Enrich.WithProperty("MachineName", Environment.MachineName)
                               .WriteTo.Console(outputTemplate: "{MachineName} | {Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u4}] | {Message:l}{NewLine}{Exception}")
                               .WriteTo.DatadogLogs(hostBuilderContext.Configuration.GetSection("datadog_api_key").Value, "Workshop2021", "Workshop2021", Environment.MachineName);
                     }
                     else
                     {
                         loggerConfiguration.MinimumLevel.Debug()
                               .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                               .Enrich.FromLogContext()
                               .Enrich.WithProperty("MachineName", Environment.MachineName)
                               .WriteTo.Console(outputTemplate: "{MachineName} | {Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u4}] | {Message:l}{NewLine}{Exception}");


                     }
                 })
                 .ConfigureServices((services) =>
                 {
                     services.AddHttpClient();
                     services.AddHealthChecks().AddCheck("Health Check", () => HealthCheckResult.Healthy($"Application is running", GetData(services)), tags: new[] { "liveness" });
                 })
                 .ConfigureWebHostDefaults(webBuilder =>
                 {
                     webBuilder.UseStartup<TStartUp>();
                 });
        }

        private static IReadOnlyDictionary<string, object> GetData(IServiceCollection services)
        {
            return new Dictionary<string, object>() {
                { "Environment.MachineName" , Environment.MachineName.ToString() },
                { "Environment.OSVersion" , Environment.OSVersion.ToString() },
                { "Environment.ProcessorCount" , Environment.ProcessorCount.ToString() },
                { "Environment.WorkingSet" , Environment.WorkingSet.ToString() },
                { "DateTime.UtcNow" , DateTime.UtcNow.ToString("o") },
                { "DateTimeOffset.Now" , DateTimeOffset.Now.ToString("o") },
                { "Message" , services.BuildServiceProvider().GetService<HostBuilderContext>().Configuration.GetSection("message").Value},
            };
        }


    }
}
