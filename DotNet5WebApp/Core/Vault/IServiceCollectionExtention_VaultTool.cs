using DotNet5WebApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace DotNet5WebApp.Core.Vault
{
    public static class IServiceCollectionExtention_VaultTool
    {
        public static IServiceCollection AddVaultBackedContext<TContext>(
            this IServiceCollection services,
            IConfiguration configuration, string connectionString, string vaultRole) where TContext : DbContext
        {

            SqlConnectionStringBuilder dbBuilder = new SqlConnectionStringBuilder(
              configuration.GetConnectionString(connectionString)
            );

            dbBuilder.UserID = configuration["database:" + vaultRole + ":userID"];
            dbBuilder.Password = configuration["database:" + vaultRole + ":password"];

            configuration.GetSection("ConnectionStrings")[connectionString] = dbBuilder.ConnectionString;

            services.AddDbContext<ProjectContext>(opt =>
                opt.UseSqlServer(configuration.GetConnectionString(connectionString)));

            return services;
        }
    }
}
