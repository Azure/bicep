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
        [DataRow(new [] { "build", "--stdout", "--outfile", "dir1", "file1" }, "The --outfile and --stdout parameters cannot both be used")]
        [DataRow(new [] { "build", "--stdout", "--outdir", "dir1", "file1" }, "The --outdir and --stdout parameters cannot both be used")]
        [DataRow(new [] { "build", "--outfile", "dir1", "--outdir", "dir2", "file1" }, "The --outdir and --outfile parameters cannot both be used")]
        [DataRow(new [] { "decompile" }, "The input file path was not specified")]
        [DataRow(new [] { "decompile", "--stdout" }, "The input file path was not specified")]
        [DataRow(new [] { "decompile", "file1", "file2" }, "The input file path cannot be specified multiple times")]
        [DataRow(new [] { "decompile", "--wibble" }, "Unrecognized parameter \"--wibble\"")]
        [DataRow(new [] { "decompile", "--outdir" }, "The --outdir parameter expects an argument")]
        [DataRow(new [] { "decompile", "--outdir", "dir1", "--outdir", "dir2" }, "The --outdir parameter cannot be specified twice")]
        [DataRow(new [] { "decompile", "--outfile" }, "The --outfile parameter expects an argument")]
        [DataRow(new [] { "decompile", "--outfile", "dir1", "--outfile", "dir2" }, "The --outfile parameter cannot be specified twice")]
        [DataRow(new [] { "decompile", "--stdout", "--outfile", "dir1", "file1" }, "The --outfile and --stdout parameters cannot both be used")]
        [DataRow(new [] { "decompile", "--stdout", "--outdir", "dir1", "file1" }, "The --outdir and --stdout parameters cannot both be used")]
        [DataRow(new [] { "decompile", "--outfile", "dir1", "--outdir", "dir2", "file1" }, "The --outdir and --outfile parameters cannot both be used")]
        public void Invalid_args_trigger_validation_exceptions(string[] parameters, string expectedException)
        {
            Action parseFunc = () => ArgumentParser.TryParse(parameters);

            parseFunc.Should().Throw<CommandLineException>().WithMessage(expectedException);
        }

        [TestMethod]
        public void BuildOneFile_ShouldReturnOneFile()
        {
            var arguments = ArgumentParser.TryParse(new[] {"build", "file1"});
            var bulidOrDecompileArguments = (BuildOrDecompileArguments?) arguments;

            // using classic assert so R# understands the value is not null
            Assert.IsNotNull(arguments);
            bulidOrDecompileArguments!.InputFile.Should().Be("file1");
            bulidOrDecompileArguments!.OutputToStdOut.Should().BeFalse();
            bulidOrDecompileArguments!.OutputDir.Should().BeNull();
            bulidOrDecompileArguments!.OutputFile.Should().BeNull();
        }

        [TestMethod]
        public void BuildOneFileStdOut_ShouldReturnOneFileAndStdout()
        {
            var arguments = ArgumentParser.TryParse(new[] {"build", "--stdout", "file1"});
            var bulidOrDecompileArguments = (BuildOrDecompileArguments?) arguments;

            // using classic assert so R# understands the value is not null
            Assert.IsNotNull(arguments);
            bulidOrDecompileArguments!.InputFile.Should().Be("file1");
            bulidOrDecompileArguments!.OutputToStdOut.Should().BeTrue();
            bulidOrDecompileArguments!.OutputDir.Should().BeNull();
            bulidOrDecompileArguments!.OutputFile.Should().BeNull();
        }

        [TestMethod]
        public void BuildOneFileStdOutAllCaps_ShouldReturnOneFileAndStdout()
        {
            var arguments = ArgumentParser.TryParse(new[] {"build", "--STDOUT", "file1"});
            var bulidOrDecompileArguments = (BuildOrDecompileArguments?) arguments;

            // using classic assert so R# understands the value is not null
            Assert.IsNotNull(arguments);
            bulidOrDecompileArguments!.InputFile.Should().Be("file1");
            bulidOrDecompileArguments!.OutputToStdOut.Should().BeTrue();
            bulidOrDecompileArguments!.OutputDir.Should().BeNull();
            bulidOrDecompileArguments!.OutputFile.Should().BeNull();
        }

        [TestMethod]
        public void Build_with_outputdir_parameter_should_parse_correctly()
        {
            var arguments = ArgumentParser.TryParse(new[] {"build", "--outdir", "outdir", "file1"});
            var bulidOrDecompileArguments = (BuildOrDecompileArguments?) arguments;

            // using classic assert so R# understands the value is not null
            Assert.IsNotNull(arguments);
            bulidOrDecompileArguments!.InputFile.Should().Be("file1");
            bulidOrDecompileArguments!.OutputToStdOut.Should().BeFalse();
            bulidOrDecompileArguments!.OutputDir.Should().Be("outdir");
            bulidOrDecompileArguments!.OutputFile.Should().BeNull();
        }

        [TestMethod]
        public void Build_with_outputfile_parameter_should_parse_correctly()
        {
            var arguments = ArgumentParser.TryParse(new[] {"build", "--outfile", "jsonFile", "file1"});
            var bulidOrDecompileArguments = (BuildOrDecompileArguments?) arguments;

            // using classic assert so R# understands the value is not null
            Assert.IsNotNull(arguments);
            bulidOrDecompileArguments!.InputFile.Should().Be("file1");
            bulidOrDecompileArguments!.OutputToStdOut.Should().BeFalse();
            bulidOrDecompileArguments!.OutputDir.Should().BeNull();
            bulidOrDecompileArguments!.OutputFile.Should().Be("jsonFile");
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
            var arguments = ArgumentParser.TryParse(new[] {"build", "file1"});
            var bulidOrDecompileArguments = (BuildOrDecompileArguments?) arguments;

            // using classic assert so R# understands the value is not null
            Assert.IsNotNull(arguments);
            bulidOrDecompileArguments!.InputFile.Should().Be("file1");
            bulidOrDecompileArguments!.OutputToStdOut.Should().BeFalse();
            bulidOrDecompileArguments!.OutputDir.Should().BeNull();
            bulidOrDecompileArguments!.OutputFile.Should().BeNull();
        }

        [TestMethod]
        public void DecompileOneFileStdOut_ShouldReturnOneFileAndStdout()
        {
            var arguments = ArgumentParser.TryParse(new[] {"build", "--stdout", "file1"});
            var bulidOrDecompileArguments = (BuildOrDecompileArguments?) arguments;

            // using classic assert so R# understands the value is not null
            Assert.IsNotNull(arguments);
            bulidOrDecompileArguments!.InputFile.Should().Be("file1");
            bulidOrDecompileArguments!.OutputToStdOut.Should().BeTrue();
            bulidOrDecompileArguments!.OutputDir.Should().BeNull();
            bulidOrDecompileArguments!.OutputFile.Should().BeNull();
        }

        [TestMethod]
        public void DecompileOneFileStdOutAllCaps_ShouldReturnOneFileAndStdout()
        {
            var arguments = ArgumentParser.TryParse(new[] {"build", "--STDOUT", "file1"});
            var bulidOrDecompileArguments = (BuildOrDecompileArguments?) arguments;

            // using classic assert so R# understands the value is not null
            Assert.IsNotNull(arguments);
            bulidOrDecompileArguments!.InputFile.Should().Be("file1");
            bulidOrDecompileArguments!.OutputToStdOut.Should().BeTrue();
            bulidOrDecompileArguments!.OutputDir.Should().BeNull();
            bulidOrDecompileArguments!.OutputFile.Should().BeNull();
        }

        [TestMethod]
        public void Decompile_with_outputdir_parameter_should_parse_correctly()
        {
            var arguments = ArgumentParser.TryParse(new[] {"build", "--outdir", "outdir", "file1"});
            var bulidOrDecompileArguments = (BuildOrDecompileArguments?) arguments;

            // using classic assert so R# understands the value is not null
            Assert.IsNotNull(arguments);
            bulidOrDecompileArguments!.InputFile.Should().Be("file1");
            bulidOrDecompileArguments!.OutputToStdOut.Should().BeFalse();
            bulidOrDecompileArguments!.OutputDir.Should().Be("outdir");
            bulidOrDecompileArguments!.OutputFile.Should().BeNull();
        }

        [TestMethod]
        public void Decompile_with_outputfile_parameter_should_parse_correctly()
        {
            var arguments = ArgumentParser.TryParse(new[] {"build", "--outfile", "jsonFile", "file1"});
            var bulidOrDecompileArguments = (BuildOrDecompileArguments?) arguments;

            // using classic assert so R# understands the value is not null
            Assert.IsNotNull(arguments);
            bulidOrDecompileArguments!.InputFile.Should().Be("file1");
            bulidOrDecompileArguments!.OutputToStdOut.Should().BeFalse();
            bulidOrDecompileArguments!.OutputDir.Should().BeNull();
            bulidOrDecompileArguments!.OutputFile.Should().Be("jsonFile");
        }
    }
}
