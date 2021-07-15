// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Bicep.LangServer.IntegrationTests.Completions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.LangServer.IntegrationTests
{
    [TestClass]
    public class SnippetTemplatesTests 
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [DataTestMethod]
        [DynamicData(nameof(GetSnippetCompletionData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(CompletionData), DynamicDataDisplayName = nameof(CompletionData.GetDisplayName))]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public void VerifySnippetTemplatesAreErrorFree(CompletionData completionData)
        {
            var mainUri = new Uri("file:///main.bicep");
            var bicepContents = completionData.SnippetText;
            var files = new Dictionary<Uri, string>
            {
                [mainUri] = bicepContents,
            };
            var prefix = completionData.Prefix;

            // Template - module.bicep requires a path. So we'll create param.bicep file and
            // specify it in module snippet template
            if (prefix == "module")
            {
                var paramUri = new Uri("file:///param.bicep");
                files.Add(paramUri, "param myParam string = 'test'");
            }

            var compilation = new Compilation(TestTypeHelper.CreateEmptyProvider(), SourceFileGroupingFactory.CreateForFiles(files, mainUri, BicepTestConstants.FileResolver));
            var semanticModel = compilation.GetEntrypointSemanticModel();

            if (semanticModel.HasErrors())
            {
                string pathPrefix = $"Completions/SnippetTemplates/{completionData.Prefix}";
                var outputDirectory = FileHelper.SaveEmbeddedResourcesWithPathPrefix(TestContext, typeof(CompletionTests).Assembly, pathPrefix);
                var errors = semanticModel.GetAllDiagnostics().Where(x => x.Level == DiagnosticLevel.Error);
                var sourceTextWithDiags = OutputHelper.AddDiagsToSourceText(bicepContents, "\n", errors, diag => OutputHelper.GetDiagLoggingString(bicepContents, outputDirectory, diag));
                Assert.Fail("Template with prefix {0} contains errors. Please fix following errors:\n {1}", completionData.Prefix, sourceTextWithDiags);
            }
        }

        private static IEnumerable<object[]> GetSnippetCompletionData() => CompletionDataHelper.GetSnippetCompletionData();
    }
}
