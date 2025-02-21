// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Reflection;
using Bicep.Core.Configuration;
using Bicep.Core.Features;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;
using Bicep.Core.Samples;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Registry;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Utils;
using Bicep.Core.Workspaces;
using Bicep.IO.Abstraction;
using Bicep.IO.FileSystem;
using Bicep.LangServer.IntegrationTests;
using Bicep.LangServer.IntegrationTests.Assertions;
using Bicep.LangServer.UnitTests.Mocks;
using Bicep.LanguageServer.Handlers;
using Bicep.LanguageServer.Telemetry;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using Moq;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using static System.Net.Mime.MediaTypeNames;
using static Bicep.Core.UnitTests.Utils.RegistryHelper;

namespace Bicep.LangServer.UnitTests.Handlers
{
    [TestClass]
    public class BicepExternalSourceDocumentLinkHandlerTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        private readonly MockFileSystem MockFileSystem;

        private readonly IDirectoryHandle CacheRootDirectory;

        public BicepExternalSourceDocumentLinkHandlerTests()
        {
            this.MockFileSystem = new();

            var mockFileExplorer = new FileSystemFileExplorer(this.MockFileSystem);
            var mockConfigurationManager = new ConfigurationManager(mockFileExplorer);
            var featureProviderFactory = new FeatureProviderFactory(mockConfigurationManager, mockFileExplorer);

            this.CacheRootDirectory = featureProviderFactory.GetFeatureProvider(new Uri("file:///no-file")).CacheRootDirectory;
        }

        private void ResetModuleCache()
        {
            this.CacheRootDirectory.Delete();
        }

        private ServiceBuilder GetServices(IContainerRegistryClientFactory clientFactory)
        {
            return new ServiceBuilder()
                .WithFeatureOverrides(new(OptionalModuleNamesEnabled: true))
                .WithContainerRegistryClientFactory(clientFactory)
                .WithFileSystem(MockFileSystem)
                .WithFeatureProviderFactory(
                    BicepTestConstants.CreateFeatureProviderFactory(new FeatureProviderOverrides(CacheRootDirectory: CacheRootDirectory, OptionalModuleNamesEnabled: true))
                )
                .WithTemplateSpecRepositoryFactory(BicepTestConstants.TemplateSpecRepositoryFactory)
                ;
        }

        private (IModuleDispatcher, ISourceFileFactory) CreateModuleDispatcher(IContainerRegistryClientFactory clientFactory)
        {
            var services = GetServices(clientFactory).Build();

            return (services.Construct<IModuleDispatcher>(), services.Construct<ISourceFileFactory>());
        }

        private async Task<(string? message, DocumentLink<ExternalSourceDocumentLinkData>[] links)> GetAndResolveLinksForDisplayedDocument(IModuleDispatcher moduleDispatcher, ISourceFileFactory sourceFileFactory, TextDocumentIdentifier documentId)
        {
            var server = new LanguageServerMock();
            string? message = null;
            server.WindowMock.OnShowMessage(
                p =>
                {
                    message ??= p.Message;
                });

            var links = BicepExternalSourceDocumentLinkHandler.GetDocumentLinks(
                moduleDispatcher,
                sourceFileFactory,
                new DocumentLinkParams() { TextDocument = documentId },
                CancellationToken.None)
            .ToArray();

            message.Should().BeNull($"{nameof(BicepExternalSourceDocumentLinkHandler.GetDocumentLinks)} should never show an error, although it can trace output");

            // Call resolve on each link (normally only happens for the one that the user clicks on)
            var resolvedLinks = new List<DocumentLink<ExternalSourceDocumentLinkData>>();
            foreach (var link in links)
            {
                var telemetryProvider = StrictMock.Of<ITelemetryProvider>();
                telemetryProvider.Setup(x => x.PostEvent(It.IsAny<BicepTelemetryEvent>()));

                var resolvedLink = await BicepExternalSourceDocumentLinkHandler.ResolveDocumentLink(link, moduleDispatcher, sourceFileFactory, server.Mock.Object, telemetryProvider.Object);
                resolvedLinks.Add(resolvedLink);

                telemetryProvider.Verify(m => m.PostEvent(It.Is<BicepTelemetryEvent>(
                    p => (p.EventName == TelemetryConstants.EventNames.ExternalSourceDocLinkClickSuccess
                        || p.EventName == TelemetryConstants.EventNames.ExternalSourceDocLinkClickFailure)
                    && p.Properties != null
                    )), Times.Exactly(1));

                telemetryProvider.VerifyNoOtherCalls();
            }

            return (
                message,
                resolvedLinks.OrderBy(x => x.Range.Start).ToArray()
            );
        }

