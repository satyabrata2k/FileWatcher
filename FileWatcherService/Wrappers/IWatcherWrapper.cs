using System.IO;

namespace FileWatcherService.Wrappers
{
    public interface IWatcherWrapper
    {
        event FileSystemEventHandler Created;
        event ErrorEventHandler Error;
        string Path { get; set; }
        NotifyFilters NotifyFilter { get; set; }
        bool IncludeSubdirectories { get; set; }
        bool EnableRaisingEvents { get; set; }
    }
}