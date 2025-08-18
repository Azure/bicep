// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Bicep.Core.UnitTests.Mock;
using Bicep.Local.Extension.Host.CommandLineArguments;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Bicep.Local.Extension.UnitTests.Host.CommandLineArgument;

[TestClass]
public class CommandLineParserTests
{
    private readonly Mock<ILogger<CommandLineParser>> loggerMock;

    public CommandLineParserTests()
    {
        // Set up the logger mock
        loggerMock = StrictMock.Of<ILogger<CommandLineParser>>();
        loggerMock
            .Setup(x => x.Log<It.IsAnyType>(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ));
    }

    [TestMethod]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Arrange
        string[] args = [];
        // Act
        Action act = () => new CommandLineParser(args, null!);
        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [TestMethod]
    public void Constructor_WithNullArgs_ShouldShowHelpAndExitWithCode1()
    {
        // Arrange
        string[] args = null!;

        // Act
        var parser = new CommandLineParser(args, loggerMock.Object);

        // Assert
        parser.ShouldExit.Should().BeTrue();
        parser.ExitCode.Should().Be(1);
        parser.Options.Should().BeNull();
    }

    [TestMethod]
    public void Constructor_WithNoArgs_ShouldShowHelpAndExitWithCode1()
    {
        // Arrange
        var args = Array.Empty<string>();

        // Act
        var parser = new CommandLineParser(args, loggerMock.Object);

        // Assert
        parser.ShouldExit.Should().BeTrue();
        parser.ExitCode.Should().Be(1);
        parser.Options.Should().BeNull();
    }

    [TestMethod]
    public void Constructor_WithHelpArg_ShouldShowHelpAndExitWithCode0()
    {
        // Arrange
        var args = new[] { "--help" };

        // Act
        var parser = new CommandLineParser(args, loggerMock.Object);

        // Assert
        parser.ShouldExit.Should().BeTrue();
        parser.ExitCode.Should().Be(0);
        parser.Options.Should().BeNull();
    }

    [TestMethod]
    public void Constructor_WithVersionArg_ShouldShowHelpAndExitWithCode0()
    {
        // Arrange
        var args = new[] { "--version" };

        // Act
        var parser = new CommandLineParser(args, loggerMock.Object);

        // Assert
        parser.ShouldExit.Should().BeTrue();
        parser.ExitCode.Should().Be(0);
        parser.Options.Should().BeNull();
    }

    [TestMethod]
    public void Constructor_WithInvalidArg_ShouldExitWithCode1()
    {
        // Arrange - pass unknown argument to trigger error
        var args = new[] { "--sockt" };

        // Act
        var parser = new CommandLineParser(args, loggerMock.Object);

        // Assert
        parser.ShouldExit.Should().BeTrue();
        parser.ExitCode.Should().Be(1);
        parser.Options.Should().BeNull();
        parser.Errors.Should().NotBeNull();
        parser.Errors.Should().ContainSingle(e => e.Tag == CommandLine.ErrorType.UnknownOptionError);
    }

    [TestMethod]
    public void Constructor_WithValidSocketArgs_ShouldNotExitAndCreateParserSuccessfully()
    {
        // Arrange
        var args = new[] { "--socket", "/tmp/test.sock" };

        // Act
        var parser = new CommandLineParser(args, loggerMock.Object);

        // Assert
        parser.ShouldExit.Should().BeFalse();
        parser.ExitCode.Should().Be(0);
        parser.Should().NotBeNull();
        parser.Options.Should().NotBeNull();
        parser.Result.Should().NotBeNull();
        parser.Options.Socket.Should().Be("/tmp/test.sock");
        parser.Options.Pipe.Should().BeNull();
        parser.Options.Http.Should().BeNull();
    }

