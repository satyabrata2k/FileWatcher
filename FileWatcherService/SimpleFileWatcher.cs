using System.IO;
using FileWatcherService.Wrappers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FileWatcherService
{
    public class SimpleFileWatcher : IFileWatcher
    {
        private readonly IWatcherWrapper _watcherWrapper;
        private readonly IFileBag _fileBag;
        private readonly ILogger<SimpleFileWatcher> _logger;
        private readonly string _source;
        private readonly string _destination;

        public SimpleFileWatcher(IConfiguration config, IWatcherWrapper watcherWrapper, IFileBag fileBag, ILogger<SimpleFileWatcher> logger)
        {
            _watcherWrapper = watcherWrapper;
            _fileBag = fileBag;
            _logger = logger;

            _source = config.GetValue<string>("FolderPath:Source");
            _destination = config.GetValue<string>("FolderPath:Destination");

            Directory.CreateDirectory(_source);
            Directory.CreateDirectory(_destination);
        }

        public void Start()
        {
            _watcherWrapper.Path = _source;
            _watcherWrapper.NotifyFilter = NotifyFilters.FileName;
            _watcherWrapper.IncludeSubdirectories = true;
            _watcherWrapper.EnableRaisingEvents = true;

            _watcherWrapper.Created += OnCreated;
            _watcherWrapper.Error += OnError;

            _logger.LogInformation($"Source Directory: '{_source}', Destination Directory: '{_destination}'");
            _logger.LogInformation("SimpleFileWatcher started.");
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            _fileBag.Add(e.FullPath);
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            _logger.LogError(e?.GetException(), "Error ");
        }
    }
}