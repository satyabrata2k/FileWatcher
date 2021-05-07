using System.IO;

namespace FileWatcherService
{
    public interface IFileWatcher
    {
        void Start(FileSystemWatcher watcher);
    }
}