        /// <param name="displayedModuleTarget">E.g. mockregistry.io/test/module1:v1</param>
        private static TextDocumentIdentifier GetDocumentIdForExternalModuleSource(string displayedModuleTarget)
        {
            var moduleReferenceComponents = OciArtifactAddressComponents.TryParse(displayedModuleTarget).Unwrap();
            var moduleExtRef = new ExternalSourceReference(moduleReferenceComponents, null).WithRequestForSourceFile("main.bicep");
            var moduleId = new TextDocumentIdentifier(moduleExtRef.ToUri());
            return moduleId;
        }

        private (string registry, string repo, string tag)[] GetCachedModules()
        {
            var cachedModules = CachedModules.GetCachedModules(MockFileSystem, CacheRootDirectory);
            return [.. cachedModules.Select(x => (x.Registry, x.Repository, x.Tag))];
        }

        private async Task<IContainerRegistryClientFactory> PublishThreeNestedModules(bool module1WithSource = true, bool module2WithSource = true, bool module3WithSource = true)
        {
            // Setup:
            //   module1 is published with source
            //   module2 references module1 and is published with source
            //   module3 references module1 and module2 and is published with source
            return await RegistryHelper.CreateMockRegistryClientWithPublishedModulesAsync(
                MockFileSystem, [
                    new("br:mockregistry.io/test/module1:v1", "param p1 bool", module1WithSource),
                    new("br:mockregistry.io/test/module1:v2", """
                        module m1 'br:mockregistry.io/test/module1:v1' = {
                            name: 'm1'
                            params: {
                                p1: true
                          }
                        }
                        """, module2WithSource),
                    new("br:mockregistry.io/test/module1:v3", """
                        module m1v2 'br:mockregistry.io/test/module1:v2' = {
                            name: 'm1v2'
                        }
                        """, module3WithSource)
                    ]);
        }

        private async Task RestoreModuleViaLocalCode(IContainerRegistryClientFactory clientFactory, string moduleName, string tag)
        {
            // Compile some code to force restoration of module1:v3 (which should always be the case if we're displaying its source)
            var result = await CompilationHelper.RestoreAndCompile(
                GetServices(clientFactory),
                ("main.bicep", $$"""
                    module m 'br:mockregistry.io/test/{{moduleName}}:{{tag}}' = {
                        name: 'm'
                    }
                    """));
            result.Should().NotHaveAnyDiagnostics();
        }

        [TestMethod]
        public async Task IfNotShowingExternalModuleSourceCode_ThenReturnNoLinks()
        {
            var clientFactory = await RegistryHelper.CreateMockRegistryClientWithPublishedModulesAsync(
                MockFileSystem, [
                    new("br:mockregistry.io/test/module1:v1", "metadata m = ''", WithSource: true)
                ]);
            var (moduleDispatcher, sourceFileFactory) = CreateModuleDispatcher(clientFactory);
            await RestoreModuleViaLocalCode(clientFactory, "module1", "v1");

            var (msg, links) = await GetAndResolveLinksForDisplayedDocument(moduleDispatcher, sourceFileFactory, new TextDocumentIdentifier("file:///main.bicep"));

            links.Should().HaveCount(0);
            msg.Should().BeNull();
        }

