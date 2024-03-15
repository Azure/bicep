// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Reflection;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;
using Bicep.Core.Samples;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.Registry;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Utils;
using Bicep.LangServer.IntegrationTests;
using Bicep.LangServer.IntegrationTests.Assertions;
using Bicep.LangServer.UnitTests.Mocks;
using Bicep.LanguageServer.Handlers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using static System.Net.Mime.MediaTypeNames;

namespace Bicep.LangServer.UnitTests.Handlers
{
    [TestClass]
    public class BicepExternalSourceDocumentLinkHandlerTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        private Dictionary<string, MockFileData> MockFiles = new();
        [NotNull]
        private MockFileSystem? MockFileSystem { get; set; }


        //asdfg
        //#if WINDOWS_BUILD
        //        private const string ROOT = "c:\\";
        //#else
        //        private const string ROOT = "/";
        //#endif

        //asdfg private string CacheRootPath => MockFileSystem.Path.Combine(ROOT, ".br");
        private string CacheRootPath => BicepTestConstants.FeatureProviderFactory.GetFeatureProvider(new Uri("file:///no-file")).CacheRootDirectory;

        private void ResetMockFileSystem()
        {
            MockFileSystem = new(MockFiles);
        }

        [TestInitialize]
        public void TestInitialize()
        {
            ResetMockFileSystem();
        }

        private ServiceBuilder GetServices(IContainerRegistryClientFactory clientFactory)
        {
            return new ServiceBuilder()
                .WithFeatureOverrides(new(OptionalModuleNamesEnabled: true))
                .WithContainerRegistryClientFactory(clientFactory)
                .WithFileSystem(MockFileSystem)
                .WithFeatureProviderFactory(
                    BicepTestConstants.CreateFeatureProviderFactory(new FeatureProviderOverrides(PublishSourceEnabled: true, CacheRootDirectory: CacheRootPath, OptionalModuleNamesEnabled: true))
                )
                .WithTemplateSpecRepositoryFactory(BicepTestConstants.TemplateSpecRepositoryFactory)
                ;
        }

        private IModuleDispatcher CreateModuleDispatcher(IContainerRegistryClientFactory clientFactory)
        {
            return GetServices(clientFactory).Build().Construct<IModuleDispatcher>();
        }

        private async Task<Result<DocumentLink<ExternalSourceDocumentLinkData>[], string>> GetAndResolveLinksForDisplayedDocument(IModuleDispatcher moduleDispatcher, TextDocumentIdentifier documentId)
        {
            var server = new LanguageServerMock();
            ShowMessageParams? showMessageParams = null;
            server.WindowMock.OnShowMessage(
                p =>
                {
                    showMessageParams ??= p;
                });

            var links = BicepExternalSourceDocumentLinkHandler.GetDocumentLinks(
                moduleDispatcher,
                new DocumentLinkParams() { TextDocument = documentId },
                CancellationToken.None)
            .ToArray();

            showMessageParams!.Should().BeNull($"{nameof(BicepExternalSourceDocumentLinkHandler.GetDocumentLinks)} should never show an error, although it can trace output");

            // Call resolve on each link (normally only happens for the one that the user clicks on)
            var resolvedLinks = new List<DocumentLink<ExternalSourceDocumentLinkData>>();
            foreach (var link in links)
            {
                var resolvedLink = await BicepExternalSourceDocumentLinkHandler.ResolveDocumentLink(link, moduleDispatcher, server.Mock.Object);
                resolvedLinks.Add(resolvedLink);

                if (showMessageParams is { })
                {
                    // Return a failure with the message that was shown to the user
                    // (Resolve is allowed to show the user messages because it's only called on user interaction)
                    return new(showMessageParams.Message);
                }
            }
            return new(resolvedLinks.OrderBy(x => x.Range.Start).ToArray());
        }

        /// <param name="displayedModuleTarget">E.g. mockregistry.io/test/module1:v1</param>
        private TextDocumentIdentifier GetDocumentIdForExternalModuleSource(IModuleDispatcher moduleDispatcher, string displayedModuleTarget)
        {
            var moduleRef = OciArtifactReference.TryParseModule(displayedModuleTarget).Unwrap();
            var moduleExtRef = new ExternalSourceReference(moduleRef, null).WithRequestForSourceFile("main.bicep");
            var moduleId = new TextDocumentIdentifier(moduleExtRef.ToUri());
            return moduleId;
        }

