using System.Timers;

namespace FileWatcherService.Wrappers
{
    public class TimeWrapper: Timer, ITimer
    {
        bool ITimer.AutoReset
        {
            get => AutoReset;
            set => AutoReset = value;
        }
        
        double ITimer.Interval
        {
            get => Interval;
            set => Interval = value;
        }

        void ITimer.Start()
        {
            Start();
        }

        void ITimer.Stop()
        {
            Stop();
        }

        event ElapsedEventHandler ITimer.Elapsed
        {
            add => Elapsed += value;
            remove => Elapsed -= value;
        }
    }
}