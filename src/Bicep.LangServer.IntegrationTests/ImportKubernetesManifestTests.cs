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
using Bicep.Core.Features;
using Bicep.Core.UnitTests.Baselines;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using Bicep.LangServer.IntegrationTests.Assertions;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Newtonsoft.Json.Linq;
using Bicep.Core;

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
            var telemetryEventsListener = new MultipleMessageListener<TelemetryEventParams>();
            var baselineFolder = BaselineFolder.BuildOutputFolder(TestContext, embeddedYml);
            var yamlFile = baselineFolder.EntryFile;
            var bicepFile = baselineFolder.GetFileOrEnsureCheckedIn(Path.ChangeExtension(embeddedYml.FileName, ".bicep"));

            var features = BicepTestConstants.Features with { ImportsEnabled = true, };

            using var helper = await LanguageServerHelper.StartServerWithClientConnectionAsync(
                this.TestContext,
                options => options
                    .OnTelemetryEvent(telemetryEventsListener.AddMessage),
                new LanguageServer.Server.CreationOptions(FeatureProviderFactory: IFeatureProviderFactory.WithStaticFeatureProvider(features)));
            var client = helper.Client;

            var response = await client.SendRequest(new ImportKubernetesManifestRequest(yamlFile.OutputFilePath), default);

            var telemetry = await telemetryEventsListener.WaitForAll();
            telemetry.Should().ContainEvent("ImportKubernetesManifest/success", new JObject
                {
                    ["success"] = "true",
                });

            bicepFile.ShouldHaveExpectedValue();

            CompilationHelper.Compile(new ServiceBuilder().WithFeatureProvider(features), bicepFile.ReadFromOutputFolder()).Should().GenerateATemplate();
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

            using var helper = await LanguageServerHelper.StartServerWithClientConnectionAsync(
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
