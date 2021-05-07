namespace FileWatcherService
{
    public interface IFileMover
    {
        void Enqueue(string sourceFile);
    }
}