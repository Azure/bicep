// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions.TestingHelpers;
using Bicep.Core;
using Bicep.Core.Configuration;
using Bicep.Core.Features;
using Bicep.Core.Registry;
using Bicep.Core.SourceGraph;
using Bicep.Core.SourceLink;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.Registry;
using Bicep.Core.UnitTests.Utils;
using Bicep.IO.Abstraction;
using Bicep.IO.FileSystem;
using Bicep.LanguageServer.Compilation;
using Bicep.LanguageServer.Features.Custom.Visualization;
using Bicep.LanguageServer.Features.Custom.Visualization.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OmniSharp.Extensions.LanguageServer.Protocol;
using static Bicep.Core.UnitTests.Utils.RegistryHelper;

namespace Bicep.LangServer.UnitTests.Features.Visualization;

[TestClass]
public class VisualGraphRemoteModuleTests
{
    private const string Registry = "mockregistry.io";

    private readonly MockFileSystem fileSystem = new();

    private readonly IDirectoryHandle cacheRootDirectory;

    [NotNull]
    public TestContext? TestContext { get; set; }

    public VisualGraphRemoteModuleTests()
    {
        var fileExplorer = new FileSystemFileExplorer(this.fileSystem);
        var configurationManager = new BicepConfigurationManager(fileExplorer, BicepTestConstants.TestCloudConfigurationTrustPolicy);
        var featureProviderFactory = new FeatureProviderFactory(configurationManager, fileExplorer);

        this.cacheRootDirectory = featureProviderFactory.GetFeatureProvider(new IOUri("file", "", "/dummy.bicep")).CacheRootDirectory;
    }

    [TestMethod]
    public async Task RemoteModuleWithSource_ExpandsLikeALocalModule()
    {
        var services = await PublishModules();
        var (context, entryUri) = await CompileAndRestore(services, """
            module storage 'br:mockregistry.io/test/storage:v1' = {
              name: 'storage'
            }
            """);

        var result = VisualGraphBuilder.BuildWithSources(context, entryUri, CreateProvider(services));
        var nodes = result.Graph.Nodes.ToDictionary(node => node.Id);

        nodes["storage"].Should().Match<GraphNode>(node => node.HasChildren && !node.RequiresRestore);
        nodes["storage::nsg"].Should().Match<GraphNode>(node =>
            node.Kind == GraphNodeKind.Resource &&
            node.ParentId == "storage" &&
            node.Type == "Microsoft.Network/networkSecurityGroups" &&
            !node.HasError);
        nodes["storage::inner"].Should().Match<GraphNode>(node => node.Kind == GraphNodeKind.Module && node.HasChildren);
        nodes["storage::inner::child"].ParentId.Should().Be("storage::inner");
        nodes["storage::copies"].IsCollection.Should().BeTrue();
        result.Graph.Edges.Select(edge => edge.Id).Should().Contain("storage::inner->storage::nsg");

        // Children open the module's published source rather than a file on disk.
        result.Sources["storage::nsg"].Uri.Should().StartWith("bicep-extsrc:").And.EndWith("#main.bicep");
        result.Sources["storage::inner::child"].Uri.Should().StartWith("bicep-extsrc:").And.EndWith($"#{Uri.EscapeDataString("modules/inner.bicep")}");
        result.Sources["storage::inner::child"].Range.Start.Line.Should().Be(1);
        result.Sources["storage"].Uri.Should().StartWith("file:");
    }

    [TestMethod]
    public async Task RemoteModule_WithoutSourceProvider_IsALeaf()
    {
        var services = await PublishModules();
        var (context, entryUri) = await CompileAndRestore(services, """
            module storage 'br:mockregistry.io/test/storage:v1' = {
              name: 'storage'
            }
            """);

        var graph = VisualGraphBuilder.Build(context, entryUri);

        graph.Nodes.Should().ContainSingle().Which.Should().Match<GraphNode>(node => !node.HasChildren && !node.RequiresRestore);
    }

