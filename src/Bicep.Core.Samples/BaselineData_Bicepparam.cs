// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.UnitTests.Baselines;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Bicep.Core.Samples
{
    public class BaselineData_Bicepparam
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

                data = Filter switch {
                    TestDataFilterType.ValidOnly => data.Where(x => x.IsValid),
                    TestDataFilterType.InvalidOnly => data.Where(x => !x.IsValid),
                    _ => data,
                };

                return data.Select(x => new object[] { x });
            }

            public string GetDisplayName(MethodInfo methodInfo, object[] data)
            {
                var baselineData = (data[0] as BaselineData_Bicepparam)!;

                return $"{methodInfo.Name}({baselineData.paramsFile.StreamPath})";
            }
        }

        public record BaselineData(
            BaselineFolder OutputFolder,
            BaselineFile Parameters,
            BaselineFile? Compiled,
            BaselineFile Bicep,
            BaselineFile Tokens,
            BaselineFile Diagnostics,
            BaselineFile Symbols,
            BaselineFile Syntax,
            BaselineFile Formatted);

        private readonly EmbeddedFile paramsFile;

        public bool IsValid => !paramsFile.StreamPath.StartsWith("Files/baselines_bicepparam/Invalid_");

        public BaselineData_Bicepparam(EmbeddedFile paramsFile)
        {
            this.paramsFile = paramsFile;
        }

        public BaselineData GetData(TestContext testContext)
        {
            var outputFolder = BaselineFolder.BuildOutputFolder(testContext, paramsFile);

            using (new AssertionScope())
            {
                return new(
                    OutputFolder: outputFolder,
                    Parameters: outputFolder.GetFileOrEnsureCheckedIn("parameters.bicepparam"),
                    Compiled: outputFolder.TryGetFile("parameters.json"),
                    Bicep: outputFolder.GetFileOrEnsureCheckedIn("main.bicep"),
                    Tokens: outputFolder.GetFileOrEnsureCheckedIn("parameters.tokens.bicepparam"),
                    Diagnostics: outputFolder.GetFileOrEnsureCheckedIn("parameters.diagnostics.bicepparam"),
                    Symbols: outputFolder.GetFileOrEnsureCheckedIn("parameters.symbols.bicepparam"),
                    Syntax: outputFolder.GetFileOrEnsureCheckedIn("parameters.syntax.bicepparam"),
                    Formatted: outputFolder.GetFileOrEnsureCheckedIn("parameters.formatted.bicepparam"));
            }
        }

        private static IEnumerable<BaselineData_Bicepparam> GetAllExampleData()
        {
            var embeddedFiles = EmbeddedFile.LoadAll(
                typeof(Bicep.Core.Samples.AssemblyInitializer).Assembly,
                "baselines_bicepparam",
                streamName => Path.GetFileName(streamName) == "parameters.bicepparam");

            // ensure this list is kept up-to-date to validate that we're picking all of the baseline tests
            embeddedFiles.Select(x => x.StreamPath).Should().BeEquivalentTo(
                "Files/baselines_bicepparam/Invalid_Parameters/parameters.bicepparam",
                "Files/baselines_bicepparam/Invalid_MismatchedTypes/parameters.bicepparam",
                "Files/baselines_bicepparam/Parameters/parameters.bicepparam");

            foreach (var file in embeddedFiles)
            {
                yield return new BaselineData_Bicepparam(file);
            }
        }
    }
}
