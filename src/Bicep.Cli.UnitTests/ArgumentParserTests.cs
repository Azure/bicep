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
            actual.Should().Contain("bicep build");
            actual.Should().Contain("options");
            actual.Should().Contain("--stdout");
            actual.Should().Contain("bicep decompile");
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

        [DataTestMethod]
        [DataRow(new [] { "build" }, "The input file path was not specified")]
        [DataRow(new [] { "build", "--stdout" }, "The input file path was not specified")]
        [DataRow(new [] { "build", "file1", "file2" }, "The input file path cannot be specified multiple times")]
        [DataRow(new [] { "build", "--wibble" }, "Unrecognized parameter \"--wibble\"")]
        [DataRow(new [] { "build", "--outdir" }, "The --outdir parameter expects an argument")]
        [DataRow(new [] { "build", "--outdir", "dir1", "--outdir", "dir2" }, "The --outdir parameter cannot be specified twice")]
        [DataRow(new [] { "build", "--outfile" }, "The --outfile parameter expects an argument")]
        [DataRow(new [] { "build", "--outfile", "dir1", "--outfile", "dir2" }, "The --outfile parameter cannot be specified twice")]
        [DataRow(new [] { "decompile" }, "The input file path was not specified")]
        [DataRow(new [] { "decompile", "file1", "file2" }, "The input file path cannot be specified multiple times")]
        [DataRow(new [] { "decompile", "--wibble" }, "Unrecognized parameter \"--wibble\"")]
        public void Invalid_args_trigger_validation_exceptions(string[] parameters, string expectedException)
        {
            Action parseFunc = () => ArgumentParser.TryParse(parameters);

            parseFunc.Should().Throw<CommandLineException>().WithMessage(expectedException);
        }

        [TestMethod]
        public void BuildOneFile_ShouldReturnOneFile()
        {
            var arguments = (BuildArguments?)ArgumentParser.TryParse(new[] {"build", "file1"});

            // using classic assert so R# understands the value is not null
            Assert.IsNotNull(arguments);
            arguments!.InputFile.Should().Be("file1");
            arguments!.OutputToStdOut.Should().BeFalse();
            arguments!.OutputDir.Should().BeNull();
            arguments!.OutputFile.Should().BeNull();
        }

        [TestMethod]
        public void BuildOneFileStdOut_ShouldReturnOneFileAndStdout()
        {
            var arguments = (BuildArguments?)ArgumentParser.TryParse(new[] {"build", "--stdout", "file1"});

            // using classic assert so R# understands the value is not null
            Assert.IsNotNull(arguments);
            arguments!.InputFile.Should().Be("file1");
            arguments!.OutputToStdOut.Should().BeTrue();
            arguments!.OutputDir.Should().BeNull();
            arguments!.OutputFile.Should().BeNull();
        }

        [TestMethod]
        public void BuildOneFileStdOutAllCaps_ShouldReturnOneFileAndStdout()
        {
            var arguments = (BuildArguments?)ArgumentParser.TryParse(new[] {"build", "--STDOUT", "file1"});

            // using classic assert so R# understands the value is not null
            Assert.IsNotNull(arguments);
            arguments!.InputFile.Should().Be("file1");
            arguments!.OutputToStdOut.Should().BeTrue();
            arguments!.OutputDir.Should().BeNull();
            arguments!.OutputFile.Should().BeNull();
        }

        [TestMethod]
        public void Build_with_outputdir_parameter_should_parse_correctly()
        {
            var arguments = (BuildArguments?)ArgumentParser.TryParse(new[] {"build", "--outdir", "outdir", "file1"});

            // using classic assert so R# understands the value is not null
            Assert.IsNotNull(arguments);
            arguments!.InputFile.Should().Be("file1");
            arguments!.OutputToStdOut.Should().BeFalse();
            arguments!.OutputDir.Should().Be("outdir");
            arguments!.OutputFile.Should().BeNull();
        }

        [TestMethod]
        public void Build_with_outputfile_parameter_should_parse_correctly()
        {
            var arguments = (BuildArguments?)ArgumentParser.TryParse(new[] {"build", "--outfile", "jsonFile", "file1"});

            // using classic assert so R# understands the value is not null
            Assert.IsNotNull(arguments);
            arguments!.InputFile.Should().Be("file1");
            arguments!.OutputToStdOut.Should().BeFalse();
            arguments!.OutputDir.Should().BeNull();
            arguments!.OutputFile.Should().Be("jsonFile");
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

        [TestMethod]
        public void DecompileOneFile_ShouldReturnOneFile()
        {
            var arguments = ArgumentParser.TryParse(new[] {"decompile", "file1"}) as DecompileArguments;

            arguments!.Should().NotBeNull();
            arguments!.InputFile.Should().Be("file1");
        }
    }
}