using System;
using System.Collections.Generic;
using System.IO;
using FileWatcherService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace FileWatcherServiceTests
{
    public class FileWatcherTests
    {
        private IConfiguration _config;
        private readonly string _testSourceDir = @"C:\\RunWatcherTestSource" + Guid.NewGuid();
        private readonly string _testDestinationDir = @"C:\\RunWatcherTestDest" + Guid.NewGuid();

        [SetUp]
        public void Setup()
        {
            var inMemorySettings = new Dictionary<string, string> {
                {"FolderPath:Source", _testSourceDir},
                {"FolderPath:Destination", _testDestinationDir}
            };

            _config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
        }

        [TearDown]
        public void TearDown()
        {
            Directory.Delete(_testSourceDir);
            Directory.Delete(_testDestinationDir);
        }

        [Test]
        public void Starts_File_Watcher()
        {
            var fileMover = Substitute.For<IFileMover>();
            var logger = Substitute.For<ILogger<FileWatcher>>();
            var watcher = Substitute.For<FileSystemWatcher>();

            var target = new FileWatcher(_config, fileMover, logger);
            target.Start(watcher);

            Assert.That(watcher.Path, Is.EqualTo(_testSourceDir));
            Assert.That(watcher.NotifyFilter, Is.EqualTo(NotifyFilters.FileName));
            Assert.That(watcher.IncludeSubdirectories, Is.EqualTo(true));
            Assert.That(watcher.EnableRaisingEvents, Is.EqualTo(true));

            logger.Received(1).LogInformation("FileWatcher started");
        }
    }
}