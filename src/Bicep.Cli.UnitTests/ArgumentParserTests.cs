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
    public class ArgumentParserTests
    {
        private static readonly IOContext IO = new(new StringWriter(), new StringWriter());

        private static readonly IFileSystem FileSystem = new FileSystem();

        [TestMethod]
        public void Empty_parameters_should_return_null()
        {
            var arguments = ArgumentParser.TryParse([], IO, FileSystem);
            arguments.Should().BeNull();
        }

        [TestMethod]
        public void Wrong_command_should_return_null()
        {
            var arguments = ArgumentParser.TryParse(["wrong"], IO, FileSystem);
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
        [DataRow(new[] { "restore", "file1", "file2" }, "The input file path cannot be specified multiple times.")]
        public void Invalid_args_trigger_validation_exceptions(string[] parameters, string expectedException)
        {
            Action parseFunc = () => ArgumentParser.TryParse(parameters, IO, FileSystem);

            parseFunc.Should().Throw<CommandLineException>().WithMessage(expectedException);
        }

        [TestMethod]
        public void BuildOneFile_ShouldReturnOneFile()
        {
            var arguments = ArgumentParser.TryParse(["build", "file1"], IO, FileSystem);
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
            var arguments = ArgumentParser.TryParse(["build", "--stdout", "file1"], IO, FileSystem);
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
            var arguments = ArgumentParser.TryParse(["build", "--stdout", "--no-restore", "file1"], IO, FileSystem);
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
            var arguments = ArgumentParser.TryParse(["build", "--STDOUT", "file1"], IO, FileSystem);
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
            var arguments = ArgumentParser.TryParse(["build", "--outdir", ".", "file1"], IO, FileSystem);
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
            var arguments = ArgumentParser.TryParse(["build", "--outfile", "jsonFile", "file1"], IO, FileSystem);
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
            var arguments = ArgumentParser.TryParse(["build", "--outfile", "jsonFile", "file1", "--no-restore"], IO, FileSystem);
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
            var arguments = ArgumentParser.TryParse(["--license"], IO, FileSystem);

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
            var arguments = ArgumentParser.TryParse(["--third-party-notices"], IO, FileSystem);

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
            var arguments = ArgumentParser.TryParse(["--version"], IO, FileSystem);

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
            var arguments = ArgumentParser.TryParse(["--help"], IO, FileSystem);

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
            var arguments = ArgumentParser.TryParse(["-v"], IO, FileSystem);

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
            var arguments = ArgumentParser.TryParse(["-h"], IO, FileSystem);

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
            var arguments = ArgumentParser.TryParse(["decompile", "file1"], IO, FileSystem);
            var bulidOrDecompileArguments = (DecompileArguments?)arguments;

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
            var arguments = ArgumentParser.TryParse(["decompile", "--stdout", "file1"], IO, FileSystem);
            var bulidOrDecompileArguments = (DecompileArguments?)arguments;

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
            var arguments = ArgumentParser.TryParse(["decompile", "--STDOUT", "file1"], IO, FileSystem);
            var bulidOrDecompileArguments = (DecompileArguments?)arguments;

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
            var arguments = ArgumentParser.TryParse(["decompile", "--outdir", ".", "file1"], IO, FileSystem);
            var bulidOrDecompileArguments = (DecompileArguments?)arguments;

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
            var arguments = ArgumentParser.TryParse(["decompile", "--outfile", "jsonFile", "file1"], IO, FileSystem);
            var bulidOrDecompileArguments = (DecompileArguments?)arguments;

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
            var arguments = ArgumentParser.TryParse(["publish", "file1", "--target", "target1"], IO, FileSystem);
            arguments.Should().BeOfType<PublishArguments>();
            var typed = (PublishArguments)arguments!;

            typed.InputFile.Should().Be("file1");
            typed.TargetModuleReference.Should().Be("target1");
            typed.NoRestore.Should().BeFalse();
        }

        [TestMethod]
        public void Publish_with_no_restore_should_parse_correctly()
        {
            var arguments = ArgumentParser.TryParse(["publish", "file1", "--target", "target1", "--no-restore"], IO, FileSystem);
            arguments.Should().BeOfType<PublishArguments>();
            var typed = (PublishArguments)arguments!;

            typed.InputFile.Should().Be("file1");
            typed.TargetModuleReference.Should().Be("target1");
            typed.NoRestore.Should().BeTrue();
        }

        [TestMethod]
        public void Restore__with_no_force_should_parse_correctly()
        {
            var arguments = ArgumentParser.TryParse(["restore", "file1"], IO, FileSystem);
            arguments.Should().BeOfType<RestoreArguments>();
            var typed = (RestoreArguments)arguments!;

            typed.ForceModulesRestore.Should().Be(false);
            typed.InputFile.Should().Be("file1");
        }

        [TestMethod]
        public void Restore_with_force_should_parse_correctly()
        {
            var arguments = ArgumentParser.TryParse(["restore", "--force", "file1"], IO, FileSystem);
            arguments.Should().BeOfType<RestoreArguments>();
            var typed = (RestoreArguments)arguments!;

            typed.ForceModulesRestore.Should().Be(true);
            typed.InputFile.Should().Be("file1");
        }
    }
}
