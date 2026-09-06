// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Cli.IntegrationTests
{
    [TestClass]
    public class InvalidArgsTests : TestBase
    {
        [TestMethod]
        public async Task Empty_args_should_fail_with_error()
        {
            var (output, error, result) = await Bicep();

            using (new AssertionScope())
            {
                result.Should().NotBe(0);
                error.Should().NotBeEmpty();
            }
        }

        [TestMethod]
        public async Task Unknown_command_should_fail_with_error()
        {
            var (output, error, result) = await Bicep("notacommand");

            using (new AssertionScope())
            {
                result.Should().NotBe(0);
                error.Should().NotBeEmpty();
            }
        }

        [DataTestMethod]
        // Missing input file (CommandLineException thrown from action)
        [DataRow(new[] { "test" }, "The input file path was not specified")]
        [DataRow(new[] { "decompile" }, "The input file path was not specified")]
        [DataRow(new[] { "generate-params" }, "The input file path was not specified")]
        [DataRow(new[] { "publish" }, "The input file path was not specified")]
        // Missing required option
        [DataRow(new[] { "publish", "file.bicep" }, "The target module was not specified.")]
        // Either input file or --pattern required (thrown from InputOutputArgumentsResolver)
        [DataRow(new[] { "build" }, "Either the input file path or the --pattern parameter must be specified")]
        [DataRow(new[] { "build-params" }, "Either the input file path or the --pattern parameter must be specified")]
        [DataRow(new[] { "restore" }, "Either the input file path or the --pattern parameter must be specified")]
        [DataRow(new[] { "lint" }, "Either the input file path or the --pattern parameter must be specified")]
        [DataRow(new[] { "format" }, "Either the input file path or the --pattern parameter must be specified")]
        // Unrecognized options (rejected by System.CommandLine for commands with TreatUnmatchedTokensAsErrors = true)
        [DataRow(new[] { "build", "--wibble" }, "Unrecognized parameter \"--wibble\"")]
        [DataRow(new[] { "test", "--wibble" }, "Unrecognized parameter \"--wibble\"")]
        [DataRow(new[] { "build-params", "--wibble" }, "Unrecognized parameter \"--wibble\"")]
        [DataRow(new[] { "decompile", "--wibble" }, "Unrecognized parameter \"--wibble\"")]
        [DataRow(new[] { "generate-params", "--wibble" }, "Unrecognized parameter \"--wibble\"")]
        [DataRow(new[] { "restore", "--fake" }, "Unrecognized parameter \"--fake\"")]
        [DataRow(new[] { "lint", "--fake" }, "Unrecognized parameter \"--fake\"")]
        [DataRow(new[] { "--wiggle" }, "Unrecognized command or argument '--wiggle'.")]
        public async Task Invalid_args_should_fail_with_expected_error(string[] args, string expectedError)
        {
            var (output, error, result) = await Bicep(args);

            using (new AssertionScope())
            {
                result.Should().NotBe(0);
                error.Should().Contain(expectedError);
            }
        }

        [TestMethod]
        public async Task JsonRpc_with_pipe_and_socket_should_fail()
        {
            var (output, error, result) = await Bicep("jsonrpc", "--pipe", "foo", "--socket", "1234");

            using (new AssertionScope())
            {
                result.Should().NotBe(0);
                error.Should().Contain("Only one of --pipe, --socket, or --stdio may be specified.");
            }
        }

        [TestMethod]
        public async Task JsonRpc_with_pipe_and_stdio_should_fail()
        {
            var (output, error, result) = await Bicep("jsonrpc", "--pipe", "foo", "--stdio");

            using (new AssertionScope())
            {
                result.Should().NotBe(0);
                error.Should().Contain("Only one of --pipe, --socket, or --stdio may be specified.");
            }
        }

        [TestMethod]
        public async Task JsonRpc_with_socket_and_stdio_should_fail()
        {
            var (output, error, result) = await Bicep("jsonrpc", "--socket", "1234", "--stdio");

            using (new AssertionScope())
            {
                result.Should().NotBe(0);
                error.Should().Contain("Only one of --pipe, --socket, or --stdio may be specified.");
            }
        }

        [TestMethod]
        public async Task Publish_with_invalid_documentation_uri_should_fail()
        {
            var (output, error, result) = await Bicep("publish", "file.bicep", "--target", "br:example.azurecr.io/module:v1", "--documentation-uri", "not-a-uri");

            using (new AssertionScope())
            {
                result.Should().NotBe(0);
                error.Should().Contain("The --documentation-uri should be a well formed uri string.");
            }
        }

        [TestMethod]
        public async Task Publish_with_empty_documentation_uri_should_fail()
        {
            // --documentation-uri present but no value supplied → Tokens.Count == 0
            var (output, error, result) = await Bicep("publish", "file.bicep", "--target", "br:example.azurecr.io/module:v1", "--documentation-uri");

            using (new AssertionScope())
            {
                result.Should().NotBe(0);
                error.Should().Contain("The --documentation-uri parameter expects an argument.");
            }
        }

        [TestMethod]
        public async Task Publish_with_documentation_uri_specified_twice_should_fail()
        {
            // --documentation-uri supplied twice → Tokens.Count > 1
            var (output, error, result) = await Bicep("publish", "file.bicep", "--target", "br:example.azurecr.io/module:v1", "--documentation-uri", "https://example.com", "--documentation-uri", "https://other.com");

            using (new AssertionScope())
            {
                result.Should().NotBe(0);
                error.Should().Contain("The --documentation-uri parameter cannot be specified more than once.");
            }
        }

        [TestMethod]
        public async Task Format_with_stdout_and_outdir_should_fail()
        {
            var (output, error, result) = await Bicep("format", "--stdout", "--outdir", ".", "file.bicep");

            using (new AssertionScope())
            {
                result.Should().NotBe(0);
                error.Should().Contain("The --outdir and --stdout parameters cannot both be used");
            }
        }

        [TestMethod]
        public async Task Format_with_outdir_and_outfile_should_fail()
        {
            var (output, error, result) = await Bicep("format", "--outdir", ".", "--outfile", "out.bicep", "file.bicep");

            using (new AssertionScope())
            {
                result.Should().NotBe(0);
                error.Should().Contain("The --outdir and --outfile parameters cannot both be used");
            }
        }

        [DataTestMethod]
        // build: --pattern conflicts
        [DataRow(new[] { "build", "--stdout", "--pattern", "*.bicep" }, "The --stdout parameter cannot be used with the --pattern parameter")]
        [DataRow(new[] { "build", "--outfile", "foo", "--pattern", "*.bicep" }, "The --outfile parameter cannot be used with the --pattern parameter")]
        // build: output option conflicts
        [DataRow(new[] { "build", "--stdout", "--outdir", ".", "file.bicep" }, "The --outdir and --stdout parameters cannot both be used")]
        [DataRow(new[] { "build", "--stdout", "--outfile", "foo", "file.bicep" }, "The --outfile and --stdout parameters cannot both be used")]
        [DataRow(new[] { "build", "--outdir", ".", "--outfile", "foo", "file.bicep" }, "The --outdir and --outfile parameters cannot both be used")]
        // build-params: --pattern conflicts
        [DataRow(new[] { "build-params", "--bicep-file", "a.bicep", "--pattern", "*.bicepparam" }, "The --bicep-file parameter cannot be used with the --pattern parameter")]
        [DataRow(new[] { "build-params", "--stdout", "--pattern", "*.bicepparam" }, "The --stdout parameter cannot be used with the --pattern parameter")]
        [DataRow(new[] { "build-params", "--outfile", "foo", "--pattern", "*.bicepparam" }, "The --outfile parameter cannot be used with the --pattern parameter")]
        // build-params: output option conflicts
        [DataRow(new[] { "build-params", "--stdout", "--outdir", ".", "file.bicepparam" }, "The --outdir and --stdout parameters cannot both be used")]
        [DataRow(new[] { "build-params", "--stdout", "--outfile", "foo", "file.bicepparam" }, "The --outfile and --stdout parameters cannot both be used")]
        [DataRow(new[] { "build-params", "--outdir", ".", "--outfile", "foo", "file.bicepparam" }, "The --outdir and --outfile parameters cannot both be used")]
        // decompile: output option conflicts
        [DataRow(new[] { "decompile", "--stdout", "--outdir", ".", "file.json" }, "The --outdir and --stdout parameters cannot both be used")]
        [DataRow(new[] { "decompile", "--stdout", "--outfile", "foo", "file.json" }, "The --outfile and --stdout parameters cannot both be used")]
        [DataRow(new[] { "decompile", "--outdir", ".", "--outfile", "foo", "file.json" }, "The --outdir and --outfile parameters cannot both be used")]
        // decompile-params: output option conflicts
        [DataRow(new[] { "decompile-params", "--stdout", "--outdir", ".", "file.json" }, "The --outdir and --stdout parameters cannot both be used")]
        [DataRow(new[] { "decompile-params", "--stdout", "--outfile", "foo", "file.json" }, "The --outfile and --stdout parameters cannot both be used")]
        [DataRow(new[] { "decompile-params", "--outdir", ".", "--outfile", "foo", "file.json" }, "The --outdir and --outfile parameters cannot both be used")]
        // format: --pattern conflicts
        [DataRow(new[] { "format", "--stdout", "--pattern", "*.bicep" }, "The --stdout parameter cannot be used with the --pattern parameter")]
        [DataRow(new[] { "format", "--outdir", ".", "--pattern", "*.bicep" }, "The --outdir parameter cannot be used with the --pattern parameter")]
        [DataRow(new[] { "format", "--outfile", "foo", "--pattern", "*.bicep" }, "The --outfile parameter cannot be used with the --pattern parameter")]
        // format: output option conflicts (--outfile + --stdout missing from old FormatCommand.cs)
        [DataRow(new[] { "format", "--stdout", "--outfile", "foo", "file.bicep" }, "The --outfile and --stdout parameters cannot both be used")]
        // generate-params: output option conflicts
        [DataRow(new[] { "generate-params", "--stdout", "--outdir", ".", "file.bicep" }, "The --outdir and --stdout parameters cannot both be used")]
        [DataRow(new[] { "generate-params", "--stdout", "--outfile", "foo", "file.bicep" }, "The --outfile and --stdout parameters cannot both be used")]
        [DataRow(new[] { "generate-params", "--outdir", ".", "--outfile", "foo", "file.bicep" }, "The --outdir and --outfile parameters cannot both be used")]
        public async Task Conflicting_output_options_should_fail_with_expected_error(string[] args, string expectedError)
        {
            var (output, error, result) = await Bicep(args);

            using (new AssertionScope())
            {
                result.Should().NotBe(0);
                error.Should().Contain(expectedError);
            }
        }

        [TestMethod]
        public async Task PublishExtension_without_input_file_or_binaries_should_fail()
        {
            var (output, error, result) = await Bicep("publish-extension", "--target", "br:example.azurecr.io/ext:v1");

            using (new AssertionScope())
            {
                result.Should().NotBe(0);
                error.Should().Contain("The input file path was not specified.");
            }
        }

        [TestMethod]
        public async Task PublishExtension_without_target_should_fail()
        {
            var (output, error, result) = await Bicep("publish-extension", "index.json");

            using (new AssertionScope())
            {
                result.Should().NotBe(0);
                error.Should().Contain("The target extension was not specified.");
            }
        }
    }
}
