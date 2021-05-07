using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace FileWatcherService
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = AppStartup();

            var fileWatcher = ActivatorUtilities.CreateInstance<FileWatcher>(host.Services);
            fileWatcher.Start(new FileSystemWatcher());
            
            Console.ReadLine();
        }

        private static void ConfigSetup(IConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
        }

        private static IHost AppStartup()
        {
            var builder = new ConfigurationBuilder();
            ConfigSetup(builder);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Build())
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) => {
                    services.AddTransient<IFileWatcher, FileWatcher>();
                    services.AddTransient<IFileMover, FileMover>();
                })
                .UseSerilog()
                .Build();

            return host;
        }
    }
}
