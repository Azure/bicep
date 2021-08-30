// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using Bicep.Cli.Services;
using Bicep.Cli.Arguments;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Cli.UnitTests
{
    [TestClass]
    public class ArgumentParserTests
    {
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
        [DataRow(new[] { "build" }, "The input file path was not specified")]
        [DataRow(new[] { "build", "--stdout" }, "The input file path was not specified")]
        [DataRow(new[] { "build", "file1", "file2" }, "The input file path cannot be specified multiple times")]
        [DataRow(new[] { "build", "--wibble" }, "Unrecognized parameter \"--wibble\"")]
        [DataRow(new[] { "build", "--outdir" }, "The --outdir parameter expects an argument")]
        [DataRow(new[] { "build", "--outdir", "dir1", "--outdir", "dir2" }, "The --outdir parameter cannot be specified twice")]
        [DataRow(new[] { "build", "--outfile" }, "The --outfile parameter expects an argument")]
        [DataRow(new[] { "build", "--outfile", "dir1", "--outfile", "dir2" }, "The --outfile parameter cannot be specified twice")]
        [DataRow(new[] { "build", "--stdout", "--outfile", "dir1", "file1" }, "The --outfile and --stdout parameters cannot both be used")]
        [DataRow(new[] { "build", "--stdout", "--outdir", "dir1", "file1" }, "The --outdir and --stdout parameters cannot both be used")]
        [DataRow(new[] { "build", "--outfile", "dir1", "--outdir", "dir2", "file1" }, "The --outdir and --outfile parameters cannot both be used")]
        [DataRow(new[] { "build", "--outdir", "dir1", "file1" }, "The specified output directory \"*\" does not exist.")]
        [DataRow(new[] { "decompile" }, "The input file path was not specified")]
        [DataRow(new[] { "decompile", "--stdout" }, "The input file path was not specified")]
        [DataRow(new[] { "decompile", "file1", "file2" }, "The input file path cannot be specified multiple times")]
        [DataRow(new[] { "decompile", "--wibble" }, "Unrecognized parameter \"--wibble\"")]
        [DataRow(new[] { "decompile", "--outdir" }, "The --outdir parameter expects an argument")]
        [DataRow(new[] { "decompile", "--outdir", "dir1", "--outdir", "dir2" }, "The --outdir parameter cannot be specified twice")]
        [DataRow(new[] { "decompile", "--outfile" }, "The --outfile parameter expects an argument")]
        [DataRow(new[] { "decompile", "--outfile", "dir1", "--outfile", "dir2" }, "The --outfile parameter cannot be specified twice")]
        [DataRow(new[] { "decompile", "--stdout", "--outfile", "dir1", "file1" }, "The --outfile and --stdout parameters cannot both be used")]
        [DataRow(new[] { "decompile", "--stdout", "--outdir", "dir1", "file1" }, "The --outdir and --stdout parameters cannot both be used")]
        [DataRow(new[] { "decompile", "--outfile", "dir1", "--outdir", "dir2", "file1" }, "The --outdir and --outfile parameters cannot both be used")]
        [DataRow(new[] { "decompile", "--outdir", "dir1", "file1" }, "The specified output directory \"*\" does not exist.")]
        [DataRow(new[] { "publish" }, "The input file path was not specified.")]
        [DataRow(new[] { "publish", "--fake" }, "Unrecognized parameter \"--fake\"")]
        [DataRow(new[] { "publish", "--target" }, "The --target parameter expects an argument.")]
        [DataRow(new[] { "publish", "--target", "foo", "--target" }, "The --target parameter expects an argument.")]
        [DataRow(new[] { "publish", "--target", "foo", "--target", "foo2" }, "The --target parameter cannot be specified twice.")]
        [DataRow(new[] { "publish", "file" }, "The target module was not specified.")]
        [DataRow(new[] { "publish", "file", "file2" }, "The input file path cannot be specified multiple times.")]
        [DataRow(new[] { "restore" }, "The input file path was not specified.")]
        [DataRow(new[] { "restore", "--fake" }, "Unrecognized parameter \"--fake\"")]
        [DataRow(new[] { "restore", "file1", "file2"}, "The input file path cannot be specified multiple times.")]
        public void Invalid_args_trigger_validation_exceptions(string[] parameters, string expectedException)
        {
            Action parseFunc = () => ArgumentParser.TryParse(parameters);

            parseFunc.Should().Throw<CommandLineException>().WithMessage(expectedException);
        }

        [TestMethod]
        public void BuildOneFile_ShouldReturnOneFile()
        {
            var arguments = ArgumentParser.TryParse(new[] {"build", "file1"});
            var buildArguments = (BuildArguments?) arguments;

            // using classic assert so R# understands the value is not null
            Assert.IsNotNull(arguments);
            buildArguments!.InputFile.Should().Be("file1");
            buildArguments!.OutputToStdOut.Should().BeFalse();
            buildArguments!.OutputDir.Should().BeNull();
            buildArguments!.OutputFile.Should().BeNull();
            buildArguments!.NoRestore.Should().BeFalse();
        }

        [TestMethod]
        public void BuildOneFileStdOut_ShouldReturnOneFileAndStdout()
        {
            var arguments = ArgumentParser.TryParse(new[] {"build", "--stdout", "file1"});
            var buildArguments = (BuildArguments?) arguments;

            // using classic assert so R# understands the value is not null
            Assert.IsNotNull(arguments);
            buildArguments!.InputFile.Should().Be("file1");
            buildArguments!.OutputToStdOut.Should().BeTrue();
            buildArguments!.OutputDir.Should().BeNull();
            buildArguments!.OutputFile.Should().BeNull();
            buildArguments!.NoRestore.Should().BeFalse();
        }

        [TestMethod]
        public void BuildOneFileStdOut_and_no_restore_ShouldReturnOneFileAndStdout()
        {
            var arguments = ArgumentParser.TryParse(new[] { "build", "--stdout", "--no-restore", "file1" });
            var buildArguments = (BuildArguments?)arguments;

            // using classic assert so R# understands the value is not null
            Assert.IsNotNull(arguments);
            buildArguments!.InputFile.Should().Be("file1");
            buildArguments!.OutputToStdOut.Should().BeTrue();
            buildArguments!.OutputDir.Should().BeNull();
            buildArguments!.OutputFile.Should().BeNull();
            buildArguments!.NoRestore.Should().BeTrue();
        }

        [TestMethod]
        public void BuildOneFileStdOutAllCaps_ShouldReturnOneFileAndStdout()
        {
            var arguments = ArgumentParser.TryParse(new[] {"build", "--STDOUT", "file1"});
            var buildArguments = (BuildArguments?) arguments;

            // using classic assert so R# understands the value is not null
            Assert.IsNotNull(arguments);
            buildArguments!.InputFile.Should().Be("file1");
            buildArguments!.OutputToStdOut.Should().BeTrue();
            buildArguments!.OutputDir.Should().BeNull();
            buildArguments!.OutputFile.Should().BeNull();
            buildArguments!.NoRestore.Should().BeFalse();
        }

        [TestMethod]
        public void Build_with_outputdir_parameter_should_parse_correctly()
        {
            // Use relative . to ensure directory exists else the parser will throw.
            var arguments = ArgumentParser.TryParse(new[] {"build", "--outdir", ".", "file1"}); 
            var buildArguments = (BuildArguments?) arguments;

            // using classic assert so R# understands the value is not null
            Assert.IsNotNull(arguments);
            buildArguments!.InputFile.Should().Be("file1");
            buildArguments!.OutputToStdOut.Should().BeFalse();
            buildArguments!.OutputDir.Should().Be(".");
            buildArguments!.OutputFile.Should().BeNull();
            buildArguments!.NoRestore.Should().BeFalse();
        }


        [TestMethod]
        public void Build_with_outputfile_parameter_should_parse_correctly()
        {
            var arguments = ArgumentParser.TryParse(new[] {"build", "--outfile", "jsonFile", "file1"});
            var buildArguments = (BuildArguments?) arguments;

            // using classic assert so R# understands the value is not null
            Assert.IsNotNull(arguments);
            buildArguments!.InputFile.Should().Be("file1");
            buildArguments!.OutputToStdOut.Should().BeFalse();
            buildArguments!.OutputDir.Should().BeNull();
            buildArguments!.OutputFile.Should().Be("jsonFile");
            buildArguments!.NoRestore.Should().BeFalse();
        }

        [TestMethod]
        public void Build_with_outputfile_and_no_restore_parameter_should_parse_correctly()
        {
            var arguments = ArgumentParser.TryParse(new[] { "build", "--outfile", "jsonFile", "file1", "--no-restore" });
            var buildArguments = (BuildArguments?)arguments;

            // using classic assert so R# understands the value is not null
            Assert.IsNotNull(arguments);
            buildArguments!.InputFile.Should().Be("file1");
            buildArguments!.OutputToStdOut.Should().BeFalse();
            buildArguments!.OutputDir.Should().BeNull();
            buildArguments!.OutputFile.Should().Be("jsonFile");
            buildArguments!.NoRestore.Should().BeTrue();
        }


        [TestMethod]
        public void Version_argument_should_return_VersionArguments_instance()
        {
            var arguments = ArgumentParser.TryParse(new[] { "--version" });

            arguments.Should().BeOfType<RootArguments>();
        }

        [TestMethod]
        public void Help_argument_should_return_HelpArguments_instance()
        {
            var arguments = ArgumentParser.TryParse(new[] { "--help" });

            arguments.Should().BeOfType<RootArguments>();
        }

        [TestMethod]
        public void Version_argument_should_return_VersionShortArguments_instance()
        {
            var arguments = ArgumentParser.TryParse(new[] {"-v"});

            arguments.Should().BeOfType<RootArguments>();
        }

        [TestMethod]
        public void Help_argument_should_return_HelpShortArguments_instance()
        {
            var arguments = ArgumentParser.TryParse(new[] {"-h"});

            arguments.Should().BeOfType<RootArguments>();
        }

        [TestMethod]
        public void DecompileOneFile_ShouldReturnOneFile()
        {
            var arguments = ArgumentParser.TryParse(new[] {"decompile", "file1"});
            var bulidOrDecompileArguments = (DecompileArguments?) arguments;

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
            var arguments = ArgumentParser.TryParse(new[] {"decompile", "--stdout", "file1"});
            var bulidOrDecompileArguments = (DecompileArguments?) arguments;

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
            var arguments = ArgumentParser.TryParse(new[] {"decompile", "--STDOUT", "file1"});
            var bulidOrDecompileArguments = (DecompileArguments?) arguments;

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
            // Use relative . to ensure directory exists else the parser will throw.
            var arguments = ArgumentParser.TryParse(new[] {"decompile", "--outdir", ".", "file1"}); 
            var bulidOrDecompileArguments = (DecompileArguments?) arguments;

            // using classic assert so R# understands the value is not null
            Assert.IsNotNull(arguments);
            bulidOrDecompileArguments!.InputFile.Should().Be("file1");
            bulidOrDecompileArguments!.OutputToStdOut.Should().BeFalse();
            bulidOrDecompileArguments!.OutputDir.Should().Be(".");
            bulidOrDecompileArguments!.OutputFile.Should().BeNull();
        }

        [TestMethod]
        public void Decompile_with_outputfile_parameter_should_parse_correctly()
        {
            var arguments = ArgumentParser.TryParse(new[] {"decompile", "--outfile", "jsonFile", "file1"});
            var bulidOrDecompileArguments = (DecompileArguments?) arguments;

            // using classic assert so R# understands the value is not null
            Assert.IsNotNull(arguments);
            bulidOrDecompileArguments!.InputFile.Should().Be("file1");
            bulidOrDecompileArguments!.OutputToStdOut.Should().BeFalse();
            bulidOrDecompileArguments!.OutputDir.Should().BeNull();
            bulidOrDecompileArguments!.OutputFile.Should().Be("jsonFile");
        }

        [TestMethod]
        public void Publish_should_parse_correctly()
        {
            var arguments = ArgumentParser.TryParse(new[] { "publish", "file1", "--target", "target1" });
            arguments.Should().BeOfType<PublishArguments>();
            var typed = (PublishArguments)arguments!;

            typed.InputFile.Should().Be("file1");
            typed.TargetModuleReference.Should().Be("target1");
            typed.NoRestore.Should().BeFalse();
        }

        [TestMethod]
        public void Publish_with_no_restore_should_parse_correctly()
        {
            var arguments = ArgumentParser.TryParse(new[] { "publish", "file1", "--target", "target1", "--no-restore" });
            arguments.Should().BeOfType<PublishArguments>();
            var typed = (PublishArguments)arguments!;

            typed.InputFile.Should().Be("file1");
            typed.TargetModuleReference.Should().Be("target1");
            typed.NoRestore.Should().BeTrue();
        }

        [TestMethod]
        public void Restore_should_parse_correctly()
        {
            var arguments = ArgumentParser.TryParse(new[] { "restore", "file1" });
            arguments.Should().BeOfType<RestoreArguments>();
            var typed = (RestoreArguments)arguments!;

            typed.InputFile.Should().Be("file1");
        }
    }
}
