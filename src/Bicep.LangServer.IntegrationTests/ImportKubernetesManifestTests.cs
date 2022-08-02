// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Bicep.Core.UnitTests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bicep.Core.UnitTests.Assertions;
using Bicep.LanguageServer.Handlers;
using System;
using System.Collections.Generic;
using System.IO;
using Bicep.Core.UnitTests;
using System.Linq;
using FluentAssertions;
using Bicep.Core.Extensions;
using Bicep.Core.UnitTests.Baselines;

namespace Bicep.LangServer.IntegrationTests
{
    [TestClass]
    public class ImportKubernetesManifestTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        private static IEnumerable<object[]> GetWorkingExampleData()
            => EmbeddedFile.LoadAll(
                typeof(ImportKubernetesManifestTests).Assembly,
                "ImportKubernetesManifest",
                streamName => Path.GetExtension(streamName) == ".yml")
            .Select(x => new object[] { x });

        [DataTestMethod]
        [DynamicData(nameof(GetWorkingExampleData), DynamicDataSourceType.Method)]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public async Task ImportKubernetesManifest_generates_valid_bicep_files_from_kubernetes_manifests(EmbeddedFile embeddedYml)
        {
            var baselineFolder = BaselineFolder.BuildOutputFolder(TestContext, embeddedYml);
            var yamlFile = baselineFolder.EntryFile;
            var bicepFile = baselineFolder.GetFileOrEnsureCheckedIn(Path.ChangeExtension(embeddedYml.FileName, ".bicep"));

            var features = BicepTestConstants.Features with { ImportsEnabled = true, };

            using var helper = await LanguageServerHelper.StartServerWithClientConnectionAsync(
                this.TestContext,
                options => {},
                new LanguageServer.Server.CreationOptions(Features: features));
            var client = helper.Client;

            var response = await client.SendRequest(new ImportKubernetesManifestRequest(yamlFile.OutputFilePath), default);

            bicepFile.ShouldHaveExpectedValue();

            var context = new CompilationHelper.CompilationHelperContext(Features: features);
            CompilationHelper.Compile(context, bicepFile.ReadFromOutputFolder()).Should().GenerateATemplate();
        }
    }
}