    [TestMethod]
    public void Constructor_WithValidPipeArgs_ShouldNotExitAndCreateParserSuccessfully()
    {
        // Arrange
        var pipeName = "testpipe";
        var args = new[] { "--pipe", pipeName };

        // Act
        var parser = new CommandLineParser(args, loggerMock.Object);

        // Assert
        parser.ShouldExit.Should().BeFalse();
        parser.ExitCode.Should().Be(0);
        parser.Should().NotBeNull();
        parser.Options.Should().NotBeNull();
        parser.Result.Should().NotBeNull();
        parser.Options.Pipe.Should().Be(pipeName);
        parser.Options.Socket.Should().BeNull();
        parser.Options.Http.Should().BeNull();
    }

    [TestMethod]
    public void Constructor_WithValidHttpArgs_ShouldNotExitAndCreateParserSuccessfully()
    {
        // Arrange
        var httpPort = 8080;
        var args = new[] { "--http", httpPort.ToString() };

        // Act
        var parser = new CommandLineParser(args, loggerMock.Object);

        // Assert
        parser.ShouldExit.Should().BeFalse();
        parser.ExitCode.Should().Be(0);
        parser.Should().NotBeNull();
        parser.Options.Should().NotBeNull();
        parser.Result.Should().NotBeNull();
        parser.Options.Http.Should().Be(httpPort);
        parser.Options.Socket.Should().BeNull();
        parser.Options.Pipe.Should().BeNull();
    }

    [TestMethod]
    public void Constructor_WithPipeAndHttpArgs_ShouldNotExitAndParseBothCorrectly()
    {
        // Arrange
        var pipeName = "testpipe";
        var httpPort = 9090;
        var args = new[] { "--pipe", pipeName, "--http", httpPort.ToString() };

        // Act
        var parser = new CommandLineParser(args, loggerMock.Object);

        // Assert
        parser.ShouldExit.Should().BeFalse();
        parser.ExitCode.Should().Be(0);
        parser.Should().NotBeNull();
        parser.Options.Should().NotBeNull();
        parser.Options.Pipe.Should().Be(pipeName);
        parser.Options.Http.Should().Be(httpPort);
        parser.Options.Socket.Should().BeNull();
    }

    [TestMethod]
    public void Constructor_WithAllValidArgs_ShouldNotExitAndParseAllCorrectly()
    {
        // Arrange
        var socketPath = "/tmp/test.sock";
        var pipeName = "testpipe";
        var httpPort = 8080;
        var args = new[] { "--socket", socketPath, "--pipe", pipeName, "--http", httpPort.ToString() };

        // Act
        var parser = new CommandLineParser(args, loggerMock.Object);

        // Assert
        parser.ShouldExit.Should().BeFalse();
        parser.ExitCode.Should().Be(0);
        parser.Should().NotBeNull();
        parser.Options.Should().NotBeNull();
        parser.Options.Socket.Should().Be(socketPath);
        parser.Options.Pipe.Should().Be(pipeName);
        parser.Options.Http.Should().Be(httpPort);
    }

    [TestMethod]
    public void Constructor_WithUnknownArgs_ShouldNotExitAndIgnoreUnknownArgumentsDueToIgnoreUnknownArgumentsSetting()
    {
        // Arrange
        var socketPath = "/tmp/test.sock";
        var args = new[] { "--socket", socketPath, "--unknown-arg", "value" };

        // Act
        var parser = new CommandLineParser(args, loggerMock.Object);

        // Assert
        parser.ShouldExit.Should().BeTrue();
        parser.ExitCode.Should().Be(1);
        parser.Should().NotBeNull();
        parser.Errors.Should().NotBeNull();
        parser.Options.Should().BeNull();
    }

    [TestMethod]
    public void Constructor_WithDuplicateSocketArgs_ShouldExit()
    {
        // Arrange
        var firstSocket = "/tmp/first.sock";
        var secondSocket = "/tmp/second.sock";
        var args = new[] { "--socket", firstSocket, "--socket", secondSocket };

        // Act
        var parser = new CommandLineParser(args, loggerMock.Object);

        // Assert
        parser.ShouldExit.Should().BeTrue();
        parser.ExitCode.Should().Be(1);
        parser.Should().NotBeNull();
        parser.Options.Should().BeNull();
    }

