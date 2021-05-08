using System;
using System.Collections.Generic;
using System.IO;
using FileWatcherService;
using FileWatcherService.Wrappers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace FileWatcherServiceTests
{
    public class SimpleFileWatcherTests
    {
        private IConfiguration _config;
        private const string TestSourceDir = @"TestData\Src";
        private const string TestDestinationDir = @"TestData\Dest";

        [SetUp]
        public void Setup()
        {
            var inMemorySettings = new Dictionary<string, string> {
                {"FolderPath:Source", TestSourceDir},
                {"FolderPath:Destination", TestDestinationDir}
            };

            _config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
        }

        [Test]
        public void Starts_File_Watcher()
        {
            var fileBag = Substitute.For<IFileBag>();
            var watcherWrapper = Substitute.For<IWatcherWrapper>();
            var logger = Substitute.For<ILogger<SimpleFileWatcher>>();

            var target = new SimpleFileWatcher(_config, watcherWrapper, fileBag, logger);
            target.Start();

            Assert.That(watcherWrapper.Path, Is.EqualTo(TestSourceDir));
            Assert.That(watcherWrapper.NotifyFilter, Is.EqualTo(NotifyFilters.FileName));
            Assert.That(watcherWrapper.IncludeSubdirectories, Is.EqualTo(true));
            Assert.That(watcherWrapper.EnableRaisingEvents, Is.EqualTo(true));

            logger.Received(1).LogInformation($"SimpleFileWatcher started.");
            logger.LogInformation($"Source Directory: '{TestSourceDir}', Destination Directory: '{TestDestinationDir}'");
        }

        [Test]
        public void Events_Are_Wired_Correctly()
        {
            const string file = "testSourceFile.txt";
            var fileBag = Substitute.For<IFileBag>();
            var logger = Substitute.For<ILogger<SimpleFileWatcher>>();
            var fileSysEvtArg = new FileSystemEventArgs(WatcherChangeTypes.All, TestSourceDir, file);
            var exception = new Exception();
            var errorEvtArgs = new ErrorEventArgs(exception);
            var watcherWrapper = Substitute.For<IWatcherWrapper>();
            
            var target = new SimpleFileWatcher(_config, watcherWrapper, fileBag, logger);
            target.Start();
            watcherWrapper.Created += Raise.Event<FileSystemEventHandler>(this, fileSysEvtArg);
            watcherWrapper.Error += Raise.Event<ErrorEventHandler>(this, errorEvtArgs);
            
            fileBag.Received(1).Add(TestSourceDir + "\\" + file);
            logger.Received(1).LogError(errorEvtArgs.GetException(), "Error ");            
        }
    }
}