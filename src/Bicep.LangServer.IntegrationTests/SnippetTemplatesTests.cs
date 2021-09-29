// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.FileSystem;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
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
            string pathPrefix = $"Completions/SnippetTemplates/{completionData.Prefix}";

            var outputDirectory = FileHelper.SaveEmbeddedResourcesWithPathPrefix(TestContext, typeof(SnippetTemplatesTests).Assembly, pathPrefix);

            var mainUri = PathHelper.FilePathToFileUrl(Path.Combine(outputDirectory, "main.bicep"));
            var bicepContents = completionData.SnippetText;
            var files = new Dictionary<Uri, string>
            {
                [mainUri] = bicepContents,
            };

            // overrides for certain snippets which have contextual dependencies (e.g. external files)
            switch (completionData.Prefix)
            {
                case "module":
                    var paramUri = PathHelper.FilePathToFileUrl(Path.Combine(outputDirectory, "param.bicep"));
                    files.Add(paramUri, "param myParam string = 'test'");
                    break;
                case "res-logic-app-from-file":
                    var requiredFile = PathHelper.FilePathToFileUrl(Path.Combine(outputDirectory, "REQUIRED"));
                    files.Add(requiredFile, @"{""definition"":{""$schema"":""https://schema.management.azure.com/providers/Microsoft.Logic/schemas/2016-06-01/workflowdefinition.json#"",""contentVersion"":""1.0.0.0"",""outputs"":{}}}");
                    return;
            }

            var compilation = new Compilation(TestTypeHelper.CreateEmptyProvider(), SourceFileGroupingFactory.CreateForFiles(files, mainUri, BicepTestConstants.FileResolver), BicepTestConstants.BuiltInConfiguration);
            var semanticModel = compilation.GetEntrypointSemanticModel();

            if (semanticModel.HasErrors())
            {
                var errors = semanticModel.GetAllDiagnostics().Where(x => x.Level == DiagnosticLevel.Error);
                var sourceTextWithDiags = OutputHelper.AddDiagsToSourceText(bicepContents, "\n", errors, diag => OutputHelper.GetDiagLoggingString(bicepContents, outputDirectory, diag));
                Assert.Fail("Template with prefix {0} contains errors. Please fix following errors:\n {1}", completionData.Prefix, sourceTextWithDiags);
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(GetSnippetCompletionData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(CompletionData), DynamicDataDisplayName = nameof(CompletionData.GetDisplayName))]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public void VerifySnippetTemplatesDoNotContainTargetScope(CompletionData completionData)
        {
            var parser = new Parser(completionData.SnippetText);
            var programSyntax = parser.Program();
            var children = programSyntax.Children;

            if (children.Any(x => x is TargetScopeSyntax targetScopeSyntax && targetScopeSyntax is not null))
            {
                Assert.Fail("Snippet templates should not contain targetScope. Please remove targetScope from template with prefix {0}.", completionData.Prefix);
            }
        }

        private static IEnumerable<object[]> GetSnippetCompletionData() => CompletionDataHelper.GetSnippetCompletionData();
    }
}
