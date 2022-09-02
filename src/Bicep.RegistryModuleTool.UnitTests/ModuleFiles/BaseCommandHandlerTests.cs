// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Exceptions;
using Bicep.RegistryModuleTool.Commands;
using Bicep.RegistryModuleTool.TestFixtures.MockFactories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;

namespace Bicep.RegistryModuleTool.UnitTests.ModuleFiles
{
    [TestClass]
    public class BaseCommandHandlerTests
    {
        [TestMethod]
        public void Invoke_NoException_Passthrough()
        {
            var exitCode = Invoke(new PassThroughCommandHandler(100));

            exitCode.Should().Be(100);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetExceptionData), DynamicDataSourceType.Method)]
        public void Invoke_CaughtException_ReturnsOne(Exception exceptionToThrow)
        {
            var exitCode = Invoke(new ThrowExceptionCommandHandler(exceptionToThrow));

            exitCode.Should().Be(1);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetExpectedExceptionData), DynamicDataSourceType.Method)]
        public void Invoke_CaughtExpectedException_LogsDebug(Exception exceptionToThrow)
        {
            var logger = MockLoggerFactory.CreateLogger();

            Invoke(new ThrowExceptionCommandHandler(exceptionToThrow, logger));

            Mock.Get(logger).Verify(x => x.Log(
                LogLevel.Debug,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                exceptionToThrow,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
        }

        [DataTestMethod]
        [DynamicData(nameof(GetUnexpectedExceptionData), DynamicDataSourceType.Method)]
        public void Invoke_CaughtUnexpectedException_LogsCritical(Exception exceptionToThrow)
        {
            var logger = MockLoggerFactory.CreateLogger();

            Invoke(new ThrowExceptionCommandHandler(exceptionToThrow, logger));

            Mock.Get(logger).Verify(x => x.Log(
                LogLevel.Critical,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                exceptionToThrow,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
        }

        private static int Invoke(ICommandHandler handler)
        {
            var command = new TestCommand()
            {
                Handler = handler,
            };

            return command.Invoke("");
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
                : base(new MockFileSystem(), MockLoggerFactory.CreateLogger())
            {
                this.exitCode = exitCode;
            }

            protected override int InvokeInternal(InvocationContext context) => this.exitCode;
        }

        private class ThrowExceptionCommandHandler : BaseCommandHandler
        {
            private readonly Exception exceptionToThrow;

            public ThrowExceptionCommandHandler(Exception exceptionToThrow, ILogger? logger = null)
                : base(new MockFileSystem(), logger ?? MockLoggerFactory.CreateLogger())
            {
                this.exceptionToThrow = exceptionToThrow;
            }

            protected override int InvokeInternal(InvocationContext context)
            {
                throw exceptionToThrow;
            }
        }
    }
}