        private (string registry, string repo, string tag)[] GetCachedModules()
        {
            var cachedModules = CachedModules.GetCachedRegistryModules(MockFileSystem, CacheRootPath);
            return cachedModules.Select(x => (x.Registry, x.Repository, x.Tag)).ToArray();
        }

        [TestMethod]
        public async Task IfNotShowingExternalModuleSourceCode_ThenReturnNoLinks()
        {
            var clientFactory = await RegistryHelper.CreateMockRegistryClientWithPublishedModulesAsync(
                MockFileSystem, [
                    ("br:mockregistry.io/test/module1:v1", "param p1 bool", withSource: true),
                ]);
            var moduleDispatcher = CreateModuleDispatcher(clientFactory);
            await CompilationHelper.RestoreAndCompile(
                GetServices(clientFactory),
                ("main.bicep", """
                    module m1 'br:mockregistry.io/test/module1:v1' = {
                        params: {
                            p1: true
                      }
                    }
                    """));

            var links = (await GetAndResolveLinksForDisplayedDocument(moduleDispatcher, new TextDocumentIdentifier("file:///main.bicep"))).Unwrap();

            links.Should().HaveCount(0);
        }

        [TestMethod]
        public async Task NestedExternalSource_ShouldGetCorrectLink()
        {
            // Setup: 
            //   module1 is published with source
            //   module2 references module1 and is published with source
            var clientFactory = await RegistryHelper.CreateMockRegistryClientWithPublishedModulesAsync(
                MockFileSystem, [
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

            // Compile some code to force restoration of module2 (which should always be the case if we're displaying its source)
            var moduleDispatcher = CreateModuleDispatcher(clientFactory);
            await CompilationHelper.RestoreAndCompile(
                GetServices(clientFactory),
                ("main.bicep", """
                    module m2 'br:mockregistry.io/test/module2:v2' = {
                        name: 'm2'
                    }
                    """));

            // Get a URI for displaying module2's source
            var module2Uri = GetDocumentIdForExternalModuleSource(moduleDispatcher, "mockregistry.io/test/module2:v2");

            // ACT: Get all links inside module2's source display
            var links = (await GetAndResolveLinksForDisplayedDocument(moduleDispatcher, module2Uri)).Unwrap();

            var link = links.Should().HaveCount(1).And.Subject.First();
            link.Range.Should().HaveRange((0, 10), (0, 46));
            link.Target.Should().NotBeNull();
            var target = new ExternalSourceReference(link.Target!);
            target.FullTitle.Should().Be("br:mockregistry.io/test/module1:v1/main.bicep (module1:v1)");
            target.RequestedFile.Should().Be("main.bicep");
            target.ToArtifactReference().Unwrap().FullyQualifiedReference.Should().Be("br:mockregistry.io/test/module1:v1");
        }

        [TestMethod]
        public async Task DoublyNestedExternalSource_ShouldGetCorrectLink()
        {
            // Setup: 
            //   module1 is published with source
            //   module2 references module1 and is published with source
            //   module3 references module1 and module2 and is published with source
            var clientFactory = await RegistryHelper.CreateMockRegistryClientWithPublishedModulesAsync(
                MockFileSystem, [
                    ("br:mockregistry.io/test/module1:v1", "param p1 bool", withSource: true),
                    ("br:mockregistry.io/test/module1:v2", """
                        module m1 'br:mockregistry.io/test/module1:v1' = {
                            name: 'm1'
                            params: {
                                p1: true
                          }
                        }
                        """, withSource: true),
                    ("br:mockregistry.io/test/module1:v3", """
                        module m1 'br:mockregistry.io/test/module1:v1' = {
                            name: 'm1'
                            params: {
                                p1: true
                          }
                        }
                        module m1v2 'br:mockregistry.io/test/module1:v2' = {
                            name: 'm1v2'
                        }
                        """, withSource: true)
                    ]);

            // Compile some code to force restoration of module1:v3 (which should always be the case if we're displaying its source)
            var moduleDispatcher = CreateModuleDispatcher(clientFactory);
            await CompilationHelper.RestoreAndCompile(
                GetServices(clientFactory),
                ("main.bicep", """
                    module m1v3 'br:mockregistry.io/test/module1:v3' = {
                        name: 'm1v3'
                    }
                    """));

            // Get a URI for displaying module1:v3's source
            var module2Uri = GetDocumentIdForExternalModuleSource(moduleDispatcher, "mockregistry.io/test/module1:v3");

            // ACT: Get all nested links (one for m1:v1 and one for m1:v2)
            var links = (await GetAndResolveLinksForDisplayedDocument(moduleDispatcher, module2Uri)).Unwrap();

            var link1 = links.Should().HaveCount(2).And.Subject.First();
            link1.Range.Should().HaveRange((0, 10), (0, 46));
            link1.Target.Should().NotBeNull();
            var target1 = new ExternalSourceReference(link1.Target!);
            target1.FullTitle.Should().Be("br:mockregistry.io/test/module1:v1/main.bicep (module1:v1)");
            target1.RequestedFile.Should().Be("main.bicep");
            target1.ToArtifactReference().Unwrap().FullyQualifiedReference.Should().Be("br:mockregistry.io/test/module1:v1");

            var link2 = links.Skip(1).First();
            link2.Range.Should().HaveRange((6, 12), (6, 48));
            link2.Target.Should().NotBeNull();
            var target2 = new ExternalSourceReference(link2.Target!);
            target2.FullTitle.Should().Be("br:mockregistry.io/test/module1:v2/main.bicep (module1:v2)");
            target2.RequestedFile.Should().Be("main.bicep");
            target2.ToArtifactReference().Unwrap().FullyQualifiedReference.Should().Be("br:mockregistry.io/test/module1:v2");

            // ACT: Follow that link and get all nested links inside module1:v2
            var links3 = (await GetAndResolveLinksForDisplayedDocument(moduleDispatcher, new TextDocumentIdentifier(target2.ToUri()))).Unwrap();
            var link3 = links3.Should().HaveCount(1).And.Subject.First();
            link3.Target.Should().NotBeNull();
            var target3 = new ExternalSourceReference(link3.Target!);
            target3.ToArtifactReference().Unwrap().FullyQualifiedReference.Should().Be("br:mockregistry.io/test/module1:v1");

            // Should have restored module1:v1
            GetCachedModules().Should().BeEquivalentTo([
                ("mockregistry.io", "test/module1", "v1"),
                ("mockregistry.io", "test/module1", "v2"),
                ("mockregistry.io", "test/module1", "v3"),
            ]);
        }

        //asdfg published without sources

        [TestMethod]
        public async Task NestedExternalSource_ExternalModuleSourceBeingDisplayedNotYetRestored_ShowReturnNoLinks()
        {
            // Setup:
            //   module1 is published with source
            //   module2 references module1 and is published with source
            //   module3 references module1 and module2 and is published with source
            var clientFactory = await RegistryHelper.CreateMockRegistryClientWithPublishedModulesAsync(
                MockFileSystem, [
                    ("br:mockregistry.io/test/module1:v1", "param p1 bool", withSource: true),
                    ("br:mockregistry.io/test/module1:v2", """
                        module m1 'br:mockregistry.io/test/module1:v1' = {
                            name: 'm1'
                            params: {
                                p1: true
                          }
                        }
                        """, withSource: true),
                    ("br:mockregistry.io/test/module1:v3", """
                        module m1v2 'br:mockregistry.io/test/module1:v2' = {
                            name: 'm1v2'
                        }
                        """, withSource: true)
                    ]);

            // Do *not* force restoration of module1:v3 before showing its source (shouldn't normally happen)

            var moduleDispatcher = CreateModuleDispatcher(clientFactory);

            // Get a URI for displaying module1:v3's source
            var module2Uri = GetDocumentIdForExternalModuleSource(moduleDispatcher, "mockregistry.io/test/module1:v3");

            // ACT: Get all nested links
            var links = (await GetAndResolveLinksForDisplayedDocument(moduleDispatcher, module2Uri)).Unwrap();
            links.Should().BeEmpty("Getting links for an unrestored module should just return empty (not a normal scenario)");
        }

        //asdfg what if can't restore?
        [TestMethod]
        public async Task NestedExternalSource_LinkToModuleNotYetRestored_ShouldAutomaticallyRestore()
        {
            // Setup:
            //   module1 is published with source
            //   module2 references module1 and is published with source
            //   module3 references module1 and module2 and is published with source
            var clientFactory = await RegistryHelper.CreateMockRegistryClientWithPublishedModulesAsync(
                MockFileSystem, [
                    ("br:mockregistry.io/test/module1:v1", "param p1 bool", withSource: true),
                    ("br:mockregistry.io/test/module1:v2", """
                        module m1 'br:mockregistry.io/test/module1:v1' = {
                            name: 'm1'
                            params: {
                                p1: true
                          }
                        }
                        """, withSource: true),
                    ("br:mockregistry.io/test/module1:v3", """
                        module m1v2 'br:mockregistry.io/test/module1:v2' = {
                            name: 'm1v2'
                        }
                        """, withSource: true)
                    ]);
            GetCachedModules().Should().BeEquivalentTo([
                ("mockregistry.io", "test/module1", "v1"),
                ("mockregistry.io", "test/module1", "v2"),
            ]);

            // Get rid of restoration of module1:v1 and :v2
            ResetMockFileSystem();

            // Compile some code to force restoration of module1:v3 (which should always be the case if we're displaying its source)
            var moduleDispatcher = CreateModuleDispatcher(clientFactory);
            await CompilationHelper.RestoreAndCompile(
                GetServices(clientFactory),
                ("main.bicep", """
                    module m1v3 'br:mockregistry.io/test/module1:v3' = {
                        name: 'm1v3'
                    }
                    """));
            GetCachedModules().Should().BeEquivalentTo([
                ("mockregistry.io", "test/module1", "v3"),
            ]);

            // Get a URI for displaying module1:v3's source
            var module2Uri = GetDocumentIdForExternalModuleSource(moduleDispatcher, "mockregistry.io/test/module1:v3");

            // ACT: Get all nested links for module1:v3 - this should force a restoration of module1:v2 and hence should succeed
            var links = (await GetAndResolveLinksForDisplayedDocument(moduleDispatcher, module2Uri)).Unwrap();
            var link = links.Should().HaveCount(1).And.Subject.First();
            link.Target.Should().NotBeNull();
            var target1 = new ExternalSourceReference(link.Target!);
            target1.ToArtifactReference().Unwrap().FullyQualifiedReference.Should().Be("br:mockregistry.io/test/module1:v2");

            // Should have restored module1:v2
            GetCachedModules().Should().BeEquivalentTo([
                ("mockregistry.io", "test/module1", "v2"),
                ("mockregistry.io", "test/module1", "v3"),
            ]);
        }

        [TestMethod]
        public async Task Asdfg()
        {
            // Setup: 
            //   module1 is published with source
            //   module2 references module1 and is published with source
            var clientFactory = await RegistryHelper.CreateMockRegistryClientWithPublishedModulesAsync(
                MockFileSystem, [
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

            // Get a URI for displaying module2
            var module2Uri = GetDocumentIdForExternalModuleSource(moduleDispatcher, "mockregistry.io/test/module2:v2");

            // ACT: Get all nested links inside module2
            var links = (await GetAndResolveLinksForDisplayedDocument(moduleDispatcher, module2Uri)).Unwrap();

            var link = links.Should().HaveCount(1).And.Subject.First();
            link.Range.Should().HaveRange((0, 10), (0, 46));
            link.Target.Should().NotBeNull();
            var target = new ExternalSourceReference(link.Target!);
            target.FullTitle.Should().Be("br:mockregistry.io/test/module1:v1/main.bicep (module1:v1)");
            target.RequestedFile.Should().Be("main.bicep");
            target.ToArtifactReference().Unwrap().FullyQualifiedReference.Should().Be("br:mockregistry.io/test/module1:v1");
        }


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
