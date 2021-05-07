using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FileWatcherService
{
    public class Worker : IWorker
    {
        private readonly ILogger<Worker> _logger;
        private readonly string _source;
        private readonly string _destination;
        private readonly int _parallelThreads;

        public Worker(IConfiguration config, ILogger<Worker> logger)
        {
            _logger = logger;
            _source = config.GetValue<string>("FolderPath:Source");
            _destination = config.GetValue<string>("FolderPath:Destination");
            _parallelThreads = config.GetValue<int>("FolderPath:ParallelThreads");
            _logger.LogInformation($"Number Of parallel Threads : {_parallelThreads} ");
        }

        public void MoveFilesToDestination(List<string> files)
        {
            Parallel.ForEach(files, new ParallelOptions
            {
                MaxDegreeOfParallelism = _parallelThreads
            }, Move);
        }
        
        private void Move(string file)
        {
            try
            {
                var sourceFile = file;
                var destinationFile = sourceFile.Replace(_source, _destination);
                Transfer(sourceFile, destinationFile);
                _logger.LogInformation($"Successfully transferred file '{file}'");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Error While transferring file '{file}'");
            }
        }

        private static void Transfer(string sourceFile, string destinationFile)
        {
            if (File.Exists(destinationFile))
            {
                File.Delete(destinationFile);
            }
            File.Copy(sourceFile, destinationFile, true);
            File.Delete(sourceFile);
        }
    }
}