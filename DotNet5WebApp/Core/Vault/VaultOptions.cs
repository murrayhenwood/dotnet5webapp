using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNet5WebApp.Core.Vault
{
    public class VaultOptions
    {
        public string Address { get; set; }
        public string Role { get; set; }

        public string MountPath { get; set; }
        public string Engine { get; set; }


        public string VaultToken { get; set; }
    }
}
