using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaultSharp;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.AuthMethods.UserPass;
using VaultSharp.V1.Commons;
using VaultSharp.V1.SecretsEngines;

namespace DotNet5WebApp.Core.Vault
{
    public class VaultKeyValueBackedConfigurationProvider : IFileProvider
    {
        public VaultKVCredentials _config;
        private IVaultClient _client;

        private class InMemoryFile : IFileInfo
        {
            private readonly byte[] _data;
            public InMemoryFile(string json) => _data = Encoding.UTF8.GetBytes(json);
            public Stream CreateReadStream() => new MemoryStream(_data);
            public bool Exists { get; } = true;
            public long Length => _data.Length;
            public string PhysicalPath { get; } = string.Empty;
            public string Name { get; } = string.Empty;
            public DateTimeOffset LastModified { get; } = DateTimeOffset.UtcNow;
            public bool IsDirectory { get; } = false;
        }

        private readonly IFileInfo _fileInfo;
        public VaultKeyValueBackedConfigurationProvider(string json) => _fileInfo = new InMemoryFile(json);
        public IFileInfo GetFileInfo(string _) => _fileInfo;
        public IDirectoryContents GetDirectoryContents(string _) => null;
        public IChangeToken Watch(string _) => NullChangeToken.Singleton;

        public Stream CreateReadStream() => _fileInfo.CreateReadStream();

        public VaultKeyValueBackedConfigurationProvider(VaultKVCredentials config) : base() //IConfigurationBuilder_AddVaultKeyValueBackedConfiguration
        {
            _config = config;

            var vaultClientSettings = new VaultClientSettings(
                _config.Address,
                new TokenAuthMethodInfo(_config.Token));

            _client = new VaultClient(vaultClientSettings);
            var secrets = _client.V1.Secrets.KeyValue.V2.ReadSecretAsync<object>(path: _config.Path, null, _config.NamespaceAndProvider).GetAwaiter().GetResult();

            _fileInfo = new InMemoryFile(secrets.Data.Data.ToString());
        }

        public VaultKeyValueBackedConfigurationProvider(
            string vaultAddress,
            string vaultToken,
            string secretPath,
            string providerPath)
        {
            IAuthMethodInfo authMethod = new TokenAuthMethodInfo(vaultToken);

            var vaultClientSettings = new VaultClientSettings(vaultAddress, authMethod)
            {
                ContinueAsyncTasksOnCapturedContext = false,
            };

            var vaultClient = new VaultClient(vaultClientSettings);

            var secrets = Task.Run(() => vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync<object>(path: secretPath, null, providerPath)).Result;

            _fileInfo = new InMemoryFile(secrets.Data.Data.ToString());
        }
        public VaultKeyValueBackedConfigurationProvider(
            string vaultAddress,
            string vaultUsername,
            string vaultPassword,
            string secretPath,
            string providerPath)
        {


            IAuthMethodInfo authMethod = new UserPassAuthMethodInfo(username: vaultUsername, password: vaultPassword);

            var vaultClientSettings = new VaultClientSettings(vaultAddress, authMethod)
            {
                ContinueAsyncTasksOnCapturedContext = false,
            };

            var vaultClient = new VaultClient(vaultClientSettings);

            var secrets = Task.Run(() => vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync<object>(path: secretPath, null, providerPath)).Result;

            _fileInfo = new InMemoryFile(secrets.Data.Data.ToString());
        }
    }



    public class VaultKVCredentials
    {
        public string Address { get; set; }
        public string Token { get; set; }
        public string NamespaceAndProvider { get; set; }
        public string Path { get; set; }
    }
}
