// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.Utils;
using Bicep.LangServer.IntegrationTests;
using Bicep.LangServer.IntegrationTests.Assertions;
using Bicep.LangServer.UnitTests.Mocks;
using Bicep.LanguageServer.Handlers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;

namespace Bicep.LangServer.UnitTests.Handlers
{
    [TestClass]
    public class BicepExternalSourceDocumentLinkHandlerTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        private ServiceBuilder GetServices(IContainerRegistryClientFactory clientFactory)
        {
            var services = new ServiceBuilder()
                .WithFeatureOverrides(new(OptionalModuleNamesEnabled: true))
                .WithContainerRegistryClientFactory(clientFactory);

            return services;
        }

        private IModuleDispatcher CreateModuleDispatcher(IContainerRegistryClientFactory clientFactory)
        {
            var featureProviderFactory = BicepTestConstants.CreateFeatureProviderFactory(new FeatureProviderOverrides(PublishSourceEnabled: true));
            var dispatcher = ServiceBuilder.Create(s => s.WithDisabledAnalyzersConfiguration()
                .AddSingleton(clientFactory)
                .AddSingleton(BicepTestConstants.TemplateSpecRepositoryFactory)
                .AddSingleton(featureProviderFactory)
                ).Construct<IModuleDispatcher>();
            return dispatcher;
        }

        private async Task<DocumentLink<ExternalSourceDocumentLinkData>[]> GetLinksForDisplayedDocument(IModuleDispatcher moduleDispatcher, TextDocumentIdentifier documentId)
        {
            var links = BicepExternalSourceDocumentLinkHandler.GetDocumentLinks(
                moduleDispatcher,
                new DocumentLinkParams() { TextDocument = documentId },
                CancellationToken.None)
            .ToArray();

            var server = new LanguageServerMock();
            ShowDocumentParams? showDocumentParams = null;
            server.WindowMock.OnShowDocument(
                p =>
                {
                    showDocumentParams = p;
                    //asdfg onShowDocument(p);
                },
                enableClientCapability: true/*asdfg*/);

            server.Mock
                .Setup(m => m.SendNotification("bicep/triggerEditorCompletion"))
                .Callback((string notification) =>
                {
                    if (showDocumentParams == null)
                    {
                        throw new Exception("Completion was triggered but no show document call was made");
                    }
                    else if (showDocumentParams.Selection == null)
                    {
                        throw new Exception("No selection was given in the show document call");
                    }
                    else if (showDocumentParams.Selection.Start != showDocumentParams.Selection.End)
                    {
                        throw new Exception("Completion was triggered on a non-empty selection");
                    }

                    //asdfg string stringTriggeredForCompletion = GetStringContentsAtDocumentPosition(showDocumentParams.Uri, showDocumentParams.Selection.Start);
                    //asdfg onTriggerCompletion(stringTriggeredForCompletion);
                });

            var resolvedLinks = new List<DocumentLink<ExternalSourceDocumentLinkData>>();
            foreach (var link in links)
            {
                var resolvedLink = await BicepExternalSourceDocumentLinkHandler.ResolveDocumentLink(link, moduleDispatcher, server.Mock.Object);
                resolvedLinks.Add(resolvedLink);
            }
            return resolvedLinks.ToArray();
        }

        /// <param name="displayedModuleTarget">E.g. mockregistry.io/test/module1:v1</param>
        private TextDocumentIdentifier GetDocumentIdForExternalModuleSource(IModuleDispatcher moduleDispatcher, string displayedModuleTarget)
        {
            var moduleRef = OciArtifactReference.TryParseModule(displayedModuleTarget).Unwrap();
            var moduleExtRef = new ExternalSourceReference(moduleRef, null).WithRequestForSourceFile("main.bicep");
            var moduleId = new TextDocumentIdentifier(moduleExtRef.ToUri());
            return moduleId;
        }

        [TestMethod]
        public async Task IfNotShowingExternalSourceCode_ThenReturnNoLinks()
        {
            var clientFactory = await RegistryHelper.CreateMockRegistryClientWithPublishedModulesAsync([
                ("br:mockregistry.io/test/module1:v1", "param p1 bool", withSource: true),
            ]);
            var moduleDispatcher = CreateModuleDispatcher(clientFactory);
            var result = await CompilationHelper.RestoreAndCompile(
                GetServices(clientFactory),
                ("main.bicep", """
                    module m1 'br:mockregistry.io/test/module1:v1' = {
                        params: {
                            p1: true
                      }
                    }
                    """));

            var links = await GetLinksForDisplayedDocument(moduleDispatcher, new TextDocumentIdentifier("file:///main.bicep"));

            links.Should().HaveCount(0);
        }

