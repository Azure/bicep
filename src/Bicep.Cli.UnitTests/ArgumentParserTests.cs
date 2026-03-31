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
