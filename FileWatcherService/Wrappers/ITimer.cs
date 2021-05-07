using System.Timers;

namespace FileWatcherService.Wrappers
{
    public interface ITimer
    {
        bool AutoReset { get; set; }
        double Interval { get; set; }
        event ElapsedEventHandler Elapsed;
        void Start();
        void Stop();
    }
}