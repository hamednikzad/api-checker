using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ApiChecker.Properties;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace ApiChecker
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            var configuration = ConfigApplication();
            var appConfig = ReadAppConfig(configuration);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(appConfig));
        }

        private static AppConfig ReadAppConfig(IConfigurationRoot configuration)
        {
            var intervalInSeconds = int.Parse(configuration["IntervalInSeconds"] ?? "120");
            var apisSection = configuration.GetSection("Apis").GetChildren();

            Log.Information("IntervalInSeconds: {IntervalInSeconds}", intervalInSeconds);

            var apiAddresses = new List<ApiAddress>();
            foreach (var apiAddress in apisSection)
            {
                var name = apiAddress.GetValue<string>("Name");
                var address = apiAddress.GetValue<string>("Address");
                var enabled = apiAddress.GetValue<bool?>("Enable");

                var newApiAddress = new ApiAddress()
                {
                    Name = name,
                    Address = address,
                    Enabled = enabled ?? true
                };
                Log.Information("{Name}=> {Address}=> Enable: {Enabled}", newApiAddress.Name,
                    newApiAddress.Address, newApiAddress.Enabled);
                apiAddresses.Add(newApiAddress);
            }

            return new AppConfig()
            {
                IntervalInSeconds = intervalInSeconds,
                ApiAddresses = apiAddresses
            };
        }

        private static IConfigurationRoot ConfigApplication()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile(
                    $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json",
                    true)
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            return configuration;
        }
    }
}