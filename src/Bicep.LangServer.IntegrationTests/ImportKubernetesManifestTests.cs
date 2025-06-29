// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Baselines;
using Bicep.Core.UnitTests.Utils;
using Bicep.LangServer.IntegrationTests.Assertions;
using Bicep.LanguageServer.Handlers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
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
            var telemetryEventsListener = new MultipleMessageListener<TelemetryEventParams>();
            var baselineFolder = BaselineFolder.BuildOutputFolder(TestContext, embeddedYml);
            var yamlFile = baselineFolder.EntryFile;
            var bicepFile = baselineFolder.GetFileOrEnsureCheckedIn(Path.ChangeExtension(embeddedYml.FileName, ".bicep"));

            using var helper = await LanguageServerHelper.StartServer(
                this.TestContext,
                options => options.OnTelemetryEvent(telemetryEventsListener.AddMessage),
                services => services.WithFeatureOverrides(new(TestContext)));
            var client = helper.Client;

            var response = await client.SendRequest(new ImportKubernetesManifestRequest(yamlFile.OutputFilePath), default);

            var telemetry = await telemetryEventsListener.WaitForAll();
            telemetry.Should().ContainEvent("ImportKubernetesManifest/success", new JObject
            {
                ["success"] = "true",
            });

            bicepFile.ShouldHaveExpectedValue();

            CompilationHelper.Compile(bicepFile.ReadFromOutputFolder()).Should().GenerateATemplate();
        }

        [TestMethod]
        public async Task ImportKubernetesManifest_error_handling()
        {
            var messageListener = new MultipleMessageListener<ShowMessageParams>();
            var telemetryEventsListener = new MultipleMessageListener<TelemetryEventParams>();
            var manifestFile = FileHelper.SaveResultFile(TestContext, "manifest.yml", @"
    NOT A VALID YAML FILE
");
            var bicepFile = Path.ChangeExtension(manifestFile, ".bicep");

            using var helper = await LanguageServerHelper.StartServer(
                this.TestContext,
                options => options
                    .OnShowMessage(messageListener.AddMessage)
                    .OnTelemetryEvent(telemetryEventsListener.AddMessage));
            var client = helper.Client;

            var response = await client.SendRequest(new ImportKubernetesManifestRequest(manifestFile), default);
            response.BicepFilePath.Should().BeNull();

            var telemetry = await telemetryEventsListener.WaitForAll();
            telemetry.Should().ContainEvent("ImportKubernetesManifest/failure", new JObject
            {
                ["failureType"] = "DeserializeYamlFailed",
            });

            var message = await messageListener.WaitNext();
            message.Should().HaveMessageAndType(
                "Failed to deserialize kubernetes manifest YAML: (Lin: 1, Col: 4, Chr: 5) - (Lin: 1, Col: 25, Chr: 26): Expected dictionary node.",
                MessageType.Error);
        }
    }
}
