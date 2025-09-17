// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.IO.Abstractions;
using Bicep.Cli.Arguments;
using Bicep.Cli.Services;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Cli.UnitTests
{
    [TestClass]
    public class WhatIfOperationResultFormatterTests
    {
        private static readonly IFileSystem FileSystem = new FileSystem();

        [TestMethod]
        public void Empty_parameters_should_return_null()
        {
            var arguments = ArgumentParser.TryParse([], FileSystem);
            arguments.Should().BeNull();
        }

        [TestMethod]
        public void Wrong_command_should_return_null()
        {
            var arguments = ArgumentParser.TryParse(["wrong"], FileSystem);
            arguments.Should().BeNull();
        }

        [DataTestMethod]
        // build
        [DataRow(new[] { "build" }, "Either the input file path or the --pattern parameter must be specified")]
        [DataRow(new[] { "build", "foo.bicep", "--pattern", "*.bicep" }, "The input file path and the --pattern parameter cannot both be specified")]
        [DataRow(new[] { "build", "--pattern" }, "The --pattern parameter expects an argument")]
        [DataRow(new[] { "build", "--pattern", "*.bicep", "--stdout" }, "The --stdout parameter cannot be used with the --pattern parameter")]
        [DataRow(new[] { "build", "--pattern", "*.bicep", "--outfile", "foo" }, "The --outfile parameter cannot be used with the --pattern parameter")]
        [DataRow(new[] { "build", "--stdout" }, "Either the input file path or the --pattern parameter must be specified")]
        [DataRow(new[] { "build", "file1", "file2" }, "The input file path cannot be specified multiple times")]
        [DataRow(new[] { "build", "--wibble" }, "Unrecognized parameter \"--wibble\"")]
        [DataRow(new[] { "build", "--outdir" }, "The --outdir parameter expects an argument")]
        [DataRow(new[] { "build", "--outdir", "dir1", "--outdir", "dir2" }, "The --outdir parameter cannot be specified twice")]
        [DataRow(new[] { "build", "--outfile" }, "The --outfile parameter expects an argument")]
        [DataRow(new[] { "build", "--outfile", "dir1", "--outfile", "dir2" }, "The --outfile parameter cannot be specified twice")]
        [DataRow(new[] { "build", "--stdout", "--outfile", "dir1", "file1" }, "The --outfile and --stdout parameters cannot both be used")]
        [DataRow(new[] { "build", "--stdout", "--outdir", "dir1", "file1" }, "The --outdir and --stdout parameters cannot both be used")]
        [DataRow(new[] { "build", "--outfile", "dir1", "--outdir", "dir2", "file1" }, "The --outdir and --outfile parameters cannot both be used")]
        // build-params
        [DataRow(new[] { "build-params" }, "Either the input file path or the --pattern parameter must be specified")]
        [DataRow(new[] { "build-params", "foo.bicepparam", "--pattern", "*.bicepparam" }, "The input file path and the --pattern parameter cannot both be specified")]
        [DataRow(new[] { "build-params", "--pattern" }, "The --pattern parameter expects an argument")]
        [DataRow(new[] { "build-params", "--pattern", "*.bicepparam", "--stdout" }, "The --stdout parameter cannot be used with the --pattern parameter")]
        [DataRow(new[] { "build-params", "--pattern", "*.bicepparam", "--outfile", "foo" }, "The --outfile parameter cannot be used with the --pattern parameter")]
        [DataRow(new[] { "build-params", "--stdout" }, "Either the input file path or the --pattern parameter must be specified")]
        [DataRow(new[] { "build-params", "file1", "file2" }, "The input file path cannot be specified multiple times")]
        [DataRow(new[] { "build-params", "--wibble" }, "Unrecognized parameter \"--wibble\"")]
        [DataRow(new[] { "build-params", "--outdir" }, "The --outdir parameter expects an argument")]
        [DataRow(new[] { "build-params", "--outdir", "dir1", "--outdir", "dir2" }, "The --outdir parameter cannot be specified twice")]
        [DataRow(new[] { "build-params", "--outfile" }, "The --outfile parameter expects an argument")]
        [DataRow(new[] { "build-params", "--outfile", "dir1", "--outfile", "dir2" }, "The --outfile parameter cannot be specified twice")]
        [DataRow(new[] { "build-params", "--stdout", "--outfile", "dir1", "file1" }, "The --outfile and --stdout parameters cannot both be used")]
        [DataRow(new[] { "build-params", "--stdout", "--outdir", "dir1", "file1" }, "The --outdir and --stdout parameters cannot both be used")]
        [DataRow(new[] { "build-params", "--outfile", "dir1", "--outdir", "dir2", "file1" }, "The --outdir and --outfile parameters cannot both be used")]
        // decompile
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
        // publish
        [DataRow(new[] { "publish" }, "The input file path was not specified.")]
        [DataRow(new[] { "publish", "--fake" }, "Unrecognized parameter \"--fake\"")]
        [DataRow(new[] { "publish", "--target" }, "The --target parameter expects an argument.")]
        [DataRow(new[] { "publish", "--target", "foo", "--target" }, "The --target parameter expects an argument.")]
        [DataRow(new[] { "publish", "--target", "foo", "--target", "foo2" }, "The --target parameter cannot be specified twice.")]
        [DataRow(new[] { "publish", "file" }, "The target module was not specified.")]
        [DataRow(new[] { "publish", "file", "file2" }, "The input file path cannot be specified multiple times.")]
        // restore
        [DataRow(new[] { "restore" }, "Either the input file path or the --pattern parameter must be specified")]
        [DataRow(new[] { "restore", "foo.bicep", "--pattern", "*.bicep" }, "The input file path and the --pattern parameter cannot both be specified")]
        [DataRow(new[] { "restore", "--pattern" }, "The --pattern parameter expects an argument")]
        [DataRow(new[] { "restore", "--fake" }, "Unrecognized parameter \"--fake\"")]
        [DataRow(new[] { "restore", "file1", "file2" }, "The input file path cannot be specified multiple times")]
        // lint
        [DataRow(new[] { "lint" }, "Either the input file path or the --pattern parameter must be specified")]
        [DataRow(new[] { "lint", "foo.bicep", "--pattern", "*.bicep" }, "The input file path and the --pattern parameter cannot both be specified")]
        [DataRow(new[] { "lint", "--pattern" }, "The --pattern parameter expects an argument")]
        [DataRow(new[] { "lint", "--fake" }, "Unrecognized parameter \"--fake\"")]
        // format
        [DataRow(new[] { "format" }, "Either the input file path or the --pattern parameter must be specified")]
        [DataRow(new[] { "format", "foo.bicep", "--pattern", "*.bicep" }, "The input file path and the --pattern parameter cannot both be specified")]
        [DataRow(new[] { "format", "--pattern" }, "The --pattern parameter expects an argument")]
        [DataRow(new[] { "format", "--fake" }, "Unrecognized parameter \"--fake\"")]
        public void Invalid_args_trigger_validation_exceptions(string[] parameters, string expectedException)
        {
            Action parseFunc = () => ArgumentParser.TryParse(parameters, FileSystem);

            parseFunc.Should().Throw<CommandLineException>().WithMessage(expectedException);
        }

        [TestMethod]
        public void BuildOneFile_ShouldReturnOneFile()
        {
            var arguments = ArgumentParser.TryParse(["build", "file1"], FileSystem);
            var buildArguments = (BuildArguments?)arguments;

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
            var arguments = ArgumentParser.TryParse(["build", "--stdout", "file1"], FileSystem);
            var buildArguments = (BuildArguments?)arguments;

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
            var arguments = ArgumentParser.TryParse(["build", "--stdout", "--no-restore", "file1"], FileSystem);
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
            var arguments = ArgumentParser.TryParse(["build", "--STDOUT", "file1"], FileSystem);
            var buildArguments = (BuildArguments?)arguments;

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
            var arguments = ArgumentParser.TryParse(["build", "--outdir", ".", "file1"], FileSystem);
            var buildArguments = (BuildArguments?)arguments;

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
            var arguments = ArgumentParser.TryParse(["build", "--outfile", "jsonFile", "file1"], FileSystem);
            var buildArguments = (BuildArguments?)arguments;

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
            var arguments = ArgumentParser.TryParse(["build", "--outfile", "jsonFile", "file1", "--no-restore"], FileSystem);
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
        public void License_argument_should_return_appropriate_RootArguments_instance()
        {
            var arguments = ArgumentParser.TryParse(["--license"], FileSystem);

            arguments.Should().BeOfType<RootArguments>();
            if (arguments is RootArguments rootArguments)
            {
                rootArguments.PrintHelp.Should().BeFalse();
                rootArguments.PrintVersion.Should().BeFalse();
                rootArguments.PrintLicense.Should().BeTrue();
                rootArguments.PrintThirdPartyNotices.Should().BeFalse();
            }
        }

        [TestMethod]
        public void Third_party_notices_argument_should_return_appropriate_RootArguments_instance()
        {
            var arguments = ArgumentParser.TryParse(["--third-party-notices"], FileSystem);

            arguments.Should().BeOfType<RootArguments>();
            if (arguments is RootArguments rootArguments)
            {
                rootArguments.PrintHelp.Should().BeFalse();
                rootArguments.PrintVersion.Should().BeFalse();
                rootArguments.PrintLicense.Should().BeFalse();
                rootArguments.PrintThirdPartyNotices.Should().BeTrue();
            }
        }

        [TestMethod]
        public void Version_argument_should_return_VersionArguments_instance()
        {
            var arguments = ArgumentParser.TryParse(["--version"], FileSystem);

            arguments.Should().BeOfType<RootArguments>();
            if (arguments is RootArguments rootArguments)
            {
                rootArguments.PrintHelp.Should().BeFalse();
                rootArguments.PrintVersion.Should().BeTrue();
                rootArguments.PrintLicense.Should().BeFalse();
                rootArguments.PrintThirdPartyNotices.Should().BeFalse();
            }
        }

        [TestMethod]
        public void Help_argument_should_return_HelpArguments_instance()
        {
            var arguments = ArgumentParser.TryParse(["--help"], FileSystem);

            arguments.Should().BeOfType<RootArguments>();
            if (arguments is RootArguments rootArguments)
            {
                rootArguments.PrintHelp.Should().BeTrue();
                rootArguments.PrintVersion.Should().BeFalse();
                rootArguments.PrintLicense.Should().BeFalse();
                rootArguments.PrintThirdPartyNotices.Should().BeFalse();
            }
        }

        [TestMethod]
        public void Version_argument_should_return_VersionShortArguments_instance()
        {
            var arguments = ArgumentParser.TryParse(["-v"], FileSystem);

            arguments.Should().BeOfType<RootArguments>();
            if (arguments is RootArguments rootArguments)
            {
                rootArguments.PrintHelp.Should().BeFalse();
                rootArguments.PrintVersion.Should().BeTrue();
                rootArguments.PrintLicense.Should().BeFalse();
                rootArguments.PrintThirdPartyNotices.Should().BeFalse();
            }
        }

        [TestMethod]
        public void Help_argument_should_return_HelpShortArguments_instance()
        {
            var arguments = ArgumentParser.TryParse(["-h"], FileSystem);

            arguments.Should().BeOfType<RootArguments>();
            if (arguments is RootArguments rootArguments)
            {
                rootArguments.PrintHelp.Should().BeTrue();
                rootArguments.PrintVersion.Should().BeFalse();
                rootArguments.PrintLicense.Should().BeFalse();
                rootArguments.PrintThirdPartyNotices.Should().BeFalse();
            }
        }

        [TestMethod]
        public void DecompileOneFile_ShouldReturnOneFile()
        {
            var arguments = ArgumentParser.TryParse(["decompile", "file1"], FileSystem);
            var buildOrDecompileArguments = (DecompileArguments?)arguments;

            // using classic assert so R# understands the value is not null
            Assert.IsNotNull(arguments);
            buildOrDecompileArguments!.InputFile.Should().Be("file1");
            buildOrDecompileArguments!.OutputToStdOut.Should().BeFalse();
            buildOrDecompileArguments!.OutputDir.Should().BeNull();
            buildOrDecompileArguments!.OutputFile.Should().BeNull();
        }

        [TestMethod]
        public void DecompileOneFileStdOut_ShouldReturnOneFileAndStdout()
        {
            var arguments = ArgumentParser.TryParse(["decompile", "--stdout", "file1"], FileSystem);
            var buildOrDecompileArguments = (DecompileArguments?)arguments;

            // using classic assert so R# understands the value is not null
            Assert.IsNotNull(arguments);
            buildOrDecompileArguments!.InputFile.Should().Be("file1");
            buildOrDecompileArguments!.OutputToStdOut.Should().BeTrue();
            buildOrDecompileArguments!.OutputDir.Should().BeNull();
            buildOrDecompileArguments!.OutputFile.Should().BeNull();
        }

        [TestMethod]
        public void DecompileOneFileStdOutAllCaps_ShouldReturnOneFileAndStdout()
        {
            var arguments = ArgumentParser.TryParse(["decompile", "--STDOUT", "file1"], FileSystem);
            var buildOrDecompileArguments = (DecompileArguments?)arguments;

            // using classic assert so R# understands the value is not null
            Assert.IsNotNull(arguments);
            buildOrDecompileArguments!.InputFile.Should().Be("file1");
            buildOrDecompileArguments!.OutputToStdOut.Should().BeTrue();
            buildOrDecompileArguments!.OutputDir.Should().BeNull();
            buildOrDecompileArguments!.OutputFile.Should().BeNull();
        }

        [TestMethod]
        public void Decompile_with_outputdir_parameter_should_parse_correctly()
        {
            // Use relative . to ensure directory exists else the parser will throw.
            var arguments = ArgumentParser.TryParse(["decompile", "--outdir", ".", "file1"], FileSystem);
            var buildOrDecompileArguments = (DecompileArguments?)arguments;

            // using classic assert so R# understands the value is not null
            Assert.IsNotNull(arguments);
            buildOrDecompileArguments!.InputFile.Should().Be("file1");
            buildOrDecompileArguments!.OutputToStdOut.Should().BeFalse();
            buildOrDecompileArguments!.OutputDir.Should().Be(".");
            buildOrDecompileArguments!.OutputFile.Should().BeNull();
        }

        [TestMethod]
        public void Decompile_with_outputfile_parameter_should_parse_correctly()
        {
            var arguments = ArgumentParser.TryParse(["decompile", "--outfile", "jsonFile", "file1"], FileSystem);
            var buildOrDecompileArguments = (DecompileArguments?)arguments;

            // using classic assert so R# understands the value is not null
            Assert.IsNotNull(arguments);
            buildOrDecompileArguments!.InputFile.Should().Be("file1");
            buildOrDecompileArguments!.OutputToStdOut.Should().BeFalse();
            buildOrDecompileArguments!.OutputDir.Should().BeNull();
            buildOrDecompileArguments!.OutputFile.Should().Be("jsonFile");
        }

        [TestMethod]
        public void Publish_should_parse_correctly()
        {
            var arguments = ArgumentParser.TryParse(["publish", "file1", "--target", "target1"], FileSystem);
            arguments.Should().BeOfType<PublishArguments>();
            var typed = (PublishArguments)arguments!;

            typed.InputFile.Should().Be("file1");
            typed.TargetModuleReference.Should().Be("target1");
            typed.NoRestore.Should().BeFalse();
        }

        [TestMethod]
        public void Publish_with_no_restore_should_parse_correctly()
        {
            var arguments = ArgumentParser.TryParse(["publish", "file1", "--target", "target1", "--no-restore"], FileSystem);
            arguments.Should().BeOfType<PublishArguments>();
            var typed = (PublishArguments)arguments!;

            typed.InputFile.Should().Be("file1");
            typed.TargetModuleReference.Should().Be("target1");
            typed.NoRestore.Should().BeTrue();
        }

        [TestMethod]
        public void Restore__with_no_force_should_parse_correctly()
        {
            var arguments = ArgumentParser.TryParse(["restore", "file1"], FileSystem);
            arguments.Should().BeOfType<RestoreArguments>();
            var typed = (RestoreArguments)arguments!;

            typed.ForceModulesRestore.Should().Be(false);
            typed.InputFile.Should().Be("file1");
        }

        [TestMethod]
        public void Restore_with_force_should_parse_correctly()
        {
            var arguments = ArgumentParser.TryParse(["restore", "--force", "file1"], FileSystem);
            arguments.Should().BeOfType<RestoreArguments>();
            var typed = (RestoreArguments)arguments!;

            typed.ForceModulesRestore.Should().Be(true);
            typed.InputFile.Should().Be("file1");
        }
    }
}
