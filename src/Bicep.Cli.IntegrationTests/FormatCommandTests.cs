// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions.TestingHelpers;
using System.Text.RegularExpressions;
using Bicep.Cli.UnitTests;
using Bicep.Core.PrettyPrintV2;
using Bicep.Core.Samples;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Cli.IntegrationTests
{
    [TestClass]
    public partial class FormatCommandTests : TestBase
    {
        [TestMethod]
        public async Task Format_WithDeprecatedParams_PrintsDeprecationMessage()
        {
            var bicepPath = FileHelper.SaveResultFile(TestContext, "input.bicep", "output myOutput string = 'hello!'");

            var (output, error, result) = await Bicep("format", bicepPath, "--newline", "crlf", "--indentKind", "space", "--indentSize", "4", "--insertFinalNewline");

            result.Should().Be(0);
            output.Should().BeEmpty();
            error.Should().MatchRegex(@"DEPRECATED: The parameter --newline is deprecated and will be removed in a future version of Bicep CLI. Use --newline-kind instead.");
            error.Should().MatchRegex(@"DEPRECATED: The parameter --indentKind is deprecated and will be removed in a future version of Bicep CLI. Use --indent-kind instead.");
            error.Should().MatchRegex(@"DEPRECATED: The parameter --indentSize is deprecated and will be removed in a future version of Bicep CLI. Use --indent-size instead.");
            error.Should().MatchRegex(@"DEPRECATED: The parameter --insertFinalNewline is deprecated and will be removed in a future version of Bicep CLI. Use --insert-final-newline instead.");
        }

        [TestMethod]
        public async Task Format_WithLegacyFormatterEnabled_InvokesLegacyFormatter()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                ["main.bicep"] = "var obj = {'longString': 'loooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooong'}",
            });

            var result = await Bicep(
                services => services
                    .WithFeatureOverrides(new FeatureProviderOverrides(LegacyFormatterEnabled: true))
                    .WithFileSystem(fileSystem),
                "format",
                "main.bicep");

            AssertSuccess(result);

            fileSystem.FileExists("main.bicep").Should().BeTrue();
            fileSystem.File.ReadAllText("main.bicep").Should().Be("var obj = { 'longString': 'loooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooong' }\n");

        }

        [TestMethod]
        public async Task Format_MissingInputFilePath_Fails()
        {
            var result = await Bicep("format");

            AssertFailure(result, "Either the input file path or the --pattern parameter must be specified");
        }

        [TestMethod]
        public async Task Format_NonBicepFile_Fails()
        {
            var result = await Bicep("format", "/dev/zero");

            AssertFailure(result, "The specified input \"/dev/zero\" was not recognized as a Bicep or Bicep Parameters file. Valid files must either the .bicep or .bicepparam extension.");
        }

        [TestMethod]
        public async Task Format_LockedOutputFile_Fails()
        {
            var inputFile = FileHelper.SaveResultFile(this.TestContext, "Empty.bicep", DataSets.Empty.Bicep);

            // ReSharper disable once ConvertToUsingDeclaration
            using (new FileStream(inputFile, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            {
                // keep the output stream open while we attempt to write to it
                // this should force an access denied error
                var result = await Bicep("format", inputFile);

                AssertFailure(result, "Empty.bicep");
            }
        }

        [TestMethod]
        public async Task Format_WithBothOutdirAndStdoutSpecified_Fails()
        {
            var bicepPath = FileHelper.SaveResultFile(TestContext, "input.bicep", "output myOutput string = 'hello!'");
            var outputDirectory = FileHelper.GetResultFilePath(TestContext, "outputdir");

            var result = await Bicep("format", "--outdir", outputDirectory, "--stdout", bicepPath);

            AssertFailure(result, "The --outdir and --stdout parameters cannot both be used");
        }

        [TestMethod]
        public async Task Format_WithBothOutdirAndOutfileSpecified_Fails()
        {
            var bicepPath = FileHelper.SaveResultFile(TestContext, "input.bicep", "output myOutput string = 'hello!'");

            var outputDirectory = FileHelper.GetResultFilePath(TestContext, "outputdir");

            var result = await Bicep("format", "--outdir", outputDirectory, "--outfile", "foo.bicep", bicepPath);

            AssertFailure(result, "The --outdir and --outfile parameters cannot both be used");
        }

        [DataRow("DoesNotExist.bicep", new[] { "--stdout" }, @"An error occurred reading file. Could not find file '.+DoesNotExist.bicep'")]
        [DataRow("DoesNotExist.bicep", new[] { "--outdir", "." }, @"An error occurred reading file. Could not find file '.+DoesNotExist.bicep'")]
        [DataRow("DoesNotExist.bicep", new[] { "--outfile", "file1" }, @"An error occurred reading file. Could not find file '.+DoesNotExist.bicep'")]
        [DataRow("WrongDir\\Fake.bicep", new[] { "--stdout" }, @"An error occurred reading file. Could not find .+'.+WrongDir[\\/]Fake.bicep'")]
        [DataRow("WrongDir\\Fake.bicep", new[] { "--outdir", "." }, @"An error occurred reading file. Could not find .+'.+WrongDir[\\/]Fake.bicep'")]
        [DataRow("WrongDir\\Fake.bicep", new[] { "--outfile", "file1" }, @"An error occurred reading file. Could not find .+'.+WrongDir[\\/]Fake.bicep'")]
        [DataTestMethod]
        public async Task Format_InvalidInputPath_Fails(string badPath, string[] args, string expectedErrorPattern)
        {
            var result = await Bicep(["format", .. args, badPath]);

            AssertFailure(result, expectedErrorPattern);
        }

        [TestMethod]
        public async Task Format_NonExistentOutDir_Fails()
        {
            var bicepPath = FileHelper.SaveResultFile(TestContext, "input.bicep", "iutput myOutput string = 'hello!'");
            var outputFileDir = FileHelper.GetResultFilePath(TestContext, "outputdir");

            var result = await Bicep("format", "--outdir", outputFileDir, bicepPath);

            AssertFailure(result, @"The specified output directory "".*outputdir"" does not exist");
        }

        [DataTestMethod]
        [BaselineData_Bicepparam.TestData()]
        public async Task Format_SampleBicepParam_MatchesFormattedSample(BaselineData_Bicepparam baselineData)
        {
            var data = baselineData.GetData(TestContext);

            data.Formatted.WriteToOutputFolder(data.Parameters.EmbeddedFile.Contents);
            var result = await Bicep("format", data.Formatted.OutputFilePath);

            AssertSuccess(result);

            data.Formatted.ShouldHaveExpectedValue();
        }

        [DataTestMethod]
        [DynamicData(nameof(GetDataSets), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task Format_SampleBicepFile_MatchesFormattedSample(DataSet dataSet)
        {
            var outputDirectory = dataSet.SaveFilesToTestDirectory(TestContext);
            var bicepFilePath = Path.Combine(outputDirectory, DataSet.TestFileMain);

            var result = await Bicep("format", bicepFilePath);

            AssertSuccess(result);

            var actual = File.ReadAllText(bicepFilePath);
            actual.Should().Be(dataSet.Formatted);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task Format_should_format_files_matching_pattern(bool useRootPath)
        {
            var unformatted = """
output     myOutput     string    =    'hello!'
""";
            var formatted = """
output myOutput string = 'hello!'

""";

            var outputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
            var fileResults = new[]
            {
                (input: "file1.bicep", expectOutput: true),
                (input: "file2.bicep", expectOutput: true),
                (input: "nofile.bicep", expectOutput: false)
            };

            foreach (var (input, _) in fileResults)
            {
                FileHelper.SaveResultFile(TestContext, input, unformatted, outputPath);
            }

            var (output, error, result) = await Bicep(
                services => services.WithEnvironment(useRootPath ? TestEnvironment.Default : TestEnvironment.Default with { CurrentDirectory = outputPath }),
                ["format",
                    "--pattern",
                    useRootPath ? $"{outputPath}/file*.bicep" : "file*.bicep"]);

            result.Should().Be(0);
            error.Should().BeEmpty();
            output.Should().BeEmpty();

            foreach (var (input, expectOutput) in fileResults)
            {
                File.ReadAllText(Path.Combine(outputPath, input)).Should().Be(expectOutput ? formatted : unformatted);
            }
        }

        [TestMethod]
        public async Task Format_WithOutFile_WritesContentToOutFile()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                ["main.bicep"] = "var number = 42",
            });

            var result = await Bicep(services => services.WithFileSystem(fileSystem), "format", "main.bicep", "--outfile", "somefile.bicep");

            AssertSuccess(result);

            fileSystem.FileExists("somefile.bicep").Should().BeTrue();
        }

        [TestMethod]
        public async Task Format_WithStdout_WritesContentToStdout()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                ["main.bicep"] = "var number = 42",
            });

            var result = await Bicep(services => services.WithFileSystem(fileSystem), "format", "main.bicep", "--stdout");

            AssertSuccess(result);

            result.Stdout.Should().BeEquivalentToIgnoringNewlines("""
                var number = 42

                """);
        }

        [TestMethod]
        public async Task Format_WithOutdir_SavesFileToOutdir()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                ["main.bicep"] = "var number = 42",
                ["some-directory"] = new MockDirectoryData(),
            });

            var result = await Bicep(services => services.WithFileSystem(fileSystem), "format", "main.bicep", "--outdir", "some-directory");

            AssertSuccess(result);

            fileSystem.FileExists("some-directory/main.bicep").Should().BeTrue();
        }

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task Format_WithInsertFinalNewlineOverride_SetsFinalNewlineAccordingly(bool insertFinalNewline)
        {
            var fileContentWithoutFinalNewline = """
                var obj = {
                  foo: true
                  bar: 123
                }
                """;

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                ["main.bicep"] = insertFinalNewline
                    ? fileContentWithoutFinalNewline
                    : $"{fileContentWithoutFinalNewline}\n",
            });

            var result = await Bicep(services => services.WithFileSystem(fileSystem), "format", "main.bicep", "--insert-final-newline", insertFinalNewline.ToString());

            AssertSuccess(result);

            var formatted = fileSystem.File.ReadAllText("main.bicep");
            formatted.Should().BeEquivalentToIgnoringNewlines(insertFinalNewline
                ? $"{fileContentWithoutFinalNewline}\n"
                : fileContentWithoutFinalNewline);
        }

        [DataTestMethod]
        [DataRow(IndentKind.Space)]
        [DataRow(IndentKind.Tab)]
        public async Task Format_WithIndentKindOverride_SetsIndentKindAccordingly(IndentKind indentKind)
        {
            var fileContentTemplate = """
                var obj = {{
                {0}foo: true
                {0}bar: 123
                }}

                """;

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                ["main.bicep"] = string.Format(fileContentTemplate, indentKind is IndentKind.Space ? "\t" : "  "),
            });

            var result = await Bicep(services => services.WithFileSystem(fileSystem), "format", "main.bicep", "--indent-kind", indentKind.ToString());

            AssertSuccess(result);

            var formatted = fileSystem.File.ReadAllText("main.bicep");
            var expected = string.Format(fileContentTemplate, indentKind is IndentKind.Space ? "  " : "\t");
            formatted.Should().BeEquivalentToIgnoringNewlines(expected);
        }

        [DataTestMethod]
        [DataRow(1)]
        [DataRow(4)]
        [DataRow(8)]
        public async Task Format_WithIndentSizeOverride_SetsIndentSizeAccordingly(int indentSize)
        {
            var fileContentTemplate = """
                var obj = {{
                {0}foo: true
                {0}bar: 123
                }}

                """;

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                ["main.bicep"] = string.Format(fileContentTemplate, ""),
            });

            var result = await Bicep(services => services.WithFileSystem(fileSystem), "format", "main.bicep", "--indent-size", indentSize.ToString());

            AssertSuccess(result);

            var formatted = fileSystem.File.ReadAllText("main.bicep");
            var expected = string.Format(fileContentTemplate, new string(' ', indentSize));
            formatted.Should().BeEquivalentToIgnoringNewlines(expected);
        }

        [TestMethod]
        public async Task Format_WithInsertFinalNewline_AddsFinalNewline()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                ["main.bicep"] = "output myOutput string = 'hello!'",
            });

            var result = await Bicep(services => services.WithFileSystem(fileSystem), "format", "main.bicep", "--insert-final-newline");

            AssertSuccess(result);

            var formatted = fileSystem.File.ReadAllText("main.bicep");
            formatted.Should().BeEquivalentTo("output myOutput string = 'hello!'\n");

            result = await Bicep(services => services.WithFileSystem(fileSystem), "format", "main.bicep", "--insert-final-newline", "true");

            AssertSuccess(result);

            formatted = fileSystem.File.ReadAllText("main.bicep");
            formatted.Should().BeEquivalentTo("output myOutput string = 'hello!'\n");
        }

        [TestMethod]
        public async Task Format_WithInsertFinalNewlineFalse_RemovesFinalNewline()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                ["main.bicep"] = "output myOutput string = 'hello!'\n",
            });

            var result = await Bicep(services => services.WithFileSystem(fileSystem), "format", "main.bicep", "--insert-final-newline", "false");

            AssertSuccess(result);

            var formatted = fileSystem.File.ReadAllText("main.bicep");
            formatted.Should().BeEquivalentTo("output myOutput string = 'hello!'");
        }


        private static IEnumerable<object[]> GetDataSets() => DataSets.AllDataSets
            .Where(x => !x.Name.Equals(DataSets.PrettyPrint_LF.Name, StringComparison.Ordinal))
            .ToDynamicTestData();

        private static void AssertSuccess(CliResult result)
        {
            using (new AssertionScope())
            {
                result.ExitCode.Should().Be(0);
                AssertNoErrors(result.Stderr);
            }
        }

        private static void AssertFailure(CliResult result, string? errorMessagePattern = null)
        {
            using (new AssertionScope())
            {
                result.ExitCode.Should().Be(1);
                result.Stdout.Should().BeEmpty();
                result.Stderr.Should().NotBeEmpty();

                if (errorMessagePattern is not null)
                {
                    result.Stderr.Should().MatchRegex(errorMessagePattern);
                }
            }
        }

        [GeneratedRegex(@"^(?<indentation>\s+)")]
        private static partial Regex IndentationPattern();
    }
}