    [TestMethod]
    public void Constructor_WithDuplicatePipeArgs_ShouldExit()
    {
        // Arrange
        var firstPipe = "firstpipe";
        var secondPipe = "secondpipe";
        var args = new[] { "--pipe", firstPipe, "--pipe", secondPipe };

        // Act
        var parser = new CommandLineParser(args, loggerMock.Object);

        // Assert
        parser.ShouldExit.Should().BeTrue();
        parser.ExitCode.Should().Be(1);
        parser.Should().NotBeNull();
        parser.Result.Should().NotBeNull();
        parser.Options.Should().BeNull();
    }

    [TestMethod]
    public void Constructor_WithDuplicateHttpArgs_ShouldExit()
    {
        // Arrange
        var firstPort = 8080;
        var secondPort = 9090;
        var args = new[] { "--http", firstPort.ToString(), "--http", secondPort.ToString() };

        // Act
        var parser = new CommandLineParser(args, loggerMock.Object);

        // Assert
        parser.ShouldExit.Should().BeTrue();
        parser.ExitCode.Should().Be(1);
        parser.Should().NotBeNull();
        parser.Options.Should().BeNull();
    }

    [TestMethod]
    public void Constructor_WithDuplicateWaitForDebuggerArgs_ShouldExit()
    {
        // Arrange
        var args = new[] { "--wait-for-debugger", "--wait-for-debugger" };

        // Act
        var parser = new CommandLineParser(args, loggerMock.Object);

        // Assert
        parser.ShouldExit.Should().BeTrue();
        parser.ExitCode.Should().Be(1);
        parser.Should().NotBeNull();
        parser.Options.Should().BeNull();
    }

    [TestMethod]
    public void Constructor_WithArgumentsContainingSpaces_ShouldNotExitAndParseCorrectly()
    {
        // Arrange - Arguments with spaces should be properly quoted when passed to the application
        var socketPath = "/tmp/path with spaces/test.sock";
        var args = new[] { "--socket", socketPath };

        // Act
        var parser = new CommandLineParser(args, loggerMock.Object);

        // Assert
        parser.ShouldExit.Should().BeFalse();
        parser.ExitCode.Should().Be(0);
        parser.Should().NotBeNull();
        parser.Options.Should().NotBeNull();
        parser.Options.Socket.Should().Be(socketPath);
    }

    [TestMethod]
    public void Constructor_WithEmptyStringArguments_ShouldNotExitAndParseCorrectly()
    {
        // Arrange
        var args = new[] { "--socket", "" };

        // Act
        var parser = new CommandLineParser(args, loggerMock.Object);

        // Assert
        parser.ShouldExit.Should().BeFalse();
        parser.ExitCode.Should().Be(0);
        parser.Should().NotBeNull();
        parser.Options.Should().NotBeNull();
        parser.Options.Socket.Should().Be("");
        parser.Options.Pipe.Should().BeNull();
        parser.Options.Http.Should().BeNull();
    }

    [TestMethod]
    public void Constructor_WithWindowsStylePaths_ShouldNotExitAndParseCorrectly()
    {
        // Arrange
        var socketPath = @"C:\temp\test.sock";
        var pipeName = @"\\.\pipe\testpipe";
        var args = new[] { "--socket", socketPath, "--pipe", pipeName };

        // Act
        var parser = new CommandLineParser(args, loggerMock.Object);

        // Assert
        parser.ShouldExit.Should().BeFalse();
        parser.ExitCode.Should().Be(0);
        parser.Should().NotBeNull();
        parser.Options.Should().NotBeNull();
        parser.Options.Socket.Should().Be(socketPath);
        parser.Options.Pipe.Should().Be(pipeName);
    }

