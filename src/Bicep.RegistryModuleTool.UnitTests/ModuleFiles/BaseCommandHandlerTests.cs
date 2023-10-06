// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Bicep.Core.Exceptions;
using Bicep.RegistryModuleTool.Commands;
using Bicep.RegistryModuleTool.TestFixtures.MockFactories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Bicep.RegistryModuleTool.UnitTests.ModuleFiles
{
    [TestClass]
    public class BaseCommandHandlerTests
    {
        [TestMethod]
        public async Task InvokeAsync_NoException_Passthrough()
        {
            var exitCode = await InvokeAsync(new PassThroughCommandHandler(100));

            exitCode.Should().Be(100);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetExceptionData), DynamicDataSourceType.Method)]
        public async Task InvokeAsync_CaughtException_ReturnsOne(Exception exceptionToThrow)
        {
            var exitCode = await InvokeAsync(new ThrowExceptionCommandHandler(exceptionToThrow));

            exitCode.Should().Be(1);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetExpectedExceptionData), DynamicDataSourceType.Method)]
        public async Task InvokeAsync_CaughtExpectedException_LogsDebug(Exception exceptionToThrow)
        {
            var logger = MockLoggerFactory.CreateLogger();

            await InvokeAsync(new ThrowExceptionCommandHandler(exceptionToThrow, logger));

            Mock.Get(logger).Verify(x => x.Log(
                LogLevel.Debug,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
        }

        [DataTestMethod]
        [DynamicData(nameof(GetUnexpectedExceptionData), DynamicDataSourceType.Method)]
        public async Task InvokeAsync_CaughtUnexpectedException_LogsCritical(Exception exceptionToThrow)
        {
            var logger = MockLoggerFactory.CreateLogger();

            await InvokeAsync(new ThrowExceptionCommandHandler(exceptionToThrow, logger));

            Mock.Get(logger).Verify(x => x.Log(
                LogLevel.Critical,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                exceptionToThrow,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
        }

        private static Task<int> InvokeAsync(ICommandHandler handler)
        {
            var command = new TestCommand()
            {
                Handler = handler,
            };

            return command.InvokeAsync("");
        }

        private static IEnumerable<object[]> GetExceptionData() => GetExpectedExceptionData().Concat(GetUnexpectedExceptionData());

        private static IEnumerable<object[]> GetExpectedExceptionData()
        {
            yield return new object[]
            {
                new BicepException(""),
            };

            yield return new object[]
            {
                new IOException(),
            };

            yield return new object[]
            {
                new UnauthorizedAccessException(),
            };
        }

        private static IEnumerable<object[]> GetUnexpectedExceptionData()
        {
            yield return new object[]
            {
                new NotImplementedException (),
            };

            yield return new object[]
            {
                new ArgumentException (),
            };

            yield return new object[]
            {
                new InvalidOperationException (),
            };
        }

        private class TestCommand : Command
        {
            public TestCommand()
                : base("test")
            {
            }
        }

        private class PassThroughCommandHandler : BaseCommandHandler
        {
            private readonly int exitCode;

            public PassThroughCommandHandler(int exitCode)
                : base(MockFileSystemFactory.CreateForSample(Sample.Empty), MockLoggerFactory.CreateLogger())
            {
                this.exitCode = exitCode;
            }

            protected override Task<int> InvokeInternalAsync(InvocationContext context) => Task.FromResult(this.exitCode);
        }

        private class ThrowExceptionCommandHandler : BaseCommandHandler
        {
            private readonly Exception exceptionToThrow;

            public ThrowExceptionCommandHandler(Exception exceptionToThrow, ILogger? logger = null)
                : base(MockFileSystemFactory.CreateForSample(Sample.Empty), logger ?? MockLoggerFactory.CreateLogger())
            {
                this.exceptionToThrow = exceptionToThrow;
            }

            protected override Task<int> InvokeInternalAsync(InvocationContext context)
            {
                throw exceptionToThrow;
            }
        }
    }
}
