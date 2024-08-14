// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using System;
using Azure.Security.KeyVault.Secrets;

namespace EchoBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
//            .ConfigureAppConfiguration((context, config) =>
//{
//    var keyVaultEndpoint = new Uri(Environment.GetEnvironmentVariable("VaultUri"));
//    config.AddAzureKeyVault(keyVaultEndpoint, new VisualStudioCredential());
//})
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureLogging((logging) =>
                    {
                        logging.AddDebug();
                        logging.AddConsole();
                    });
                    webBuilder.UseStartup<Startup>();
                });
    }
}
