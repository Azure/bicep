// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bicep.Cli.UnitTests;
using Bicep.Core.Extensions;
using Bicep.Core.Samples;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.Core.UnitTests.Json;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Cli.IntegrationTests
{
    [TestClass]
    public class ProgramTests
    {
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public void BicepVersionShouldPrintVersionInformation()
        {
            var (output, error) = TextWriterHelper.InvokeWriterAction((@out, err) =>
            {
                var p = new Program(@out, err);
                p.Run(new[] {"--version"}).Should().Be(0);
            });

            error.Should().BeEmpty();

            output.Should().NotBeEmpty();
            output.Should().StartWith("Bicep CLI version");
        }

        [TestMethod]
        public void BicepHelpShouldPrintHelp()
        {
            var (output, error) = TextWriterHelper.InvokeWriterAction((@out, err) =>
            {
                var p = new Program(@out, err);
                p.Run(new[] { "--help" }).Should().Be(0);
            });

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
            var (output, error) = TextWriterHelper.InvokeWriterAction((@out, err) =>
            {
                var p = new Program(@out, err);
                p.Run(new[] {"wrong", "fake", "broken"}).Should().Be(1);
            });

            output.Should().BeEmpty();

            error.Should().NotBeEmpty();
            error.Should().Be($"Unrecognized arguments 'wrong fake broken' specified. Use 'bicep --help' to view available options.{Environment.NewLine}");
        }

        [TestMethod]
        public void ZeroFilesToBuildShouldProduceError()
        {
            var (output, error) = TextWriterHelper.InvokeWriterAction((@out, err) =>
            {
                var p = new Program(@out, err);
                p.Run(new[] { "build" }).Should().Be(1);
            });

            output.Should().BeEmpty();

            error.Should().NotBeEmpty();
            error.Should().Be($"At least one file must be specified to the build command.{Environment.NewLine}");
        }

        [DataTestMethod]
        [DynamicData(nameof(GetValidDataSets), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public void BuildSingleFileShouldProduceExpectedTemplate(DataSet dataSet)
        {
            string bicepFilePath = FileHelper.SaveResultFile(this.TestContext!, $"{dataSet.Name}_Bicep.bicep", dataSet.Bicep);

            var (output, error) = TextWriterHelper.InvokeWriterAction((@out, err) =>
            {
                var p = new Program(@out, err);
                p.Run(new[] { "build", bicepFilePath }).Should().Be(0);
            });

            output.Should().BeEmpty();
            error.Should().BeEmpty();

            string compiledFilePath = FileHelper.GetResultFilePath(this.TestContext!, $"{dataSet.Name}_Bicep.json");
            File.Exists(compiledFilePath).Should().BeTrue();

            FileHelper.SaveResultFile(this.TestContext!, $"{dataSet.Name}_Compiled_Actual.json", File.ReadAllText(compiledFilePath));
            FileHelper.SaveResultFile(this.TestContext!, $"{dataSet.Name}_Compiled_Expected.json", dataSet.Compiled!);

            var actual = JToken.Parse(File.ReadAllText(compiledFilePath));
            var expected = JToken.Parse(dataSet.Compiled!);

            JsonAssert.AreEqual(expected, actual, this.TestContext!, $"{dataSet.Name}_Compiled_Delta.json");
        }

        [DataTestMethod]
        [DynamicData(nameof(GetValidDataSets), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public void BuildSingleFileToStdOutShouldProduceExpectedTemplate(DataSet dataSet)
        {
            string bicepFilePath = FileHelper.SaveResultFile(this.TestContext!, $"{dataSet.Name}_Bicep.bicep", dataSet.Bicep);

            var (output, error) = TextWriterHelper.InvokeWriterAction((@out, err) =>
            {
                var p = new Program(@out, err);
                p.Run(new[] {"build", "--stdout", bicepFilePath}).Should().Be(0);
            });

            error.Should().BeEmpty();
            
            output.Should().NotBeEmpty();

            FileHelper.SaveResultFile(this.TestContext!, $"{dataSet.Name}_Compiled_Actual.json", output);
            FileHelper.SaveResultFile(this.TestContext!, $"{dataSet.Name}_Compiled_Expected.json", dataSet.Compiled!);

            var actual = JToken.Parse(output);
            var expected = JToken.Parse(dataSet.Compiled!);

            JsonAssert.AreEqual(expected,actual,this.TestContext!, $"{dataSet.Name}_Compiled_Delta.json");
        }

        [TestMethod]
        public void BuildManyFilesShouldProduceExpectedTemplate()
        {
            var validDataSets = DataSets.AllDataSets.Where(ds => ds.IsValid).ToList();

            var bicepFiles = validDataSets
                .Select(ds => FileHelper.SaveResultFile(this.TestContext!, $"{ds.Name}_Bicep.bicep", ds.Bicep))
                .ToList();

            var (output, error) = TextWriterHelper.InvokeWriterAction((@out, err) =>
            {
                var p = new Program(@out, err);

                string[] args = "build".AsEnumerable().Concat(bicepFiles).ToArray();
                p.Run(args).Should().Be(0);
            });

            output.Should().BeEmpty();
            error.Should().BeEmpty();

            var compiledFiles = bicepFiles.Select(file => Path.ChangeExtension(file, ".json")).ToList();

            validDataSets.Should().HaveCount(compiledFiles.Count);
            foreach (var (dataSet, compiledFilePath) in validDataSets.Zip(compiledFiles))
            {
                FileHelper.SaveResultFile(this.TestContext!, $"{dataSet.Name}_Compiled_Actual.json", File.ReadAllText(compiledFilePath));
                FileHelper.SaveResultFile(this.TestContext!, $"{dataSet.Name}_Compiled_Expected.json", dataSet.Compiled!);

                var actual = JToken.Parse(File.ReadAllText(compiledFilePath));
                var expected = JToken.Parse(dataSet.Compiled!);

                JsonAssert.AreEqual(expected, actual, this.TestContext!, $"{dataSet.Name}_Compiled_Delta.json");
            }
        }

        [TestMethod]
        public void BuildManyFilesToStdOutShouldProduceExpectedTemplate()
        {
            var validDataSets = DataSets.AllDataSets.Where(ds => ds.IsValid).ToList();

            var bicepFiles = validDataSets
                .Select(ds => FileHelper.SaveResultFile(this.TestContext!, $"{ds.Name}_Bicep.bicep", ds.Bicep))
                .ToList();

            var (output, error) = TextWriterHelper.InvokeWriterAction((@out, err) =>
            {
                var p = new Program(@out, err);

                string[] args = new[] {"build", "--stdout"}.Concat(bicepFiles).ToArray();
                p.Run(args).Should().Be(0);
            });

            error.Should().BeEmpty();
            output.Should().NotBeEmpty();

            var actual = JArray.Parse(output);
            var expected = new JArray(validDataSets.Select(ds => JToken.Parse(ds.Compiled!)));

            FileHelper.SaveResultFile(this.TestContext!, "Combined_Compiled_Actual.json", output);
            FileHelper.SaveResultFile(this.TestContext!, "Combined_Compiled_Expected.json", expected.ToString(Formatting.Indented));

            JsonAssert.AreEqual(expected, actual, this.TestContext!, "Combined_Compiled_Delta.json");
        }

        [DataTestMethod]
        [DynamicData(nameof(GetInvalidDataSets), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public void BuildSingleFileShouldProduceExpectedErrors(DataSet dataSet)
        {
            string bicepFilePath = FileHelper.SaveResultFile(this.TestContext!, $"{dataSet.Name}_Bicep.bicep", dataSet.Bicep);

            var (output, error) = TextWriterHelper.InvokeWriterAction((@out, err) =>
            {
                var p = new Program(@out, err);
                p.Run(new[] { "build", bicepFilePath }).Should().Be(1);
            });

            output.Should().BeEmpty();
            error.Should().NotBeEmpty();

            var diagnostics = GetAllDiagnostics(dataSet.Bicep, bicepFilePath);
            error.Should().ContainAll(diagnostics);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetInvalidDataSets), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public void BuildSingleFileToStdOutShouldProduceExpectedErrors(DataSet dataSet)
        {
            string bicepFilePath = FileHelper.SaveResultFile(this.TestContext!, $"{dataSet.Name}_Bicep.bicep", dataSet.Bicep);

            var (output, error) = TextWriterHelper.InvokeWriterAction((@out, err) =>
            {
                var p = new Program(@out, err);
                p.Run(new[] {"build", "--stdout", bicepFilePath}).Should().Be(1);
            });

            output.Should().BeEmpty();
            error.Should().NotBeEmpty();

            var diagnostics = GetAllDiagnostics(dataSet.Bicep, bicepFilePath);
            error.Should().ContainAll(diagnostics);
        }

        [TestMethod]
        public void BuildManyFilesShouldProduceExpectedErrors()
        {
            var invalidDataSets = DataSets.AllDataSets.Where(ds => ds.IsValid == false).ToList();

            var bicepFiles = invalidDataSets
                .Select(ds => FileHelper.SaveResultFile(this.TestContext!, $"{ds.Name}_Bicep.bicep", ds.Bicep))
                .ToList();

            invalidDataSets.Should().HaveCount(bicepFiles.Count);

            var (output, error) = TextWriterHelper.InvokeWriterAction((@out, err) =>
            {
                var p = new Program(@out, err);
                p.Run("build".AsEnumerable().Concat(bicepFiles).ToArray()).Should().Be(1);
            });

            output.Should().BeEmpty();
            error.Should().NotBeEmpty();

            var diagnosticsFromAllFiles = invalidDataSets
                .Zip(bicepFiles, (ds, file) => GetAllDiagnostics(ds.Bicep, file))
                .SelectMany(e => e);

            error.Should().ContainAll(diagnosticsFromAllFiles);
        }

        [TestMethod]
        public void BuildManyFilesToStdOutShouldProduceExpectedErrors()
        {
            var invalidDataSets = DataSets.AllDataSets.Where(ds => ds.IsValid == false).ToList();

            var bicepFiles = invalidDataSets
                .Select(ds => FileHelper.SaveResultFile(this.TestContext!, $"{ds.Name}_Bicep.bicep", ds.Bicep))
                .ToList();

            invalidDataSets.Should().HaveCount(bicepFiles.Count);

            var (output, error) = TextWriterHelper.InvokeWriterAction((@out, err) =>
            {
                var p = new Program(@out, err);
                p.Run(new[] { "build", "--stdout" }.Concat(bicepFiles).ToArray()).Should().Be(1);
            });

            output.Should().Be("[]");
            error.Should().NotBeEmpty();

            var diagnosticsFromAllFiles = invalidDataSets
                .Zip(bicepFiles, (ds, file) => GetAllDiagnostics(ds.Bicep, file))
                .SelectMany(e => e);

            error.Should().ContainAll(diagnosticsFromAllFiles);
        }

        private IEnumerable<string> GetAllDiagnostics(string text, string bicepFilePath)
        {
            var compilation = new Compilation(SyntaxFactory.CreateFromText(text));
            var lineStarts = TextCoordinateConverter.GetLineStarts(text);

            return compilation.GetSemanticModel()
                .GetAllDiagnostics()
                .Select(d =>
                {
                    var (line, character) = TextCoordinateConverter.GetPosition(lineStarts, d.Span.Position);
                    return $"{bicepFilePath}({line + 1},{character + 1}) : {d.Level} {d.Code}: {d.Message}";
                });
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

