using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FileWatcherService
{
    public class FileMover : IFileMover
    {
        private readonly ILogger<FileMover> _logger;
        private readonly ConcurrentBag<string> _sourceFiles;
        private readonly string _source;
        private readonly string _destination;
        private readonly int _parallelThreads;
        private static Timer _aTimer;

        public FileMover(IConfiguration config, ILogger<FileMover> logger)
        {
            _logger = logger;
            _source = config.GetValue<string>("FolderPath:Source");
            _destination = config.GetValue<string>("FolderPath:Destination");
            _parallelThreads = config.GetValue<int>("FolderPath:ParallelThreads");
            _sourceFiles = new ConcurrentBag<string>();
            _logger.LogInformation($"Number Of parallel Threads : {_parallelThreads} ");

            _aTimer = new Timer {AutoReset = false};
            _aTimer.Elapsed += OnTimedEvent;
            _aTimer.Start();
        }

        public void Enqueue(string source)
        {
            try
            {
                _sourceFiles.Add(source);
                _logger.LogInformation($"Successfully Added file to queue: '{source}', Count {_sourceFiles.Count}");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Error while queue-ing file: '{source}'");
            }
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            var sources = new List<string>();
            while (!_sourceFiles.IsEmpty)
            {
                _sourceFiles.TryTake(out var item);
                sources.Add(item);
            }

            _logger.LogInformation($"Number Of files to be Transferred : {sources.Count} ");
            Parallel.ForEach(sources, new ParallelOptions {MaxDegreeOfParallelism = _parallelThreads}, DoWork);

            _aTimer.Stop();
            _aTimer.Interval = 10000;
            _aTimer.Start();
        }

        private void DoWork(string file)
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