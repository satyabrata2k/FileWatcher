using System.IO;

namespace FileWatcherService.Wrappers
{
    public class WatcherWrapper : IWatcherWrapper
    {
        private readonly FileSystemWatcher _watcher;
        
        public event ErrorEventHandler Error;
        public event FileSystemEventHandler Created;

        public WatcherWrapper(FileSystemWatcher watcher)
        {
            _watcher = watcher;
            if (watcher == null)
            {
                return;
            }

            watcher.Created += (o, args) => Created?.Invoke(o, args);
            watcher.Error += (o, args) => Error?.Invoke(o, args);
        }

        public string Path
        {
            get => _watcher.Path;
            set => _watcher.Path = value;
        }

        public NotifyFilters NotifyFilter
        {
            get => _watcher.NotifyFilter;
            set => _watcher.NotifyFilter = value;
        }

        public bool IncludeSubdirectories
        {
            get => _watcher.IncludeSubdirectories;
            set => _watcher.IncludeSubdirectories = value;
        }

        public bool EnableRaisingEvents
        {
            get => _watcher.EnableRaisingEvents;
            set => _watcher.EnableRaisingEvents = value;
        }
    }
}