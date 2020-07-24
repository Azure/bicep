using System;
using Bicep.Cli.CommandLine;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Cli.UnitTests
{
    [TestClass]
    public class ArgumentParserTests
    {
        [TestMethod]
        public void PrintUsage_ShouldNotThrow()
        {
            ArgumentParser.PrintUsage();
        }

        [TestMethod]
        public void NullOrEmptyParameters_ShouldReturnNull()
        {
            ArgumentParser.Parse(Array.Empty<string>()).Should().BeNull();
        }

        [TestMethod]
        public void WrongCommand_ShouldThrow()
        {
            Action wrongCommand = () => ArgumentParser.Parse(new[] {"wrong"});
            wrongCommand.Should().Throw<CommandLineException>().WithMessage("Unexpected command 'wrong' was specified. Valid command include: Build");
        }

        [TestMethod]
        public void BuildNoFiles_ShouldThrow()
        {
            Action noFiles = () => ArgumentParser.Parse(new[] {"build"});

            noFiles.Should().Throw<CommandLineException>().WithMessage("At least one file must be specified to the Build command.");
        }

        [TestMethod]
        public void BuildNoFilesButStdOut_ShouldThrow()
        {
            Action noFiles = () => ArgumentParser.Parse(new[] {"build", "--stdout"});

            noFiles.Should().Throw<CommandLineException>().WithMessage("At least one file must be specified to the Build command.");
        }

        [TestMethod]
        public void BuildOneFile_ShouldReturnOneFile()
        {
            var arguments = (BuildArguments?)ArgumentParser.Parse(new[] {"build", "file1"});

            // using classic assert so R# understands the value is not null
            Assert.IsNotNull(arguments);
            arguments!.Files.Should().Equal("file1");
            arguments!.OutputToStdOut.Should().BeFalse();
        }

        [TestMethod]
        public void BuildOneFileStdOut_ShouldReturnOneFileAndStdout()
        {
            var arguments = (BuildArguments?)ArgumentParser.Parse(new[] {"build", "--stdout", "file1"});

            // using classic assert so R# understands the value is not null
            Assert.IsNotNull(arguments);
            arguments!.Files.Should().Equal("file1");
            arguments!.OutputToStdOut.Should().BeTrue();
        }

                [TestMethod]
        public void BuildOneFileStdOutAllCaps_ShouldReturnOneFileAndStdout()
        {
            var arguments = (BuildArguments?)ArgumentParser.Parse(new[] {"build", "--STDOUT", "file1"});

            // using classic assert so R# understands the value is not null
            Assert.IsNotNull(arguments);
            arguments!.Files.Should().Equal("file1");
            arguments!.OutputToStdOut.Should().BeTrue();
        }

        [TestMethod]
        public void BuildMultipleFiles_ShouldReturnAllFiles()
        {
            var arguments = (BuildArguments?) ArgumentParser.Parse(new[] {"build", "file1", "file2", "file3"});

            Assert.IsNotNull(arguments);
            arguments!.Files.Should().Equal("file1", "file2", "file3");
            arguments!.OutputToStdOut.Should().BeFalse();
        }

        [TestMethod]
        public void BuildMultipleFilesStdOut_ShouldReturnAllFilesAndStdOut()
        {
            var arguments = (BuildArguments?) ArgumentParser.Parse(new[] {"build", "--stdout", "file1", "file2", "file3"});

            Assert.IsNotNull(arguments);
            arguments!.Files.Should().Equal("file1", "file2", "file3");
            arguments!.OutputToStdOut.Should().BeTrue();
        }

        [TestMethod]
        public void BuildMultipleFilesStdOutTwice_ShouldReturnAllFilesAndStdOut()
        {
            var arguments = (BuildArguments?) ArgumentParser.Parse(new[] {"build", "--stdout", "file1", "file2", "--stdout", "file3"});

            Assert.IsNotNull(arguments);
            arguments!.Files.Should().Equal("file1", "file2", "file3");
            arguments!.OutputToStdOut.Should().BeTrue();
        }
    }
}
