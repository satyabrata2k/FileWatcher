using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Timers;
using FileWatcherService.Wrappers;
using Microsoft.Extensions.Logging;

namespace FileWatcherService
{
    public class FileBag : IFileBag
    {
        private readonly IWorker _worker;
        private readonly ITimer _timer;
        private readonly ILogger<FileBag> _logger;
        private readonly ConcurrentBag<string> _sourceFiles;

        public FileBag(IWorker worker, ITimer timer, ILogger<FileBag> logger)
        {
            _worker = worker;
            _timer = timer;
            _logger = logger;
            _sourceFiles = new ConcurrentBag<string>();

            _timer.AutoReset = false;
            _timer.Elapsed += MoveBaggedFiles;
            _timer.Start();
        }

        public void Add(string source)
        {
            try
            {
                _sourceFiles.Add(source);
                _logger.LogInformation($"Successfully Added file to bag: '{source}', Current Items in bag: {_sourceFiles.Count}");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Error while bagging file: '{source}'");
            }
        }

        private void MoveBaggedFiles(object sender, ElapsedEventArgs e)
        {
            _timer.Stop();

            var files = new List<string>();
            while (!_sourceFiles.IsEmpty)
            {
                _sourceFiles.TryTake(out var item);
                files.Add(item);
            }

            _logger.LogInformation($"Number Of files to be Transferred : {files.Count} ");
            _worker.MoveFilesToDestination(files);
            
            _timer.Interval = 15000;
            _timer.Start();
        }
    }
}