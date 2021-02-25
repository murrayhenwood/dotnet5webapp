using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNet5WebApp.Core.Vault
{
    public static class IConfigurationBuilder_VaultExtensions
    {
        public static IConfigurationBuilder AddVaultSecretsEngineConfiguration(
            this IConfigurationBuilder configuration,
        Action<VaultOptions> options)
        {
            var vaultOptions = new VaultConfigurationSource(options);
            configuration.Add(vaultOptions);
            return configuration;
        }
    }
}
