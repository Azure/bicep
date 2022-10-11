// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Bicep.Core;
using Bicep.Core.Diagnostics;
using Bicep.Core.FileSystem;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Bicep.LangServer.IntegrationTests.Completions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.LangServer.IntegrationTests
{
    [TestClass]
    public class SnippetTemplatesTests
    {
        private static ServiceBuilder Services => new ServiceBuilder().WithEmptyAzResources();

        [NotNull]
        public TestContext? TestContext { get; set; }

        [DataTestMethod]
        [DynamicData(nameof(GetSnippetCompletionData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(CompletionData), DynamicDataDisplayName = nameof(CompletionData.GetDisplayName))]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public void VerifySnippetTemplatesAreErrorFree(CompletionData completionData)
        {
            string pathPrefix = $"Files/SnippetTemplates/{completionData.Prefix}";

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
                case "res-scoped-lock":
                    // This file cannot be directly compiled, because the snippet declares an extension resource without the resource being extended.
                    // This is deliberate behavior.
                    return;
            }

            var compilation = Services.BuildCompilation(files, mainUri);
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

        [DataTestMethod]
        [DynamicData(nameof(GetSnippetCompletionData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(CompletionData), DynamicDataDisplayName = nameof(CompletionData.GetDisplayName))]
        public void VerifySnippetTemplatesDoNotContainResourceGroupLocation(CompletionData completionData)
        {
            if (
                completionData.SnippetText.Contains("resourceGroup().location")
                || completionData.SnippetText.Contains("deployment().location")
                )
            {
                Assert.Fail("Snippet templates should not contain resourceGroup().location or deployment().location. Snippet: {0}.", completionData.Prefix);
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(GetSnippetCompletionData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(CompletionData), DynamicDataDisplayName = nameof(CompletionData.GetDisplayName))]
        public void VerifySnippetTemplatesUseCorrectLocationSyntax(CompletionData completionData)
        {
            if (completionData.SnippetText.Contains("location:") && !completionData.SnippetText.Contains("location: 'global'")) // location: 'global' is okay
            {
                completionData.SnippetText.Should().MatchRegex("location: \\/\\*\\$\\{[0-9]+:location\\}\\*\\/", "All snippets that include a location property should use this format for it: \"location: /*${xxx:location}*/'location'\". (Snippet: " + completionData.Prefix + ")");
            }
        }


        private static IEnumerable<object[]> GetSnippetCompletionData() => CompletionDataHelper.GetSnippetCompletionData();
    }
}
