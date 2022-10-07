// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core;
using Bicep.Core.Samples;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bicep.Cli.IntegrationTests
{
    [TestClass]
    public class FormatCommandTests : TestBase
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public async Task Format_ZeroFiles_ShouldFail_WithExpectedErrorMessage()
        {
            var (output, error, result) = await Bicep("format");

            using (new AssertionScope())
            {
                result.Should().Be(1);
                output.Should().BeEmpty();

                error.Should().NotBeEmpty();
                error.Should().Contain($"The input file path was not specified");
            }
        }

        [TestMethod]
        public async Task Format_NonBicepFiles_ShouldFail_WithExpectedErrorMessage()
        {
            var (output, error, result) = await Bicep("format", "/dev/zero");

            using (new AssertionScope())
            {
                result.Should().Be(1);
                output.Should().BeEmpty();

                error.Should().NotBeEmpty();
                error.Should().Contain($@"The specified input ""/dev/zero"" was not recognized as a bicep file. Bicep files must use the {LanguageConstants.LanguageFileExtension} extension.");
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(GetValidDataSets), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task Format_Valid_Files_with_DefaultSettings_ShouldSucceed(DataSet dataSet)
        {
            var outputDirectory = dataSet.SaveFilesToTestDirectory(TestContext);
            var bicepFilePath = Path.Combine(outputDirectory, DataSet.TestFileMain);
            var originalContent = File.ReadAllText(bicepFilePath);
            if (string.IsNullOrEmpty(originalContent))
            {
                // Skip if input file is empty
                return;
            }
            var (output, error, result) = await Bicep("format", bicepFilePath);

            // Should format successfully
            using (new AssertionScope())
            {
                result.Should().Be(0);
                output.Should().BeEmpty();
                AssertNoErrors(error);
            }

            var actual = File.ReadAllText(bicepFilePath);

            string lf = ((char)10).ToString();
            string cr = ((char)13).ToString();

            // Should not have new lines in end of file
            actual.Should().NotEndWith(lf);
            actual.Should().NotEndWith(cr);

            // Should use two spaces for indentation
            var indentation = GetIndentation(actual);
            if (indentation.Length > 0)
            {
                indentation.Should().Be("  ");
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(GetValidDataSets), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task Format_Valid_Files_with_DefaultSettings_and_OutFile_ShouldSucceed(DataSet dataSet)
        {
            var outputDirectory = dataSet.SaveFilesToTestDirectory(TestContext);
            var bicepFilePath = Path.Combine(outputDirectory, DataSet.TestFileMain);
            var originalContent = File.ReadAllText(bicepFilePath);
            if (string.IsNullOrEmpty(originalContent))
            {
                // Skip if input file is empty
                return;
            }
            var outFilePath = Path.Combine(outputDirectory, DataSet.TestFileMainFormatted);
            var (output, error, result) = await Bicep("format", bicepFilePath, "--outfile", outFilePath);

            // Should format successfully
            using (new AssertionScope())
            {
                result.Should().Be(0);
                output.Should().BeEmpty();
                AssertNoErrors(error);
            }

            var actual = File.ReadAllText(outFilePath);

            string lf = ((char)10).ToString();
            string cr = ((char)13).ToString();

            // Should not have new lines in end of file
            actual.Should().NotEndWith(lf);
            actual.Should().NotEndWith(cr);

            // Should use two spaces for indentation
            var indentation = GetIndentation(actual);
            if (indentation.Length > 0)
            {
                indentation.Should().Be("  ");
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(GetValidDataSets), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task Format_Valid_Files_with_DefaultSettings_to_StandardOut_ShouldSucceed(DataSet dataSet)
        {
            var outputDirectory = dataSet.SaveFilesToTestDirectory(TestContext);
            var bicepFilePath = Path.Combine(outputDirectory, DataSet.TestFileMain);
            var originalContent = File.ReadAllText(bicepFilePath);
            if (string.IsNullOrEmpty(originalContent))
            {
                // Skip if input file is empty
                return;
            }
            var (output, error, result) = await Bicep("format", bicepFilePath, "--stdout");

            // Should format successfully to stdout
            using (new AssertionScope())
            {
                result.Should().Be(0);
                output.Should().NotBeEmpty();
                AssertNoErrors(error);
            }

            var actual = output;

            string lf = ((char)10).ToString();
            string cr = ((char)13).ToString();

            // Should not have new lines in end of file
            actual.Should().NotEndWith(lf);
            actual.Should().NotEndWith(cr);

            // Should use two spaces for indentation
            var indentation = GetIndentation(actual);
            if (indentation.Length > 0)
            {
                indentation.Should().Be("  ");
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(GetValidDataSets), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task Format_Valid_Files_with_Newline_CRLF_ShouldSucceed(DataSet dataSet)
        {
            var outputDirectory = dataSet.SaveFilesToTestDirectory(TestContext);
            var bicepFilePath = Path.Combine(outputDirectory, DataSet.TestFileMain);
            var originalContent = File.ReadAllText(bicepFilePath);
            if (string.IsNullOrEmpty(originalContent))
            {
                // Skip if input file is empty
                return;
            }
            var (output, error, result) = await Bicep("format", bicepFilePath, "--newLine", "CRLF");

            // Should format successfully
            using (new AssertionScope())
            {
                result.Should().Be(0);
                output.Should().BeEmpty();
                AssertNoErrors(error);
            }

            var actual = File.ReadAllText(bicepFilePath);

            string lf = ((char)10).ToString();
            string cr = ((char)13).ToString();

            // Should not have new lines in end of file
            actual.Should().NotEndWith(lf);
            actual.Should().NotEndWith(cr);

            // Should use CRLF lineEndings
            actual.Should().Contain(cr + lf);

            // Should use two spaces for indentation
            var indentation = GetIndentation(actual);
            if (indentation.Length > 0)
            {
                indentation.Should().Be("  ");
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(GetValidDataSets), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task Format_Valid_Files_with_Newline_LF_ShouldSucceed(DataSet dataSet)
        {
            var outputDirectory = dataSet.SaveFilesToTestDirectory(TestContext);
            var bicepFilePath = Path.Combine(outputDirectory, DataSet.TestFileMain);
            var originalContent = File.ReadAllText(bicepFilePath);
            if (string.IsNullOrEmpty(originalContent))
            {
                // Skip if input file is empty
                return;
            }
            var (output, error, result) = await Bicep("format", bicepFilePath, "--newLine", "LF");

            // Should format successfully
            using (new AssertionScope())
            {
                result.Should().Be(0);
                output.Should().BeEmpty();
                AssertNoErrors(error);
            }

            var actual = File.ReadAllText(bicepFilePath);

            string lf = ((char)10).ToString();
            string cr = ((char)13).ToString();

            // Should not have new lines in end of file
            actual.Should().NotEndWith(lf);
            actual.Should().NotEndWith(cr);

            // Should use only LF lineEndings
            actual.Should().Contain(lf);
            actual.Should().NotContain(cr);

            // Should use two spaces for indentation
            var indentation = GetIndentation(actual);
            if (indentation.Length > 0)
            {
                indentation.Should().Be("  ");
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(GetValidDataSets), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task Format_Valid_Files_with_Newline_CRLF_and_finalNewLine_ShouldSucceed(DataSet dataSet)
        {
            var outputDirectory = dataSet.SaveFilesToTestDirectory(TestContext);
            var bicepFilePath = Path.Combine(outputDirectory, DataSet.TestFileMain);
            var originalContent = File.ReadAllText(bicepFilePath);
            if (string.IsNullOrEmpty(originalContent))
            {
                // Skip if input file is empty
                return;
            }
            var (output, error, result) = await Bicep("format", bicepFilePath, "--newLine", "CRLF", "--insertFinalNewline");

            // Should format successfully
            using (new AssertionScope())
            {
                result.Should().Be(0);
                output.Should().BeEmpty();
                AssertNoErrors(error);
            }

            var actual = File.ReadAllText(bicepFilePath);

            string lf = ((char)10).ToString();
            string cr = ((char)13).ToString();

            // Should have a new line at the end
            actual.Should().EndWith(cr + lf);

            // Should use CRLF lineEndings
            actual.TrimEnd().Should().Contain(cr + lf);

            // Should use two spaces for indentation
            var indentation = GetIndentation(actual);
            if (indentation.Length > 0)
            {
                indentation.Should().Be("  ");
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(GetValidDataSets), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task Format_Valid_Files_with_Newline_CRLF_and_TabIndentation_ShouldSucceed(DataSet dataSet)
        {
            var outputDirectory = dataSet.SaveFilesToTestDirectory(TestContext);
            var bicepFilePath = Path.Combine(outputDirectory, DataSet.TestFileMain);
            var originalContent = File.ReadAllText(bicepFilePath);
            if (string.IsNullOrEmpty(originalContent))
            {
                // Skip if input file is empty
                return;
            }
            var (output, error, result) = await Bicep("format", bicepFilePath, "--newLine", "CRLF", "--indentKind", "Tab");

            // Should format successfully
            using (new AssertionScope())
            {
                result.Should().Be(0);
                output.Should().BeEmpty();
                AssertNoErrors(error);
            }

            var actual = File.ReadAllText(bicepFilePath);

            string lf = ((char)10).ToString();
            string cr = ((char)13).ToString();

            // Should not have new lines in end of file
            actual.Should().NotEndWith(lf);
            actual.Should().NotEndWith(cr);

            // Should use CRLF lineEndings
            actual.Should().Contain(cr+lf);

            // Should use tabs for indentation
            string tab = ((char)9).ToString();
            var indentation = GetIndentation(actual);
            if (indentation.Length > 0)
            {
                indentation.Should().Be(tab);
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(GetValidDataSets), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task Format_Valid_Files_with_Newline_CRLF_and_Indentsize_4_ShouldSucceed(DataSet dataSet)
        {
            var outputDirectory = dataSet.SaveFilesToTestDirectory(TestContext);
            var bicepFilePath = Path.Combine(outputDirectory, DataSet.TestFileMain);
            var originalContent = File.ReadAllText(bicepFilePath);
            if (string.IsNullOrEmpty(originalContent))
            {
                // Skip if input file is empty
                return;
            }
            var (output, error, result) = await Bicep("format", bicepFilePath, "--newLine", "CRLF", "--indentSize", "4");

            // Should format successfully
            using (new AssertionScope())
            {
                result.Should().Be(0);
                output.Should().BeEmpty();
                AssertNoErrors(error);
            }

            var actual = File.ReadAllText(bicepFilePath);

            string lf = ((char)10).ToString();
            string cr = ((char)13).ToString();

            // Should not have new lines in end of file
            actual.Should().NotEndWith(lf);
            actual.Should().NotEndWith(cr);

            // Should use CRLF lineEndings
            actual.Should().Contain(cr + lf);

            // Should use four spaces for indentation
            var indentation = GetIndentation(actual);
            if (indentation.Length > 0)
            {
                indentation.Should().Be("    ");
            }
        }

        [TestMethod]
        public async Task Format_with_outdir_and_stdout_ShouldFail_WithExpectedErrorMessage()
        {
            var bicepPath = FileHelper.SaveResultFile(TestContext, "input.bicep", @"
output myOutput string = 'hello!'
            ");

            var outputDirectory = FileHelper.GetResultFilePath(TestContext, "outputdir");

            var (output, error, result) = await Bicep("format", "--outdir", outputDirectory, "--stdout", bicepPath);

            result.Should().Be(1);
            output.Should().BeEmpty();
            error.Should().MatchRegex(@"The --outdir and --stdout parameters cannot both be used");
        }

        [TestMethod]
        public async Task Format_with_outfile_and_stdout_ShouldFail_WithExpectedErrorMessage()
        {
            var bicepPath = FileHelper.SaveResultFile(TestContext, "input.bicep", @"
output myOutput string = 'hello!'
            ");

            var (output, error, result) = await Bicep("format", "--outfile", bicepPath, "--stdout", bicepPath);

            result.Should().Be(1);
            output.Should().BeEmpty();
            error.Should().MatchRegex(@"The --outfile and --stdout parameters cannot both be used");
        }

        [TestMethod]
        public async Task Format_with_outdir_and_outfile_ShouldFail_WithExpectedErrorMessage()
        {
            var bicepPath = FileHelper.SaveResultFile(TestContext, "input.bicep", @"
output myOutput string = 'hello!'
            ");

            var outputDirectory = FileHelper.GetResultFilePath(TestContext, "outputdir");
            var outFilePath = Path.Combine(outputDirectory, DataSet.TestFileMainFormatted);

            var (output, error, result) = await Bicep("format", "--outdir", outputDirectory, "--outfile", outFilePath, bicepPath);

            result.Should().Be(1);
            output.Should().BeEmpty();
            error.Should().MatchRegex(@"The --outdir and --outfile parameters cannot both be used");
        }

        [TestMethod]
        public async Task Format_with_indentSize_and_indentKindTab_ShouldFail_WithExpectedErrorMessage()
        {
            var bicepPath = FileHelper.SaveResultFile(TestContext, "input.bicep", @"
output myOutput string = 'hello!'
            ");

            var (output, error, result) = await Bicep("format", "--indentSize", "2", "--indentKind", "Tab", bicepPath);

            result.Should().Be(1);
            output.Should().BeEmpty();
            error.Should().MatchRegex(@"The --indentSize cannot be used when --indentKind is ""Tab""");
        }

        [TestMethod]
        public async Task Format_WithNonExistantOutDir_ShouldFail_WithExpectedErrorMessage()
        {
            var bicepPath = FileHelper.SaveResultFile(TestContext, "input.bicep", @"
output myOutput string = 'hello!'
            ");

            var outputFileDir = FileHelper.GetResultFilePath(TestContext, "outputdir");
            var (output, error, result) = await Bicep("format", "--outdir", outputFileDir, bicepPath);

            result.Should().Be(1);
            output.Should().BeEmpty();
            error.Should().MatchRegex(@"The specified output directory "".*outputdir"" does not exist");
        }

        [TestMethod]
        [DynamicData(nameof(GetValidDataSets), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task Format_Valid_Files_with_OutDir_ShouldSucceed(DataSet dataSet)
        {
            var outputDirectory = dataSet.SaveFilesToTestDirectory(TestContext);
            var bicepFilePath = Path.Combine(outputDirectory, DataSet.TestFileMain);
            var originalContent = File.ReadAllText(bicepFilePath);
            if (string.IsNullOrEmpty(originalContent))
            {
                // Skip if input file is empty
                return;
            }
            var (output, error, result) = await Bicep("format", bicepFilePath, "--outdir", outputDirectory);

            // Should format successfully
            using (new AssertionScope())
            {
                result.Should().Be(0);
                output.Should().BeEmpty();
                AssertNoErrors(error);
            }

            var actual = File.ReadAllText(bicepFilePath);

            string lf = ((char)10).ToString();
            string cr = ((char)13).ToString();

            // Should not have new lines in end of file
            actual.Should().NotEndWith(lf);
            actual.Should().NotEndWith(cr);

            // Should use two spaces for indentation
            var indentation = GetIndentation(actual);
            if (indentation.Length > 0)
            {
                indentation.Should().Be("  ");
            }
        }

        [DataRow("DoesNotExist.bicep", new[] { "--stdout" }, @"An error occurred reading file. Could not find file '.+DoesNotExist.bicep'")]
        [DataRow("DoesNotExist.bicep", new[] { "--outdir", "." }, @"An error occurred reading file. Could not find file '.+DoesNotExist.bicep'")]
        [DataRow("DoesNotExist.bicep", new[] { "--outfile", "file1" }, @"An error occurred reading file. Could not find file '.+DoesNotExist.bicep'")]
        [DataRow("WrongDir\\Fake.bicep", new[] { "--stdout" }, @"An error occurred reading file. Could not find .+'.+WrongDir[\\/]Fake.bicep'")]
        [DataRow("WrongDir\\Fake.bicep", new[] { "--outdir", "." }, @"An error occurred reading file. Could not find .+'.+WrongDir[\\/]Fake.bicep'")]
        [DataRow("WrongDir\\Fake.bicep", new[] { "--outfile", "file1" }, @"An error occurred reading file. Could not find .+'.+WrongDir[\\/]Fake.bicep'")]
        [DataTestMethod]
        public async Task Format_InvalidInputPaths_ShouldProduceExpectedError(string badPath, string[] args, string expectedErrorRegex)
        {
            var (output, error, result) = await Bicep(new[] { "format" }.Concat(args).Append(badPath).ToArray());

            result.Should().Be(1);
            output.Should().BeEmpty();
        }

        [TestMethod]
        public async Task Format_LockedOutputFile_ShouldProduceExpectedError()
        {
            var inputFile = FileHelper.SaveResultFile(this.TestContext, "Empty.bicep", DataSets.Empty.Bicep);

            // ReSharper disable once ConvertToUsingDeclaration
            using (new FileStream(inputFile, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            {
                // keep the output stream open while we attempt to write to it
                // this should force an access denied error
                var (output, error, result) = await Bicep("format", inputFile);

                result.Should().Be(1);
                output.Should().BeEmpty();
                error.Should().Contain("Empty.bicep");
            }
        }

        private static IEnumerable<object[]> GetValidDataSets() => DataSets
            .AllDataSets
            .Where(ds => ds.IsValid)
            .ToDynamicTestData();

        private static IEnumerable<object[]> GetInvalidDataSets() => DataSets
            .AllDataSets
            .Where(ds => ds.IsValid == false)
            .ToDynamicTestData();

        private static IEnumerable<object[]> GetValidDataSetsWithExternalModules() => DataSets
            .AllDataSets
            .Where(ds => ds.IsValid && ds.HasExternalModules)
            .ToDynamicTestData();

        /// <summary>
        /// Find first indented line that follows a brace ({) or
        /// bracket ([) and returns the indentation part of that line.
        /// </summary>
        /// <param name="content">Content of the file</param>
        /// <returns>Indentation part of first indented line.</returns>
        private static string GetIndentation(string content)
        {
            var indentation = Regex.Match(content, @"(\{|\[)\r?\n(?<indentation>\s+)\S", RegexOptions.Multiline);
            if (indentation.Success)
            {
                return indentation.Groups["indentation"].Value;
            }
            return string.Empty;
        }
    }
}
