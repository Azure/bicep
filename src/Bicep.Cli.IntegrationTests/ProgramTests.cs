// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Bicep.Cli.UnitTests;
using Bicep.Core.FileSystem;
using Bicep.Core.Samples;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Workspaces;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Bicep.Cli.IntegrationTests
{
    [TestClass]
    public class ProgramTests
    { 
        [NotNull]
        public TestContext? TestContext { get; set; }

        private static Program CreateProgram(TextWriter outputWriter, TextWriter errorWriter)
        {
            return new Program(TestResourceTypeProvider.Create(), outputWriter, errorWriter, BicepTestConstants.DevAssemblyFileVersion);
        }

        [TestMethod]
        public void BicepVersionShouldPrintVersionInformation()
        {
            var (output, error, result) = TextWriterHelper.InvokeWriterAction((@out, err) =>
            {
                var p = CreateProgram(@out, err);
                return p.Run(new[] {"--version"});
            });

            result.Should().Be(0);
            error.Should().BeEmpty();

            output.Should().NotBeEmpty();
            output.Should().StartWith("Bicep CLI version");
        }

        [TestMethod]
        public void BicepHelpShouldPrintHelp()
        {
            var (output, error, result) = TextWriterHelper.InvokeWriterAction((@out, err) =>
            {
                var p = CreateProgram(@out, err);
                return p.Run(new[] { "--help" });
            });

            result.Should().Be(0);
            error.Should().BeEmpty();
            
            output.Should().NotBeEmpty();
            output.Should().ContainAll(
                "build",
                "[options]",
                "<file>",
                ".bicep",
                "Arguments:",
                "Options:",
                "--outdir",
                "--outfile",
                "--stdout",
                "--version",
                "--help",
                "information",
                "version",
                "bicep",
                "usage");
        }

        [TestMethod]
        public void WrongArgumentsShouldProduceError()
        {
            var (output, error, result) = TextWriterHelper.InvokeWriterAction((@out, err) =>
            {
                var p = CreateProgram(@out, err);
                return p.Run(new[] {"wrong", "fake", "broken"});
            });

            result.Should().Be(1);
            output.Should().BeEmpty();

            error.Should().NotBeEmpty();
            error.Should().Be($"Unrecognized arguments 'wrong fake broken' specified. Use 'bicep --help' to view available options.{Environment.NewLine}");
        }

        [TestMethod]
        public void ZeroFilesToBuildShouldProduceError()
        {
            var (output, error, result) = TextWriterHelper.InvokeWriterAction((@out, err) =>
            {
                var p = CreateProgram(@out, err);
                return p.Run(new[] { "build" });
            });

            result.Should().Be(1);
            output.Should().BeEmpty();

            error.Should().NotBeEmpty();
            error.Should().Be($"The input file path was not specified{Environment.NewLine}");
        }

        [DataTestMethod]
        [DynamicData(nameof(GetValidDataSets), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public void BuildSingleFileShouldProduceExpectedTemplate(DataSet dataSet)
        {
            var outputDirectory = dataSet.SaveFilesToTestDirectory(TestContext, dataSet.Name);
            var bicepFilePath = Path.Combine(outputDirectory, DataSet.TestFileMain);

            var (output, error, result) = TextWriterHelper.InvokeWriterAction((@out, err) =>
            {
                var p = CreateProgram(@out, err);
                return p.Run(new[] { "build", bicepFilePath });
            });

            using (new AssertionScope())
            {
                result.Should().Be(0);
                output.Should().BeEmpty();
                AssertEmptyOrDeprecatedError(error, dataSet.Name);
            }

            var compiledFilePath = Path.Combine(outputDirectory, DataSet.TestFileMainCompiled);
            File.Exists(compiledFilePath).Should().BeTrue();

            var actual = JToken.Parse(File.ReadAllText(compiledFilePath));

            actual.Should().EqualWithJsonDiffOutput(
                TestContext, 
                JToken.Parse(dataSet.Compiled!),
                expectedLocation: Path.Combine("src", "Bicep.Core.Samples", "Files", dataSet.Name, DataSet.TestFileMainCompiled),
                actualLocation: compiledFilePath);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetValidDataSets), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public void BuildSingleFileToStdOutShouldProduceExpectedTemplate(DataSet dataSet)
        {
            var outputDirectory = dataSet.SaveFilesToTestDirectory(TestContext, dataSet.Name);
            var bicepFilePath = Path.Combine(outputDirectory, DataSet.TestFileMain);

            var (output, error, result) = TextWriterHelper.InvokeWriterAction((@out, err) =>
            {
                var p = CreateProgram(@out, err);
                return p.Run(new[] {"build", "--stdout", bicepFilePath});
            });

            using (new AssertionScope())
            {
                result.Should().Be(0);
                output.Should().NotBeEmpty();
                AssertEmptyOrDeprecatedError(error, dataSet.Name);
            }

            var compiledFilePath = Path.Combine(outputDirectory, DataSet.TestFileMainCompiled);
            File.Exists(compiledFilePath).Should().BeTrue();

            var actual = JToken.Parse(output);

            actual.Should().EqualWithJsonDiffOutput(
                TestContext, 
                JToken.Parse(dataSet.Compiled!),
                expectedLocation: Path.Combine("src", "Bicep.Core.Samples", "Files", dataSet.Name, DataSet.TestFileMainCompiled),
                actualLocation: compiledFilePath);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetInvalidDataSets), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public void BuildSingleFileShouldProduceExpectedErrors(DataSet dataSet)
        {
            var outputDirectory = dataSet.SaveFilesToTestDirectory(TestContext, dataSet.Name);
            var bicepFilePath = Path.Combine(outputDirectory, DataSet.TestFileMain);

            var (output, error, result) = TextWriterHelper.InvokeWriterAction((@out, err) =>
            {
                var p = CreateProgram(@out, err);
                return p.Run(new[] { "build", bicepFilePath });
            });

            result.Should().Be(1);
            output.Should().BeEmpty();
            error.Should().NotBeEmpty();

            var diagnostics = GetAllDiagnostics(bicepFilePath);
            error.Should().ContainAll(diagnostics);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetInvalidDataSets), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public void BuildSingleFileToStdOutShouldProduceExpectedErrors(DataSet dataSet)
        {
            var outputDirectory = dataSet.SaveFilesToTestDirectory(TestContext, dataSet.Name);
            var bicepFilePath = Path.Combine(outputDirectory, DataSet.TestFileMain);

            var (output, error, result) = TextWriterHelper.InvokeWriterAction((@out, err) =>
            {
                var p = CreateProgram(@out, err);
                return p.Run(new[] {"build", "--stdout", bicepFilePath});
            });

            result.Should().Be(1);
            output.Should().BeEmpty();
            error.Should().NotBeEmpty();

            var diagnostics = GetAllDiagnostics(bicepFilePath);
            error.Should().ContainAll(diagnostics);
        }

        [TestMethod]
        public void Build_command_with_outfile_parameter()
        {
            var bicepPath = FileHelper.SaveResultFile(TestContext, "input.bicep", @"
output myOutput string = 'hello!'
            ");

            var outputFilePath = FileHelper.GetResultFilePath(TestContext, "output.json");

            File.Exists(outputFilePath).Should().BeFalse();
            var (output, error, result) = TextWriterHelper.InvokeWriterAction((@out, err) =>
            {
                var p = CreateProgram(@out, err);
                return p.Run(new[] {"build", "--outfile", outputFilePath, bicepPath});
            });

            File.Exists(outputFilePath).Should().BeTrue();
            result.Should().Be(0);
        }

        [TestMethod]
        public void Build_command_with_nonexistent_outdir_parameter()
        {
            var bicepPath = FileHelper.SaveResultFile(TestContext, "input.bicep", @"
output myOutput string = 'hello!'
            ");

            var outputFileDir = FileHelper.GetResultFilePath(TestContext, "outputdir");
            var (output, error, result) = TextWriterHelper.InvokeWriterAction((@out, err) =>
            {
                var p = CreateProgram(@out, err);
                return p.Run(new[] {"build", "--outdir", outputFileDir, bicepPath});
            });

            result.Should().Be(1);
            output.Should().BeEmpty();
            error.Should().MatchRegex(@"The specified output directory "".*outputdir"" does not exist");
        }

        [TestMethod]
        public void Build_command_with_outdir_parameter()
        {
            var bicepPath = FileHelper.SaveResultFile(TestContext, "input.bicep", @"
output myOutput string = 'hello!'
            ");

            var outputFileDir = FileHelper.GetResultFilePath(TestContext, "outputdir");
            Directory.CreateDirectory(outputFileDir);
            var expectedOutputFile = Path.Combine(outputFileDir, "input.json");

            File.Exists(expectedOutputFile).Should().BeFalse();
            var (output, error, result) = TextWriterHelper.InvokeWriterAction((@out, err) =>
            {
                var p = CreateProgram(@out, err);
                return p.Run(new[] {"build", "--outdir", outputFileDir, bicepPath});
            });

            File.Exists(expectedOutputFile).Should().BeTrue();
            result.Should().Be(0);
        }

        [DataRow("DoesNotExist.bicep", @"An error occurred reading file. Could not find file '.+DoesNotExist.bicep'")]
        [DataRow("WrongDir\\Fake.bicep", @"An error occurred reading file. Could not find .+'.+WrongDir[\\/]Fake.bicep'")]
        [DataTestMethod]
        public void BuildInvalidInputPathsShouldProduceExpectedError(string badPath, string expectedErrorRegex)
        {
            var (output, error, result) = TextWriterHelper.InvokeWriterAction((@out, err) =>
            {
                var p = CreateProgram(@out, err);
                return p.Run(new[] {"build", badPath});
            });

            result.Should().Be(1);
            output.Should().BeEmpty();
            error.Should().MatchRegex(expectedErrorRegex);
        }

        [DataRow("DoesNotExist.bicep", @"An error occurred reading file. Could not find file '.+DoesNotExist.bicep'")]
        [DataRow("WrongDir\\Fake.bicep", @"An error occurred reading file. Could not find .+'.+WrongDir[\\/]Fake.bicep'")]
        [DataTestMethod]
        public void BuildInvalidInputPathsToStdOutShouldProduceExpectedError(string badPath, string expectedErrorRegex)
        {
            var (output, error, result) = TextWriterHelper.InvokeWriterAction((@out, err) =>
            {
                var p = CreateProgram(@out, err);
                return p.Run(new[] { "build", "--stdout", badPath });
            });

            result.Should().Be(1);
            output.Should().BeEmpty();
            error.Should().MatchRegex(expectedErrorRegex);
        }

        [TestMethod]
        public void LockedOutputFileShouldProduceExpectedError()
        {
            var inputFile = FileHelper.SaveResultFile(this.TestContext, "Empty.bicep", DataSets.Empty.Bicep);
            var outputFile = PathHelper.GetDefaultOutputPath(inputFile);

            // ReSharper disable once ConvertToUsingDeclaration
            using (new FileStream(outputFile, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
            {
                // keep the output stream open while we attempt to write to it
                // this should force an access denied error
                var (output, error, result) = TextWriterHelper.InvokeWriterAction((@out, err) =>
                {
                    var p = CreateProgram(@out, err);
                    return p.Run(new[] { "build", inputFile });
                });

                result.Should().Be(1);
                output.Should().BeEmpty();
                error.Should().Contain("Empty.json");
            }
        }

        private static IEnumerable<string> GetAllDiagnostics(string bicepFilePath)
        {
            var syntaxTreeGrouping = SyntaxTreeGroupingBuilder.Build(new FileResolver(), new Workspace(), PathHelper.FilePathToFileUrl(bicepFilePath));
            var compilation = new Compilation(TestResourceTypeProvider.Create(), syntaxTreeGrouping);

            var output = new List<string>();
            foreach (var (syntaxTree, diagnostics) in compilation.GetAllDiagnosticsBySyntaxTree())
            {
                foreach (var diagnostic in diagnostics)
                {
                    var (line, character) = TextCoordinateConverter.GetPosition(syntaxTree.LineStarts, diagnostic.Span.Position);
                    output.Add($"{syntaxTree.FileUri.LocalPath}({line + 1},{character + 1}) : {diagnostic.Level} {diagnostic.Code}: {diagnostic.Message}");
                }
            }

            return output;
        }

        private static IEnumerable<object[]> GetValidDataSets() => DataSets
            .AllDataSets
            .Where(ds => ds.IsValid)
            .ToDynamicTestData();

        private static IEnumerable<object[]> GetInvalidDataSets() => DataSets
            .AllDataSets
            .Where(ds => ds.IsValid == false)
            .ToDynamicTestData();

        private static void AssertEmptyOrDeprecatedError(string error, string dataSetName)
        {
            if (dataSetName == "Parameters_LF" || dataSetName == "Parameters_CRLF")
            {
                // TODO: remove this branch when the support of parameter modifiers is dropped. 
                foreach(var line in error.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    line.Should().Contain("BCP156");
                }
            }
            else
            {
                error.Should().BeEmpty();
            }
        }
    }
}

