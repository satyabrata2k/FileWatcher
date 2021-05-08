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
    public class WorkerTests
    {
        private IConfiguration _config;
        private const string TestSourceDir = @"TestData\Source";
        private const string TestDestinationDir = @"TestData\Destination";
        private const string SourceFile = @"TestData\Source\Dummy_Source.json";
        private const string DestinationFile = @"TestData\Destination\Dummy_Source.json";

        [SetUp]
        public void Setup()
        {
            var inMemorySettings = new Dictionary<string, string> {
                {"FolderPath:Source", TestSourceDir},
                {"FolderPath:Destination", TestDestinationDir},
                {"FolderPath:ParallelThreads", "2"}
            };

            _config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            SetupTestData();
        }

        [Test]
        public void Moves_File_To_Destination()
        {
            var logger = Substitute.For<ILogger<Worker>>();
            var source = $@"{TestSourceDir}\Dummy_Source.json";

            var isFileInSourceBefore = File.Exists(SourceFile);
            var isFileInDestinationBefore = File.Exists(DestinationFile);

            var target = new Worker(_config, logger);
            target.MoveFilesToDestination(new List<string> { source });

            var isFileInSourceAfter= File.Exists(SourceFile);
            var isFileInDestinationAfter = File.Exists(DestinationFile);

            Assert.That(isFileInSourceBefore, Is.True);
            Assert.That(isFileInDestinationBefore, Is.False);
            Assert.That(isFileInSourceAfter, Is.False);
            Assert.That(isFileInDestinationAfter, Is.True);
            logger.Received(1).LogInformation($"Successfully transferred file '{SourceFile}'");
        }

        [Test]
        public void Logs_Error_On_Exception()
        {
            var source = $@"{TestSourceDir}\Dummy_Source.json";
            var exception = new Exception();
            var logger = Substitute.For<ILogger<Worker>>();
            logger.When(x => x.LogInformation($"Successfully transferred file '{source}'"))
                .Do(x => throw exception);

            var target = new Worker(_config, logger);
            target.MoveFilesToDestination(new List<string> { source });
            
            logger.Received(1).LogError(exception, $"Error While transferring file '{source}'");
        }

        private static void SetupTestData()
        {
            if (File.Exists(SourceFile))
            {
                File.Delete(SourceFile);
            }

            if (File.Exists(DestinationFile))
            {
                File.Delete(DestinationFile);
            }

            Directory.CreateDirectory(TestSourceDir);
            Directory.CreateDirectory(TestDestinationDir);
            const string testFile = @"TestData\Dummy_Source.json";
            File.Copy(testFile, SourceFile);
        }
    }
}