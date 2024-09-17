// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Reflection;
using Bicep.Core.UnitTests.Baselines;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.Samples
{
    public class BaselineData_BicepDeploy
    {
        public enum TestDataFilterType
        {
            All = 0,
            ValidOnly,
            InvalidOnly,
        }

        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
        public class TestDataAttribute : Attribute, ITestDataSource
        {
            public TestDataFilterType Filter { get; set; }

            public TestDataAttribute()
            {
                Filter = TestDataFilterType.All;
            }

            public IEnumerable<object[]> GetData(MethodInfo methodInfo)
            {
                var data = GetAllExampleData();

                data = Filter switch
                {
                    TestDataFilterType.ValidOnly => data.Where(x => x.IsValid),
                    TestDataFilterType.InvalidOnly => data.Where(x => !x.IsValid),
                    _ => data,
                };

                return data.Select(x => new object[] { x });
            }

            public string? GetDisplayName(MethodInfo methodInfo, object?[]? data)
            {
                var baselineData = (BaselineData_BicepDeploy)data?[0]!;

                return $"{methodInfo.Name} ({baselineData.deployFile.StreamPath})";
            }
        }

        public record BaselineData(
            BaselineFolder OutputFolder,
            BaselineFile DeployFile,
            BaselineFile? CompiledFile,
            BaselineFile BicepFile,
            BaselineFile TokensFile,
            BaselineFile DiagnosticsFile,
            BaselineFile SymbolsFile,
            BaselineFile SyntaxFile,
            BaselineFile FormattedFile);

        private readonly EmbeddedFile deployFile;

        public bool IsValid => !deployFile.StreamPath.StartsWith("Files/baselines_bicepdeploy/Invalid_");

        public BaselineData_BicepDeploy(EmbeddedFile deployFile)
        {
            this.deployFile = deployFile;
        }

        public BaselineData GetData(TestContext testContext)
        {
            var outputFolder = BaselineFolder.BuildOutputFolder(testContext, deployFile);

            using (new AssertionScope())
            {
                return new(
                    OutputFolder: outputFolder,
                    DeployFile: outputFolder.GetFileOrEnsureCheckedIn("main.bicepdeploy"),
                    CompiledFile: outputFolder.TryGetFile("main.bicepdeploy.json"),
                    BicepFile: outputFolder.GetFileOrEnsureCheckedIn("main.bicep"),
                    TokensFile: outputFolder.GetFileOrEnsureCheckedIn("main.tokens.bicepdeploy"),
                    DiagnosticsFile: outputFolder.GetFileOrEnsureCheckedIn("main.diagnostics.bicepdeploy"),
                    SymbolsFile: outputFolder.GetFileOrEnsureCheckedIn("main.symbols.bicepdeploy"),
                    SyntaxFile: outputFolder.GetFileOrEnsureCheckedIn("main.syntax.bicepdeploy"),
                    FormattedFile: outputFolder.GetFileOrEnsureCheckedIn("main.formatted.bicepdeploy"));
            }
        }

        private static IEnumerable<BaselineData_BicepDeploy> GetAllExampleData()
        {
            var embeddedFiles = EmbeddedFile.LoadAll(
                typeof(AssemblyInitializer).Assembly,
                "baselines_bicepdeploy",
                streamName => Path.GetFileName(streamName) == "main.bicepdeploy");

            // ensure this list is kept up-to-date to validate that we're picking all of the baseline tests
            string[] expectedFolders = ["Empty", "Basic"];
            var expectedFiles = expectedFolders.Select(x => $"Files/baselines_bicepdeploy/{x}/main.bicepdeploy");

            embeddedFiles.Select(x => x.StreamPath).Should().BeEquivalentTo(expectedFiles);

            foreach (var file in embeddedFiles)
            {
                yield return new BaselineData_BicepDeploy(file);
            }
        }
    }
}
