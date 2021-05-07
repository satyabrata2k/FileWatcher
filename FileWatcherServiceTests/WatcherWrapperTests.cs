using System.IO;
using FileWatcherService.Wrappers;
using NSubstitute;
using NUnit.Framework;

namespace FileWatcherServiceTests
{
    public class WatcherWrapperTests
    {
        [Test]
        public void Wraps_FileSystemWatcher()
        {
            const string path = "TestData";
            var fileSystemWatcher = Substitute.For<FileSystemWatcher>();
            var target = new WatcherWrapper(fileSystemWatcher)
            {
                Path = path,
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime,
                EnableRaisingEvents = true,
                IncludeSubdirectories = true
            };
            
            Assert.That(target.Path, Is.EqualTo(path));
            Assert.That(target.NotifyFilter, Is.EqualTo(NotifyFilters.LastWrite | NotifyFilters.CreationTime));
            Assert.That(target.EnableRaisingEvents, Is.True);
            Assert.That(target.IncludeSubdirectories, Is.True);
        }
    }
}