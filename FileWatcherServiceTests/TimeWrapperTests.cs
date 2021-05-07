using FileWatcherService.Wrappers;
using NUnit.Framework;

namespace FileWatcherServiceTests
{
    public class TimeWrapperTests
    {
        [Test]
        public void Wraps_SystemTimer()
        {
            var target = new TimeWrapper
            {
                AutoReset = true, 
                Interval = 999
            };

            Assert.That(target.AutoReset, Is.True);
            Assert.That(target.Interval, Is.EqualTo(999));
        }
    }
}