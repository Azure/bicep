// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Bicep.Cli.UnitTests;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.Samples;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Json;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
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
            return new Program(TestResourceTypeProvider.Create(), outputWriter, errorWriter);
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
                "files",
                ".bicep",
                "Arguments:",
                "Options:",
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
            error.Should().Be($"At least one file must be specified to the build command.{Environment.NewLine}");
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
                error.Should().BeEmpty();
            }

            var compiledFilePath = Path.Combine(outputDirectory, DataSet.TestFileMainCompiled);
            File.Exists(compiledFilePath).Should().BeTrue();

            var actual = JToken.Parse(File.ReadAllText(compiledFilePath));

            actual.Should().EqualWithJsonDiffOutput(
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
                error.Should().BeEmpty();
                output.Should().NotBeEmpty();
            }

            var compiledFilePath = Path.Combine(outputDirectory, DataSet.TestFileMainCompiled);
            File.Exists(compiledFilePath).Should().BeTrue();

            var actual = JToken.Parse(output);

            actual.Should().EqualWithJsonDiffOutput(
                JToken.Parse(dataSet.Compiled!),
                expectedLocation: Path.Combine("src", "Bicep.Core.Samples", "Files", dataSet.Name, DataSet.TestFileMainCompiled),
                actualLocation: compiledFilePath);
        }

        [TestMethod]
        public void BuildManyFilesShouldProduceExpectedTemplate()
        {
            var validDataSets = DataSets.AllDataSets.Where(ds => ds.IsValid).ToList();
            var outputDirectories = validDataSets.ToDictionary(ds => ds, ds => ds.SaveFilesToTestDirectory(TestContext, ds.Name));

            var bicepFiles = outputDirectories.Values.Select(dir => Path.Combine(dir, DataSet.TestFileMain));
            var (output, error, result) = TextWriterHelper.InvokeWriterAction((@out, err) =>
            {
                var p = CreateProgram(@out, err);

                string[] args = "build".AsEnumerable().Concat(bicepFiles).ToArray();
                return p.Run(args);
            });

            using (new AssertionScope())
            {
                result.Should().Be(0);
                output.Should().BeEmpty();
                error.Should().BeEmpty();
            }

            foreach (var kvp in outputDirectories)
            {
                var outputDirectory = kvp.Value;
                var dataSet = kvp.Key;

                var compiledFilePath = Path.Combine(outputDirectory, DataSet.TestFileMainCompiled);
                File.Exists(compiledFilePath).Should().BeTrue();

                var actual = JToken.Parse(File.ReadAllText(compiledFilePath));

                actual.Should().EqualWithJsonDiffOutput(
                    JToken.Parse(dataSet.Compiled!),
                    expectedLocation: Path.Combine("src", "Bicep.Core.Samples", "Files", dataSet.Name, DataSet.TestFileMainCompiled),
                    actualLocation: compiledFilePath);
            }
        }

        [TestMethod]
        public void BuildManyFilesToStdOutShouldProduceExpectedTemplate()
        {
            var validDataSets = DataSets.AllDataSets.Where(ds => ds.IsValid).ToList();
            var outputDirectories = validDataSets.ToDictionary(ds => ds, ds => ds.SaveFilesToTestDirectory(TestContext, ds.Name));

            var bicepFiles = outputDirectories.Values.Select(dir => Path.Combine(dir, DataSet.TestFileMain));
            var (output, error, result) = TextWriterHelper.InvokeWriterAction((@out, err) =>
            {
                var p = CreateProgram(@out, err);

                string[] args = new[] {"build", "--stdout"}.Concat(bicepFiles).ToArray();
                return p.Run(args);
            });

            using (new AssertionScope())
            {
                result.Should().Be(0);
                error.Should().BeEmpty();
                output.Should().NotBeEmpty();
            }

            var actual = JArray.Parse(output);
            var expected = new JArray(validDataSets.Select(ds => JToken.Parse(ds.Compiled!)));

            FileHelper.SaveResultFile(this.TestContext, "Combined_Compiled_Actual.json", output);
            FileHelper.SaveResultFile(this.TestContext, "Combined_Compiled_Expected.json", expected.ToString(Formatting.Indented));

            JsonAssert.AreEqual(expected, actual, this.TestContext, "Combined_Compiled_Delta.json");
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
        public void BuildManyFilesShouldProduceExpectedErrors()
        {
            var invalidDataSets = DataSets.AllDataSets.Where(ds => !ds.IsValid).ToList();
            var outputDirectories = invalidDataSets.ToDictionary(ds => ds, ds => ds.SaveFilesToTestDirectory(TestContext, ds.Name));

            var bicepFiles = outputDirectories.Values.Select(dir => Path.Combine(dir, DataSet.TestFileMain));
            var (output, error, result) = TextWriterHelper.InvokeWriterAction((@out, err) =>
            {
                var p = CreateProgram(@out, err);
                return p.Run("build".AsEnumerable().Concat(bicepFiles).ToArray());
            });

            result.Should().Be(1);
            output.Should().BeEmpty();
            error.Should().NotBeEmpty();

            var diagnosticsFromAllFiles = bicepFiles.SelectMany(file => GetAllDiagnostics(file));

            error.Should().ContainAll(diagnosticsFromAllFiles);
        }

        [TestMethod]
        public void BuildManyFilesToStdOutShouldProduceExpectedErrors()
        {
            var invalidDataSets = DataSets.AllDataSets.Where(ds => !ds.IsValid).ToList();
            var outputDirectories = invalidDataSets.ToDictionary(ds => ds, ds => ds.SaveFilesToTestDirectory(TestContext, ds.Name));

            var bicepFiles = outputDirectories.Values.Select(dir => Path.Combine(dir, DataSet.TestFileMain));
            var (output, error, result) = TextWriterHelper.InvokeWriterAction((@out, err) =>
            {
                var p = CreateProgram(@out, err);
                return p.Run(new[] { "build", "--stdout" }.Concat(bicepFiles).ToArray());
            });

            result.Should().Be(1);
            output.Should().Be("[]");
            error.Should().NotBeEmpty();

            var diagnosticsFromAllFiles = bicepFiles.SelectMany(file => GetAllDiagnostics(file));

            error.Should().ContainAll(diagnosticsFromAllFiles);
        }

        [DataRow("DoesNotExist.bicep", @"An error occurred loading the module. Could not find file '.+DoesNotExist.bicep'")]
        [DataRow("WrongDir\\Fake.bicep", @"An error occurred loading the module. Could not find .+'.+WrongDir[\\/]Fake.bicep'")]
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

        [DataRow("DoesNotExist.bicep", @"An error occurred loading the module. Could not find file '.+DoesNotExist.bicep'")]
        [DataRow("WrongDir\\Fake.bicep", @"An error occurred loading the module. Could not find .+'.+WrongDir[\\/]Fake.bicep'")]
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

        private IEnumerable<string> GetAllDiagnostics(string bicepFilePath)
        {
            var syntaxTreeGrouping = SyntaxTreeGroupingBuilder.Build(new FileResolver(), bicepFilePath);
            var compilation = new Compilation(TestResourceTypeProvider.Create(), syntaxTreeGrouping);

            var output = new List<string>();
            foreach (var (syntaxTree, diagnostics) in compilation.GetAllDiagnosticsBySyntaxTree())
            {
                foreach (var diagnostic in diagnostics)
                {
                    var (line, character) = TextCoordinateConverter.GetPosition(syntaxTree.LineStarts, diagnostic.Span.Position);
                    output.Add($"{syntaxTree.FilePath}({line + 1},{character + 1}) : {diagnostic.Level} {diagnostic.Code}: {diagnostic.Message}");
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
    }
}

