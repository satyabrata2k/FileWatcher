using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FileWatcherService
{
    public class FileWatcher : IFileWatcher
    {
        private readonly IFileMover _fileMover;
        private readonly ILogger<FileWatcher> _logger;
        private FileSystemWatcher _watcher;
        private readonly string _source;

        public FileWatcher(IConfiguration config, IFileMover fileMover, ILogger<FileWatcher> logger)
        {
            _fileMover = fileMover;
            _logger = logger;

            _source = config.GetValue<string>("FolderPath:Source");
            var destination = config.GetValue<string>("FolderPath:Destination");

            Directory.CreateDirectory(_source);
            Directory.CreateDirectory(destination);
        }

        public void Start(FileSystemWatcher watcher)
        {
            _watcher = watcher;
            _watcher.Path = _source;
            _watcher.NotifyFilter = NotifyFilters.FileName;
            _watcher.IncludeSubdirectories = true;
            _watcher.EnableRaisingEvents = true;
            
            _watcher.Created += OnCreated;
            _logger.LogInformation("FileWatcher started");
            _watcher.Error += OnError;
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            _logger.LogError(e?.GetException(), "Error ");
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            _fileMover.Enqueue(e.FullPath);
        }
    }
}