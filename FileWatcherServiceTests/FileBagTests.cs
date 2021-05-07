using System;
using System.Collections.Generic;
using System.Timers;
using FileWatcherService;
using FileWatcherService.Wrappers;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace FileWatcherServiceTests
{
    public class FileBagTests
    {
        [Test]
        public void Adds_Files_To_Bag()
        {
            const string file1 = "testFile";
            const string file2 = "testFile";
            var fileMover = Substitute.For<IWorker>();
            var timer = Substitute.For<ITimer>();
            var logger = Substitute.For<ILogger<FileBag>>();

            var target = new FileBag(fileMover, timer, logger);
            target.Add(file1);
            target.Add(file2);

            logger.Received(1).LogInformation($"Successfully Added file to bag: '{file1}', Current Items in bag: 1");
            logger.Received(1).LogInformation($"Successfully Added file to bag: '{file2}', Current Items in bag: 2");
        }

        [Test]
        public void Logs_Error_If_File_Cannot_Be_Bagged()
        {
            const string file = "testFile";
            var exception = new Exception();
            var fileMover = Substitute.For<IWorker>();
            var timer = Substitute.For<ITimer>();
            var logger = Substitute.For<ILogger<FileBag>>();
            logger.When(x => x.LogInformation($"Successfully Added file to bag: '{file}', Current Items in bag: 1"))
                .Do(x => throw exception);

            var target = new FileBag(fileMover, timer, logger);
            target.Add(file);

            logger.Received(1).LogError(exception, $"Error while bagging file: '{file}'");
        }

        [Test]
        public void Moves_Files_When_Timer_Elapses()
        {
            const string file1 = "testFile";
            const string file2 = "testFile";
            var fileMover = Substitute.For<IWorker>();
            var timer = Substitute.For<ITimer>();
            var logger = Substitute.For<ILogger<FileBag>>();

            var target = new FileBag(fileMover, timer, logger);
            target.Add(file1);
            target.Add(file2);
            timer.Elapsed += Raise.Event<ElapsedEventHandler>(this, Arg.Any<ElapsedEventArgs>());

            logger.Received(1).LogInformation("Number Of files to be Transferred : 2 ");
            fileMover.Received(1).MoveFilesToDestination(Arg.Any<List<string>>());
            timer.Received(1).Stop();
            timer.Received(1).Interval = 15000;
            timer.Received(2).Start();
        }
    }
}