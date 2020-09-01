// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.IO;
using System.Text;
using Bicep.Cli.CommandLine;
using Bicep.Cli.CommandLine.Arguments;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Cli.UnitTests
{
    [TestClass]
    public class ArgumentParserTests
    {
        [TestMethod]
        public void PrintUsage_ShouldPrintUsage()
        {
            var actual = TextWriterHelper.InvokeWriterAction(ArgumentParser.PrintUsage);

            actual.Should().Contain("--help");
            actual.Should().Contain("--version");
            actual.Should().Contain("build");
            actual.Should().Contain("options");
            actual.Should().Contain("--stdout");
        }

        [TestMethod]
        public void PrintUsage_ShouldNotThrow()
        {
            ArgumentParser.PrintUsage(Console.Out);
        }

        [TestMethod]
        public void PrintVersion_ShouldPrintVersion()
        {
            var actual = TextWriterHelper.InvokeWriterAction(ArgumentParser.PrintVersion);
            actual.Should().MatchRegex(@"Bicep CLI version \d+\.\d+\.\d+(|-alpha) \([0-9a-f]{10}\)");
        }

        [TestMethod]
        public void PrintVersion_ShouldNotThrow()
        {
            ArgumentParser.PrintVersion(Console.Out);
        }

        [TestMethod]
        public void GetExeName_ShouldReturnExecutableName()
        {
            ArgumentParser.GetExeName().Should().Be("bicep");
        }

        [TestMethod]
        public void Empty_parameters_should_return_null()
        {
            var arguments = ArgumentParser.TryParse(Array.Empty<string>());
            arguments.Should().BeNull();
        }

        [TestMethod]
        public void Wrong_command_should_return_null()
        {
            var arguments = ArgumentParser.TryParse(new[] {"wrong"});
            arguments.Should().BeNull();
        }

        [TestMethod]
        public void BuildNoFiles_ShouldThrow()
        {
            Action noFiles = () => ArgumentParser.TryParse(new[] {"build"});

            noFiles.Should().Throw<CommandLineException>().WithMessage("At least one file must be specified to the build command.");
        }

        [TestMethod]
        public void BuildNoFilesButStdOut_ShouldThrow()
        {
            Action noFiles = () => ArgumentParser.TryParse(new[] {"build", "--stdout"});

            noFiles.Should().Throw<CommandLineException>().WithMessage("At least one file must be specified to the build command.");
        }

        [TestMethod]
        public void BuildOneFile_ShouldReturnOneFile()
        {
            var arguments = (BuildArguments?)ArgumentParser.TryParse(new[] {"build", "file1"});

            // using classic assert so R# understands the value is not null
            Assert.IsNotNull(arguments);
            arguments!.Files.Should().Equal("file1");
            arguments!.OutputToStdOut.Should().BeFalse();
        }

        [TestMethod]
        public void BuildOneFileStdOut_ShouldReturnOneFileAndStdout()
        {
            var arguments = (BuildArguments?)ArgumentParser.TryParse(new[] {"build", "--stdout", "file1"});

            // using classic assert so R# understands the value is not null
            Assert.IsNotNull(arguments);
            arguments!.Files.Should().Equal("file1");
            arguments!.OutputToStdOut.Should().BeTrue();
        }

                [TestMethod]
        public void BuildOneFileStdOutAllCaps_ShouldReturnOneFileAndStdout()
        {
            var arguments = (BuildArguments?)ArgumentParser.TryParse(new[] {"build", "--STDOUT", "file1"});

            // using classic assert so R# understands the value is not null
            Assert.IsNotNull(arguments);
            arguments!.Files.Should().Equal("file1");
            arguments!.OutputToStdOut.Should().BeTrue();
        }

        [TestMethod]
        public void BuildMultipleFiles_ShouldReturnAllFiles()
        {
            var arguments = (BuildArguments?) ArgumentParser.TryParse(new[] {"build", "file1", "file2", "file3"});

            Assert.IsNotNull(arguments);
            arguments!.Files.Should().Equal("file1", "file2", "file3");
            arguments!.OutputToStdOut.Should().BeFalse();
        }

        [TestMethod]
        public void BuildMultipleFilesStdOut_ShouldReturnAllFilesAndStdOut()
        {
            var arguments = (BuildArguments?) ArgumentParser.TryParse(new[] {"build", "--stdout", "file1", "file2", "file3"});

            Assert.IsNotNull(arguments);
            arguments!.Files.Should().Equal("file1", "file2", "file3");
            arguments!.OutputToStdOut.Should().BeTrue();
        }

        [TestMethod]
        public void BuildMultipleFilesStdOutTwice_ShouldReturnAllFilesAndStdOut()
        {
            var arguments = (BuildArguments?) ArgumentParser.TryParse(new[] {"build", "--stdout", "file1", "file2", "--stdout", "file3"});

            Assert.IsNotNull(arguments);
            arguments!.Files.Should().Equal("file1", "file2", "file3");
            arguments!.OutputToStdOut.Should().BeTrue();
        }

        [TestMethod]
        public void Version_argument_should_return_VersionArguments_instance()
        {
            var arguments = ArgumentParser.TryParse(new[] { "--version" });

            arguments.Should().BeOfType<VersionArguments>();
        }

        [TestMethod]
        public void Help_argument_should_return_HelpArguments_instance()
        {
            var arguments = ArgumentParser.TryParse(new[] { "--help" });

            arguments.Should().BeOfType<HelpArguments>();
        }

        [TestMethod]
        public void Version_argument_should_return_VersionShortArguments_instance()
        {
            var arguments = ArgumentParser.TryParse(new[] {"-v"});

            arguments.Should().BeOfType<VersionArguments>();
        }

        [TestMethod]
        public void Help_argument_should_return_HelpShortArguments_instance()
        {
            var arguments = ArgumentParser.TryParse(new[] {"-h"});

            arguments.Should().BeOfType<HelpArguments>();
        }
    }
}