        [TestMethod]
        public async Task Asdfg()
        {
            // Setup: 
            //   module1 is published with source
            //   module2 references module1 and is published with source
            var clientFactory = await RegistryHelper.CreateMockRegistryClientWithPublishedModulesAsync([
                ("br:mockregistry.io/test/module1:v1", "param p1 bool", withSource: true),
                ("br:mockregistry.io/test/module2:v2", """
                    module m1 'br:mockregistry.io/test/module1:v1' = {
                        name: 'm1'
                        params: {
                            p1: true
                      }
                    }
                    """, withSource: true)
                ]);
            var moduleDispatcher = CreateModuleDispatcher(clientFactory);
            var result = await CompilationHelper.RestoreAndCompile(
                GetServices(clientFactory),
                ("main.bicep", """
                    module m2 'br:mockregistry.io/test/module2:v2' = {
                        name: 'm2'
                    }
                    """));

            // Get a URI for displaying the soure of module2
            var moduleDocId = GetDocumentIdForExternalModuleSource(moduleDispatcher, "mockregistry.io/test/module2:v2");

            // Act: Get all links inside module2's source display
            var links = await GetLinksForDisplayedDocument(moduleDispatcher, moduleDocId);

            links.Should().HaveCount(1);

            // Act: Resolve the links
            //var resolvedLinks = links.Select(l => BicepExternalSourceDocumentLinkHandler.ResolveDocumentLink()

            //var messageListener = new MultipleMessageListener<ShowMessageParams>();
            //var telemetryEventsListener = new MultipleMessageListener<TelemetryEventParams>();

            //using var helper = await LanguageServerHelper.StartServer(
            //    this.TestContext,
            //    options => options
            //        .OnShowMessage(messageListener.AddMessage)
            //        //.OnTelemetryEvent(telemetryEventsListener.AddMessage)
            //);
            //var client = helper.Client;


            //var response = await client.SendRequest(new ImportKubernetesManifestRequest(manifestFile), default);
            //response.BicepFilePath.Should().BeNull();

            //var telemetry = await telemetryEventsListener.WaitForAll();
            //telemetry.Should().ContainEvent("ImportKubernetesManifest/failure", new JObject
            //{
            //    ["failureType"] = "DeserializeYamlFailed",
            //});




            //ShowDocumentParams? showDocumentParams = null;
            //server.WindowMock.OnShowDocument(
            //    p =>
            //    {
            //        showDocumentParams = p;
            //        onShowDocument(p);
            //    },
            //    enableClientCapability: enableShowDocumentCapability);

            //server.Mock
            //    .Setup(m => m.SendNotification("bicep/triggerEditorCompletion"))
            //    .Callback((string notification) =>
            //    {
            //        if (showDocumentParams == null)
            //        {
            //            throw new Exception("Completion was triggered but no show document call was made");
            //        }
            //        else if (showDocumentParams.Selection == null)
            //        {
            //            throw new Exception("No selection was given in the show document call");
            //        }
            //        else if (showDocumentParams.Selection.Start != showDocumentParams.Selection.End)
            //        {
            //            throw new Exception("Completion was triggered on a non-empty selection");
            //        }

            //        string? stringTriggeredForCompletion = GetStringContentsAtDocumentPosition(showDocumentParams.Uri, showDocumentParams.Selection.Start);
            //        onTriggerCompletion(stringTriggeredForCompletion);
            //    });

            //return server;

            //var resolvedLink = await BicepExternalSourceDocumentLinkHandler.ResolveDocumentLink(links[0], moduleDispatcher, server.Mock.Object);
            //var a = resolvedLink;

            //var message = await messageListener.WaitNext();
            //message.Should().HaveMessageAndType(
            //    "Failed to deserialize kubernetes manifest YAML: (Lin: 1, Col: 4, Chr: 5) - (Lin: 1, Col: 25, Chr: 26): Expected dictionary node.",
            //    MessageType.Error);


        }
    }
}