    [TestMethod]
    public async Task NestedRegistryModuleNotInCache_RequiresRestoreAndRestoresOnRequest()
    {
        var services = await PublishModules();
        var (context, entryUri) = await CompileAndRestore(services, """
            module storage 'br:mockregistry.io/test/storage:v1' = {
              name: 'storage'
            }
            """);
        var serviceProvider = services.Build();
        var provider = CreateProvider(serviceProvider);

        var before = VisualGraphBuilder.BuildWithSources(context, entryUri, provider);

        before.Graph.Nodes.Should().ContainSingle(node => node.Id == "storage::leaf").Which.Should().Match<GraphNode>(node =>
            !node.HasChildren && node.RequiresRestore);
        before.ModuleReferences["storage::leaf"].FullyQualifiedReference.Should().Be($"br:{Registry}/test/leaf:v1");

        var manager = new Mock<ICompilationManager>(MockBehavior.Strict);
        manager.Setup(x => x.GetCompilation(It.IsAny<DocumentUri>())).Returns(context);
        var handler = new VisualGraphRestoreModuleSourceHandler(
            NullLogger<VisualGraphRestoreModuleSourceHandler>.Instance,
            manager.Object,
            provider,
            serviceProvider.Construct<IModuleDispatcher>());

        var response = await handler.Handle(new(new() { Uri = entryUri.ToUri() }, "storage::leaf"), CancellationToken.None);

        // The document does not reference the nested module, so its compilation is not refreshed (the strict mock
        // would throw).
        response.Should().Be(new VisualGraphRestoreModuleSourceResult(VisualGraphRestoreModuleSourceStatus.Restored));

        var after = VisualGraphBuilder.BuildWithSources(context, entryUri, provider);
        var nodes = after.Graph.Nodes.ToDictionary(node => node.Id);

        nodes["storage::leaf"].Should().Match<GraphNode>(node => node.HasChildren && !node.RequiresRestore);
        nodes["storage::leaf::thing"].ParentId.Should().Be("storage::leaf");
        after.Sources["storage::leaf::thing"].Uri.Should().Contain(Uri.EscapeDataString($"br:{Registry}/test/leaf:v1"));
    }

    [TestMethod]
    public async Task RestoreRequest_ForNodeThatIsNotARegistryModule_ReportsNotFound()
    {
        var services = await PublishModules();
        var (context, entryUri) = await CompileAndRestore(services, """
            module storage 'br:mockregistry.io/test/storage:v1' = {
              name: 'storage'
            }
            """);
        var serviceProvider = services.Build();
        var manager = new Mock<ICompilationManager>(MockBehavior.Strict);
        manager.Setup(x => x.GetCompilation(It.IsAny<DocumentUri>())).Returns(context);
        var handler = new VisualGraphRestoreModuleSourceHandler(
            NullLogger<VisualGraphRestoreModuleSourceHandler>.Instance,
            manager.Object,
            CreateProvider(serviceProvider),
            serviceProvider.Construct<IModuleDispatcher>());

        var response = await handler.Handle(new(new() { Uri = entryUri.ToUri() }, "storage::nsg"), CancellationToken.None);

        response.Status.Should().Be(VisualGraphRestoreModuleSourceStatus.NotFound);
    }

    [TestMethod]
    public async Task RestoreRequest_ForModuleTheRegistryDoesNotHave_ReportsTheFailure()
    {
        var services = GetServices(CreateClientFactory());
        var result = CompilationHelper.Compile(services, ("main.bicep", """
            module missing 'br:mockregistry.io/test/missing:v1' = {
              name: 'missing'
            }
            """));
        var context = new CompilationContext(result.Compilation);
        var serviceProvider = services.Build();
        var manager = new Mock<ICompilationManager>(MockBehavior.Strict);
        manager.Setup(x => x.GetCompilation(It.IsAny<DocumentUri>())).Returns(context);
        var handler = new VisualGraphRestoreModuleSourceHandler(
            NullLogger<VisualGraphRestoreModuleSourceHandler>.Instance,
            manager.Object,
            CreateProvider(serviceProvider),
            serviceProvider.Construct<IModuleDispatcher>());

        var response = await handler.Handle(new(new() { Uri = result.SourceFile.FileHandle.Uri.ToUri() }, "missing"), CancellationToken.None);

        // A failed restore does not refresh the compilation (the strict mock would throw).
        response.Status.Should().Be(VisualGraphRestoreModuleSourceStatus.Failed);
        response.Message.Should().Contain("br:mockregistry.io/test/missing:v1");
    }

    [TestMethod]
    public async Task RemoteModulePublishedWithoutSource_IsALeafThatCannotBeRestored()
    {
        var services = await PublishModules();
        var (context, entryUri) = await CompileAndRestore(services, """
            module plain 'br:mockregistry.io/test/plain:v1' = {
              name: 'plain'
            }
            """);

        var graph = VisualGraphBuilder.Build(context, entryUri, CreateProvider(services));

        graph.Nodes.Should().ContainSingle().Which.Should().Match<GraphNode>(node =>
            !node.HasChildren && !node.RequiresRestore);
    }

    [TestMethod]
    public async Task RemoteModuleNotRestored_RequiresRestore()
    {
        var services = GetServices(CreateClientFactory());
        var result = CompilationHelper.Compile(services, ("main.bicep", """
            module missing 'br:mockregistry.io/test/missing:v1' = {
              name: 'missing'
            }
            """));

        var graph = VisualGraphBuilder.Build(new CompilationContext(result.Compilation), result.SourceFile.FileHandle.Uri, CreateProvider(services));

        graph.Nodes.Should().ContainSingle().Which.RequiresRestore.Should().BeTrue();
    }

