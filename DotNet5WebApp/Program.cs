using DotNet5WebApp.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNet5WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CommonHostBuilder.Foundation<Startup>(args).Build().Run();
        }
    }
}
