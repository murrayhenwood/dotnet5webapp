using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VaultSharp;
using VaultSharp.V1.AuthMethods.AppRole;

using VaultSharp.V1.Commons;
using VaultSharp.V1.SecretsEngines;

namespace DotNet5WebApp.Core.Vault
{
    public class VaultConfigurationProvider : ConfigurationProvider
    {
        public VaultOptions _config;
        private IVaultClient _client;

        public VaultConfigurationProvider(VaultOptions config)
        {
            _config = config;

            var vaultClientSettings = new VaultClientSettings(
                _config.Address,
                new VaultSharp.V1.AuthMethods.Token.TokenAuthMethodInfo(_config.VaultToken));

            _client = new VaultClient(vaultClientSettings);
        }

        public override void Load()
        {
            LoadAsync().Wait();
        }

        public async Task LoadAsync()
        {
            await GetDatabaseCredentials();
        }

        public async Task GetDatabaseCredentials()
        {
            foreach (var role in _config.Roles)
            {
                Secret<UsernamePasswordCredentials> dynamicDatabaseCredentials =
                    await _client.V1.Secrets.Database.GetCredentialsAsync( role, _config.MountPath + _config.Engine);

                Data.Add("database:" + role + ":userID", dynamicDatabaseCredentials.Data.Username);
                Data.Add("database:" + role + ":password", dynamicDatabaseCredentials.Data.Password);
            }
        }
    }
}
