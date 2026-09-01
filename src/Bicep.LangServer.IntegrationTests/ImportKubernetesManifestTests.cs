// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Baselines;
using Bicep.Core.UnitTests.Utils;
using Bicep.LangServer.IntegrationTests.Assertions;
using Bicep.LanguageServer.Features.Custom.ImportKubernetesManifest;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;

namespace Bicep.LangServer.IntegrationTests
{
    [TestClass]
    public class ImportKubernetesManifestTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [DataTestMethod]
        [EmbeddedFilesTestData(@"Files/ImportKubernetesManifest/.*/.*\.yml")]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public async Task ImportKubernetesManifest_generates_valid_bicep_files_from_kubernetes_manifests(EmbeddedFile embeddedYml)
        {
            var baselineFolder = BaselineFolder.BuildOutputFolder(TestContext, embeddedYml);
            var yamlFile = baselineFolder.EntryFile;
            var bicepFile = baselineFolder.GetFileOrEnsureCheckedIn(Path.ChangeExtension(embeddedYml.FileName, ".bicep"));

            using var helper = await LanguageServerHelper.StartServer(
                this.TestContext,
                options => { },
                services => services.WithFeatureOverrides(new(TestContext)));
            var client = helper.Client;

            var response = await client.SendRequest(new ImportKubernetesManifestRequest(yamlFile.OutputFilePath), default);

            bicepFile.ShouldHaveExpectedValue();

            CompilationHelper.Compile(bicepFile.ReadFromOutputFolder()).Should().GenerateATemplate();
        }

        [TestMethod]
        public async Task ImportKubernetesManifest_error_handling()
        {
            var messageListener = new MultipleMessageListener<ShowMessageParams>();
            var manifestFile = FileHelper.SaveResultFile(TestContext, "manifest.yml", @"
    NOT A VALID YAML FILE
");
            var bicepFile = Path.ChangeExtension(manifestFile, ".bicep");

            using var helper = await LanguageServerHelper.StartServer(
                this.TestContext,
                options => options
                    .OnShowMessage(messageListener.AddMessage));
            var client = helper.Client;

            var response = await client.SendRequest(new ImportKubernetesManifestRequest(manifestFile), default);
            response.BicepFilePath.Should().BeNull();

            var message = await messageListener.WaitNext();
            message.Should().HaveMessageAndType(
                "Failed to deserialize kubernetes manifest YAML: Expected dictionary node.",
                MessageType.Error);
        }
    }
}