        [TestMethod]
        public async Task NestedExternalSource_ShouldGetCorrectLink()
        {
            // Setup:
            //   module1 is published with source
            //   module2 references module1 and is published with source
            var clientFactory = await RegistryHelper.CreateMockRegistryClientWithPublishedModulesAsync(
                MockFileSystem, [
                    new("br:mockregistry.io/test/module1:v1", "param p1 bool", WithSource: true),
                    new("br:mockregistry.io/test/module2:v2", """
                        module m1 'br:mockregistry.io/test/module1:v1' = {
                            name: 'm1'
                            params: {
                                p1: true
                          }
                        }
                        """, WithSource: true)
                    ]);

            // Compile some code to force restoration of module2 (which should always be the case if we're displaying its source)
            var (moduleDispatcher, sourceFileFactory) = CreateModuleDispatcher(clientFactory);
            await RestoreModuleViaLocalCode(clientFactory, "module2", "v2");

            // Get a URI for displaying module2's source
            var module2Uri = GetDocumentIdForExternalModuleSource("mockregistry.io/test/module2:v2");

            // ACT: Get all links inside module2's source display
            var (msg, links) = await GetAndResolveLinksForDisplayedDocument(moduleDispatcher, sourceFileFactory, module2Uri);

            msg.Should().BeNull();
            var link = links.Should().HaveCount(1).And.Subject.First();
            link.Range.Should().HaveRange((0, 10), (0, 46));
            link.Target.Should().NotBeNull();
            var target = new ExternalSourceReference(link.Target!);
            target.FullTitle.Should().Be("br:mockregistry.io/test/module1:v1 -> main.bicep");
            target.RequestedFile.Should().Be("main.bicep");
            target.ToArtifactReference(BicepTestConstants.DummyBicepFile).Unwrap().FullyQualifiedReference.Should().Be("br:mockregistry.io/test/module1:v1");
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
                    new("br:mockregistry.io/test/module1:v1", "param p1 bool", WithSource: true),
                    new("br:mockregistry.io/test/module1:v2", """
                        module m1 'br:mockregistry.io/test/module1:v1' = {
                            name: 'm1'
                            params: {
                                p1: true
                          }
                        }
                        """, WithSource: true),
                    new("br:mockregistry.io/test/module1:v3", """
                        module m1 'br:mockregistry.io/test/module1:v1' = {
                            name: 'm1'
                            params: {
                                p1: true
                          }
                        }
                        module m1v2 'br:mockregistry.io/test/module1:v2' = {
                            name: 'm1v2'
                        }
                        """, WithSource: true)
                    ]);

            // Compile some code to force restoration of module1:v3 (which should always be the case if we're displaying its source)
            var (moduleDispatcher, sourceFileFactory) = CreateModuleDispatcher(clientFactory);
            await RestoreModuleViaLocalCode(clientFactory, "module1", "v3");

            // Get a URI for displaying module1:v3's source
            var module2Uri = GetDocumentIdForExternalModuleSource("mockregistry.io/test/module1:v3");

            // ACT: Get all nested links (one for m1:v1 and one for m1:v2)
            var (msg, links) = await GetAndResolveLinksForDisplayedDocument(moduleDispatcher, sourceFileFactory, module2Uri);

            msg.Should().BeNull();
            var link1 = links.Should().HaveCount(2).And.Subject.First();
            link1.Range.Should().HaveRange((0, 10), (0, 46));
            link1.Target.Should().NotBeNull();
            var target1 = new ExternalSourceReference(link1.Target!);
            target1.FullTitle.Should().Be("br:mockregistry.io/test/module1:v1 -> main.bicep");
            target1.RequestedFile.Should().Be("main.bicep");
            target1.ToArtifactReference(BicepTestConstants.DummyBicepFile).Unwrap().FullyQualifiedReference.Should().Be("br:mockregistry.io/test/module1:v1");

            var link2 = links.Skip(1).First();
            link2.Range.Should().HaveRange((6, 12), (6, 48));
            link2.Target.Should().NotBeNull();
            var target2 = new ExternalSourceReference(link2.Target!);
            target2.FullTitle.Should().Be("br:mockregistry.io/test/module1:v2 -> main.bicep");
            target2.RequestedFile.Should().Be("main.bicep");
            target2.ToArtifactReference(BicepTestConstants.DummyBicepFile).Unwrap().FullyQualifiedReference.Should().Be("br:mockregistry.io/test/module1:v2");

            // ACT: Follow that link and get all nested links inside module1:v2
            var (msg3, links3) = await GetAndResolveLinksForDisplayedDocument(moduleDispatcher, sourceFileFactory, new TextDocumentIdentifier(target2.ToUri()));

            msg3.Should().BeNull();
            var link3 = links3.Should().HaveCount(1).And.Subject.First();
            link3.Target.Should().NotBeNull();
            var target3 = new ExternalSourceReference(link3.Target!);
            target3.ToArtifactReference(BicepTestConstants.DummyBicepFile).Unwrap().FullyQualifiedReference.Should().Be("br:mockregistry.io/test/module1:v1");

            // Should have restored module1:v1
            GetCachedModules().Should().BeEquivalentTo([
                ("mockregistry.io", "test/module1", "v1"),
                ("mockregistry.io", "test/module1", "v2"),
                ("mockregistry.io", "test/module1", "v3"),
            ]);
        }

        [TestMethod]
        public async Task NestedExternalSource_ExternalModuleSourceBeingDisplayedNotYetRestored_ShowReturnNoLinks()
        {
            // Setup:
            //   module1 is published with source
            //   module2 references module1 and is published with source
            //   module3 references module1 and module2 and is published with source
            var clientFactory = await RegistryHelper.CreateMockRegistryClientWithPublishedModulesAsync(
                MockFileSystem, [
                    new("br:mockregistry.io/test/module1:v1", "param p1 bool", WithSource: true),
                    new("br:mockregistry.io/test/module1:v2", """
                        module m1 'br:mockregistry.io/test/module1:v1' = {
                            name: 'm1'
                            params: {
                                p1: true
                          }
                        }
                        """, WithSource: true),
                    new("br:mockregistry.io/test/module1:v3", """
                        module m1v2 'br:mockregistry.io/test/module1:v2' = {
                            name: 'm1v2'
                        }
                        """, WithSource: true)
                    ]);

            // Do *not* force restoration of module1:v3 before showing its source (shouldn't normally happen)

            var (moduleDispatcher, sourceFileFactory) = CreateModuleDispatcher(clientFactory);

            // Get a URI for displaying module1:v3's source
            var module2Uri = GetDocumentIdForExternalModuleSource("mockregistry.io/test/module1:v3");

            // ACT: Get all nested links
            var (msg, links) = await GetAndResolveLinksForDisplayedDocument(moduleDispatcher, sourceFileFactory, module2Uri);
            msg.Should().BeNull();
            links.Should().BeEmpty("Getting links for an unrestored module should just return empty (not a normal scenario)");
        }

        [TestMethod]
        public async Task NestedExternalSource_LinkToModuleNotYetRestored_ShouldAutomaticallyRestore()
        {
            var clientFactory = await PublishThreeNestedModules();

            GetCachedModules().Should().BeEquivalentTo([
                ("mockregistry.io", "test/module1", "v1"),
                ("mockregistry.io", "test/module1", "v2"),
            ]);

            // Get rid of module cache
            ResetModuleCache();

            // Compile some code to force restoration of module1:v3 (which should always be the case if we're displaying its source)
            var (moduleDispatcher, sourceFileFactory) = CreateModuleDispatcher(clientFactory);
            await RestoreModuleViaLocalCode(clientFactory, "module1", "v3");

            GetCachedModules().Should().BeEquivalentTo([
                ("mockregistry.io", "test/module1", "v3"),
            ]);

            // Get a URI for displaying module1:v3's source
            var module2Uri = GetDocumentIdForExternalModuleSource("mockregistry.io/test/module1:v3");

            // ACT: Get all nested links for module1:v3 - this should force a restoration of module1:v2 and hence should succeed
            var (msg, links) = await GetAndResolveLinksForDisplayedDocument(moduleDispatcher, sourceFileFactory, module2Uri);

            msg.Should().BeNull();
            var link = links.Should().HaveCount(1).And.Subject.First();
            link.Target.Should().NotBeNull();
            var target1 = new ExternalSourceReference(link.Target!);
            target1.ToArtifactReference(BicepTestConstants.DummyBicepFile).Unwrap().FullyQualifiedReference.Should().Be("br:mockregistry.io/test/module1:v2");

            // Should have restored module1:v2
            GetCachedModules().Should().BeEquivalentTo([
                ("mockregistry.io", "test/module1", "v2"),
                ("mockregistry.io", "test/module1", "v3"),
            ]);
        }

        [TestMethod]
        public async Task NestedExternalSource_LinkToModuleNotYetRestored_AndRestoreFails_ShouldShowMsg_AndReturnLinkToJsonEmbeddedInModule3()
        {
            IContainerRegistryClientFactory clientFactory = await PublishThreeNestedModules();

            // Get rid of module cache
            ResetModuleCache();

            // Restore module1:v3
            await RestoreModuleViaLocalCode(clientFactory, "module1", "v3");

            // Get a URI for displaying module1:v3's source
            var module3Uri = GetDocumentIdForExternalModuleSource("mockregistry.io/test/module1:v3");

            // Unregister module1:v2 so that it can't be restored
            clientFactory = RegistryHelper.CreateMockRegistryClient(new RepoDescriptor("mockregistry.io", "test/module1", ["tag"]));

            // Compile some code to force restoration of module1:v3 (which should always be the case if we're displaying its source)
            var (moduleDispatcher, sourceFileFactory) = CreateModuleDispatcher(clientFactory);
            await RestoreModuleViaLocalCode(clientFactory, "module1", "v3");

            // ACT: Get all nested links for module1:v3 - this should force a restoration of module1:v2
            var (msg, links) = await GetAndResolveLinksForDisplayedDocument(moduleDispatcher, sourceFileFactory, module3Uri);

            msg.Should().Be("Unable to restore module br:mockregistry.io/test/module1:v2: Unable to restore the artifact with reference \"br:mockregistry.io/test/module1:v2\": The artifact does not exist in the registry.");

            // Target should be to the compiled JSON of module2 embedded inside module3's source
            links.Should().HaveCount(1);
            var target = new ExternalSourceReference(links[0].Target!);
            target.RequestedFile.Should().Be("<cache>/br/mockregistry.io/test$module1/v2$/main.json");
            target.ToArtifactReference(BicepTestConstants.DummyBicepFile).Unwrap().FullyQualifiedReference.Should().Be("br:mockregistry.io/test/module1:v3");
        }

        [TestMethod]
        public async Task NestedExternalSource_LinkToModuleNotYetRestored_AndPublishedNoLongerHasSource_ShouldShowMsg_AndReturnLinkToJsonEmbeddedInModule3()
        {
            IContainerRegistryClientFactory clientFactory = await PublishThreeNestedModules();

            // Restore module1:v3
            await RestoreModuleViaLocalCode(clientFactory, "module1", "v3");

            // Get a URI for displaying module1:v3's source
            var module3Uri = GetDocumentIdForExternalModuleSource("mockregistry.io/test/module1:v3");

            // Re-register module1:v2 without source
            clientFactory = await PublishThreeNestedModules(module2WithSource: false);

            // Get rid of module cache
            ResetModuleCache();

            // Compile some code to force restoration of module1:v3 (which should always be the case if we're displaying its source)
            await RestoreModuleViaLocalCode(clientFactory, "module1", "v3");

            // ACT: Get all nested links for module1:v3 - this should force a restoration of module1:v2
            var (moduleDispatcher, sourceFileFactory) = CreateModuleDispatcher(clientFactory);
            var (msg, links) = await GetAndResolveLinksForDisplayedDocument(moduleDispatcher, sourceFileFactory, module3Uri);

            msg.Should().Be("Unable to retrieve source code for module br:mockregistry.io/test/module1:v2. No source code is available for this module");

            // Target should be to the compiled JSON of module2 embedded inside module3's source
            links.Should().HaveCount(1);
            var target = new ExternalSourceReference(links[0].Target!);
            target.RequestedFile.Should().Be("<cache>/br/mockregistry.io/test$module1/v2$/main.json");
            target.ToArtifactReference(BicepTestConstants.DummyBicepFile).Unwrap().FullyQualifiedReference.Should().Be("br:mockregistry.io/test/module1:v3");
        }
    }
}