    [TestMethod]
    public void TemplateSpecModule_IsALeafThatCannotBeRestored()
    {
        var result = CompilationHelper.Compile("""
            module spec 'ts:00000000-0000-0000-0000-000000000000/rg/spec:v1' = {
              name: 'spec'
            }
            """);

        var graph = VisualGraphBuilder.Build(
            new CompilationContext(result.Compilation),
            result.SourceFile.FileHandle.Uri,
            Mock.Of<IVisualModuleSourceProvider>(MockBehavior.Strict));

        graph.Nodes.Should().ContainSingle().Which.Should().Match<GraphNode>(node => !node.HasChildren && !node.RequiresRestore);
    }

    private static IContainerRegistryClientFactory CreateClientFactory() =>
        CreateMockRegistryClients(
            new FakeContainerRegistryClient(),
            new RepoDescriptor(Registry, "test/leaf", ["v1"]),
            new RepoDescriptor(Registry, "test/storage", ["v1"]),
            new RepoDescriptor(Registry, "test/plain", ["v1"]));

    private ServiceBuilder GetServices(IContainerRegistryClientFactory clientFactory) =>
        new ServiceBuilder()
            .WithContainerRegistryClientFactory(clientFactory)
            .WithFileSystem(this.fileSystem)
            .WithFeatureProviderFactory(BicepTestConstants.CreateFeatureProviderFactory(new FeatureProviderOverrides(CacheRootDirectory: this.cacheRootDirectory)))
            .WithTemplateSpecRepositoryFactory(BicepTestConstants.TemplateSpecRepositoryFactory);

    private static VisualModuleSourceProvider CreateProvider(ServiceBuilder services) => CreateProvider(services.Build());

    private static VisualModuleSourceProvider CreateProvider(IDependencyHelper services) =>
        new(services.Construct<ISourceFileFactory>(), services.Construct<BicepCompiler>());

    /// <summary>
    /// Publishes <c>storage</c> (with source: a resource, a looped resource, a local module, and a nested registry
    /// module), its nested <c>leaf</c> (with source), and <c>plain</c> (without source). Then empties the module
    /// cache, so only what a test restores is cached.
    /// </summary>
    private async Task<ServiceBuilder> PublishModules()
    {
        var clientFactory = CreateClientFactory();
        var services = GetServices(clientFactory);

        await PublishModuleToRegistryAsync(new ServiceBuilder(), clientFactory, this.fileSystem, new(
            $"br:{Registry}/test/leaf:v1",
            """
            resource thing 'Microsoft.Network/networkSecurityGroups@2023-04-01' = {
              name: 'thing'
            }
            """,
            WithSource: true));
        await PublishModuleToRegistryAsync(new ServiceBuilder(), clientFactory, this.fileSystem, new(
            $"br:{Registry}/test/plain:v1",
            """
            resource thing 'Microsoft.Network/networkSecurityGroups@2023-04-01' = {
              name: 'thing'
            }
            """));

        var storage = await CompilationHelper.RestoreAndCompile(
            services,
            ("main.bicep", """
                resource nsg 'Microsoft.Network/networkSecurityGroups@2023-04-01' = {
                  name: 'nsg'
                }

                resource copies 'Microsoft.Network/networkSecurityGroups@2023-04-01' = [for i in range(0, 2): {
                  name: 'copy${i}'
                }]

                module inner 'modules/inner.bicep' = {
                  name: 'inner'
                  params: {
                    nsgId: nsg.id
                  }
                }

                module leaf 'br:mockregistry.io/test/leaf:v1' = {
                  name: 'leaf'
                }
                """),
            ("modules/inner.bicep", """
                param nsgId string
                resource child 'Microsoft.Network/networkSecurityGroups@2023-04-01' = {
                  name: 'child'
                  tags: {
                    nsg: nsgId
                  }
                }
                """));
        storage.Template.Should().NotBeNull();

        var dispatcher = services.Build().Construct<IModuleDispatcher>();
        var storageReference = dispatcher.TryGetArtifactReference(BicepTestConstants.DummyBicepFile, ArtifactType.Module, $"br:{Registry}/test/storage:v1").Unwrap();
        await dispatcher.PublishModule(
            storageReference,
            BinaryData.FromString(storage.Template!.ToString()),
            SourceArchive.CreateFrom(storage.Compilation.SourceFileGrouping).PackIntoBinaryData(),
            documentationUri: null);

        this.cacheRootDirectory.Delete();

        return services;
    }

    private static async Task<(CompilationContext Context, IOUri EntryUri)> CompileAndRestore(ServiceBuilder services, string text)
    {
        var result = await CompilationHelper.RestoreAndCompile(services, ("main.bicep", text));
        result.Should().NotHaveAnyDiagnostics();

        return (new CompilationContext(result.Compilation), result.SourceFile.FileHandle.Uri);
    }
}
