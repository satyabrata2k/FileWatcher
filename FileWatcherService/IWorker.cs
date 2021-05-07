using System.Collections.Generic;

namespace FileWatcherService
{
    public interface IWorker
    {
        void MoveFilesToDestination(List<string> files);
    }
}