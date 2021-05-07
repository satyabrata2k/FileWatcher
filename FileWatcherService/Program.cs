using System;
using System.IO;
using FileWatcherService.Wrappers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace FileWatcherService
{
    class Program
    {
        static void Main()
        {
            var host = Startup();

            var fileWatcher = ActivatorUtilities.CreateInstance<SimpleFileWatcher>(host.Services);
            fileWatcher.Start();
            
            Console.ReadLine();
        }

        private static void ConfigSetup(IConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
        }

        private static IHost Startup()
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
                    services.AddTransient<IFileBag, FileBag>();
                    services.AddTransient<ITimer, TimeWrapper>();
                    services.Add(new ServiceDescriptor(typeof(IWatcherWrapper), new WatcherWrapper(new FileSystemWatcher())));
                    services.AddTransient<IFileWatcher, SimpleFileWatcher>();
                    services.AddTransient<IWorker, Worker>();
                })
                .UseSerilog()
                .Build();

            return host;
        }
    }
}