    [TestMethod]
    public void Constructor_WithSpecialCharactersInPaths_ShouldNotExitAndParseCorrectly()
    {
        // Arrange
        var socketPath = "/tmp/test-socket_with.special@chars.sock";
        var pipeName = "pipe-with_special.chars@123";
        var args = new[] { "--socket", socketPath, "--pipe", pipeName };

        // Act
        var parser = new CommandLineParser(args, loggerMock.Object);

        // Assert
        parser.ShouldExit.Should().BeFalse();
        parser.ExitCode.Should().Be(0);
        parser.Should().NotBeNull();
        parser.Options.Should().NotBeNull();
        parser.Options.Socket.Should().Be(socketPath);
        parser.Options.Pipe.Should().Be(pipeName);
    }

    [TestMethod]
    public void Constructor_WithLongArgumentValues_ShouldNotExitAndParseCorrectly()
    {
        // Arrange
        var longSocketPath = "/very/long/path/to/socket/that/might/be/used/in/production/environments/test.sock";
        var longPipeName = "very_long_pipe_name_that_might_be_used_in_production_environments_with_descriptive_names";
        var args = new[] { "--socket", longSocketPath, "--pipe", longPipeName };

        // Act
        var parser = new CommandLineParser(args, loggerMock.Object);

        // Assert
        parser.ShouldExit.Should().BeFalse();
        parser.ExitCode.Should().Be(0);
        parser.Should().NotBeNull();
        parser.Options.Should().NotBeNull();
        parser.Options.Socket.Should().Be(longSocketPath);
        parser.Options.Pipe.Should().Be(longPipeName);
    }

    [TestMethod]
    public void Options_Property_ShouldReturnSameInstanceOnMultipleCalls()
    {
        // Arrange
        var args = new[] { "--socket", "/tmp/test.sock" };
        var parser = new CommandLineParser(args, loggerMock.Object);

        // Act
        var options1 = parser.Options;
        var options2 = parser.Options;

        // Assert
        options1.Should().BeSameAs(options2);
    }

    [TestMethod]
    public void ParserResult_Property_ShouldReturnSameInstanceOnMultipleCalls()
    {
        // Arrange
        var args = new[] { "--socket", "/tmp/test.sock" };
        var parser = new CommandLineParser(args, loggerMock.Object);

        // Act
        var parserResult1 = parser.Result;
        var parserResult2 = parser.Result;

        // Assert
        parserResult1.Should().BeSameAs(parserResult2);
    }

    #region Exit Condition Tests

    [TestMethod]
    public void ExitConditions_WithValidArguments_ShouldNotRequireExit()
    {
        // Arrange & Act
        var validArgSets = new string[][]
        {
            ["--socket", "/tmp/test.sock" ],
            ["--pipe", "testpipe" ],
            ["--http", "8080" ],
            ["--socket", "/tmp/test.sock"],
            ["--pipe", "testpipe"]
        };

        foreach (var args in validArgSets)
        {
            var parser = new CommandLineParser(args, loggerMock.Object);

            // Assert
            parser.ShouldExit.Should().BeFalse($"Valid arguments {string.Join(" ", args)} should not require exit");
            parser.ExitCode.Should().Be(0, $"Valid arguments {string.Join(" ", args)} should have exit code 0");
            parser.Options.Should().NotBeNull($"Valid arguments {string.Join(" ", args)} should produce options");
        }
    }

    [TestMethod]
    public void ExitConditions_WithHelpRequestArguments_ShouldRequireExitCode0()
    {
        // Arrange & Act
        var helpArgSets = new string[][]
        {
            ["--help" ],
            ["--version"]
        };

        foreach (var args in helpArgSets)
        {
            var parser = new CommandLineParser(args, loggerMock.Object);

            // Assert
            parser.ShouldExit.Should().BeTrue($"Help arguments {string.Join(" ", args)} should require exit");
            parser.ExitCode.Should().Be(0, $"Help arguments {string.Join(" ", args)} should have exit code 0");
            parser.Options.Should().BeNull($"Help arguments {string.Join(" ", args)} should not produce options");
        }
    }

    #endregion
}
