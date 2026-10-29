// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions.TestingHelpers;
using Bicep.Core.Configuration;
using Bicep.Core.Features;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;
using Bicep.Core.SourceLink;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.Registry;
using Bicep.Core.UnitTests.Utils;
using Bicep.IO.Abstraction;
using Bicep.IO.FileSystem;
using Bicep.LanguageServer.Compilation;
using Bicep.LanguageServer.Extensions;
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

    public VisualGraphRemoteModuleTests()
    {
        var fileExplorer = new FileSystemFileExplorer(this.fileSystem);
        var configurationManager = new BicepConfigurationManager(fileExplorer, BicepTestConstants.TestCloudConfigurationTrustPolicy);
        var featureProviderFactory = new FeatureProviderFactory(configurationManager, fileExplorer);

        this.cacheRootDirectory = featureProviderFactory.GetFeatureProvider(new IOUri("file", "", "/dummy.bicep")).CacheRootDirectory;
    }

    [TestMethod]
    public async Task RegistryModuleWithSource_ExpandsIntoItsPublishedSource()
    {
        var services = await this.PublishModules();
        var context = await CompileAndRestore(services, "storage");

        var builder = VisualGraphBuilderTests.CreateBuilder(services.Build());
        var (graph, sources) = builder.Build(context);
        var nodes = graph.Nodes.ToDictionary(node => node.Id);

        nodes["storage"].Should().Match<GraphNode>(node => node.HasChildren && node.SourceStatus == null);
        nodes["storage::nsg"].Should().Match<GraphNode>(node =>
            node.Kind == GraphNodeKind.Resource &&
            node.ParentId == "storage" &&
            node.Type == "Microsoft.Network/networkSecurityGroups" &&
            !node.HasError);
        nodes["storage::copies"].IsCollection.Should().BeTrue();
        nodes["storage::inner"].Should().Match<GraphNode>(node => node.Kind == GraphNodeKind.Module && node.HasChildren && node.IsCollection);
        nodes["storage::inner::child"].ParentId.Should().Be("storage::inner");
        graph.Edges.Select(edge => edge.Id).Should().Contain("storage::inner->storage::nsg");

        // Its nodes reveal the published source rather than a file on disk.
        sources["storage"].Uri.Should().StartWith("file:");
        sources["storage::nsg"].Uri.Should().StartWith("bicep-extsrc:").And.EndWith("#main.bicep");
        sources["storage::inner::child"].Uri.Should().StartWith("bicep-extsrc:").And.EndWith($"#{Uri.EscapeDataString("modules/inner.bicep")}");
        sources["storage::inner::child"].Range.Start.Line.Should().Be(1);

        // The nested registry module is not cached. Building the graph does not restore it; expanding it can.
        nodes["storage::leaf"].Should().Match<GraphNode>(node => !node.HasChildren && node.SourceStatus == GraphNodeSourceStatus.Unrestored);
        var restorableReferences = GetRestorableReferences(builder, context, graph);
        restorableReferences.Should().ContainSingle().Which.Should().Match<KeyValuePair<string, OciArtifactReference>>(pair =>
            pair.Key == "storage::leaf" && pair.Value.FullyQualifiedReference == $"br:{Registry}/test/leaf:v1");
        services.Build().Construct<IModuleDispatcher>().GetArtifactRestoreStatus(restorableReferences["storage::leaf"], out _)
            .Should().Be(ArtifactRestoreStatus.Unknown);
    }

    [TestMethod]
    public async Task RestoreHandler_RestoresANestedModuleWithoutRecompilingTheDocument()
    {
        var services = await this.PublishModules();
        var context = await CompileAndRestore(services, "storage");
        var provider = services.Build();
        var builder = VisualGraphBuilderTests.CreateBuilder(provider);
        var before = builder.Build(context);

        var response = await RestoreAsync(provider, builder, context, "storage::leaf");
        var after = builder.Build(context);
        var nodes = after.Graph.Nodes.ToDictionary(node => node.Id);

        response.Should().Be(new VisualGraphRestoreModuleResult(Restored: true));
        nodes["storage::leaf"].Should().Match<GraphNode>(node => node.HasChildren && node.SourceStatus == null);
        nodes["storage::leaf::thing"].ParentId.Should().Be("storage::leaf");
        after.Sources["storage::leaf::thing"].Uri.Should().Contain(Uri.EscapeDataString($"br:{Registry}/test/leaf:v1"));
        GetRestorableReferences(builder, context, after.Graph).Should().BeEmpty();
        after.Graph.GetRevision().Should().NotBe(before.Graph.GetRevision());
    }

    [TestMethod]
    public async Task RestoreHandler_IgnoresANodeThatDoesNotRequireRestore()
    {
        var services = await this.PublishModules();
        var context = await CompileAndRestore(services, "storage");
        var provider = services.Build();
        var builder = VisualGraphBuilderTests.CreateBuilder(provider);

        (await RestoreAsync(provider, builder, context, "storage::nsg")).Restored.Should().BeFalse();
        (await RestoreAsync(provider, builder, context, "storage::missing")).Restored.Should().BeFalse();
        // A path through a module that is not expanded, or through a resource, leads nowhere.
        (await RestoreAsync(provider, builder, context, "storage::leaf::thing")).Restored.Should().BeFalse();
        (await RestoreAsync(provider, builder, context, "storage::nsg::leaf")).Restored.Should().BeFalse();
        (await RestoreAsync(provider, builder, context, "")).Restored.Should().BeFalse();
        builder.Build(context).Graph.Nodes.Should().Contain(node => node.Id == "storage::leaf" && node.SourceStatus == GraphNodeSourceStatus.Unrestored);
    }

    [TestMethod]
    public async Task RegistryModuleThatFailsToRestore_ReportsTheErrorAndCanBeRetried()
    {
        var clientFactory = CreateClientFactory();
        var services = GetServices(clientFactory);
        var result = CompilationHelper.Compile(services, ("main.bicep", """
            module late 'br:mockregistry.io/test/late:v1' = {
              name: 'late'
            }
            """));
        var context = new CompilationContext(result.Compilation);
        var provider = services.Build();
        var builder = VisualGraphBuilderTests.CreateBuilder(provider);

        builder.Build(context).Graph.Nodes.Should().ContainSingle().Which.SourceStatus.Should().Be(GraphNodeSourceStatus.Unrestored);

        // The module is not published yet, so restoring it fails.
        (await RestoreAsync(provider, builder, context, "late")).Restored.Should().BeFalse();
        var failed = builder.Build(context);

        failed.Graph.Nodes.Should().ContainSingle().Which.Should().Match<GraphNode>(node =>
            node.SourceStatus == GraphNodeSourceStatus.Error && node.SourceError!.Contains($"br:{Registry}/test/late:v1"));
        GetRestorableReferences(builder, context, failed.Graph).Keys.Should().Equal("late");

        // Once it is published, retrying succeeds even though the failure was recent.
        await PublishModuleToRegistryAsync(new ServiceBuilder(), clientFactory, this.fileSystem, new($"br:{Registry}/test/late:v1", """
            resource thing 'Microsoft.Network/networkSecurityGroups@2023-04-01' = {
              name: 'thing'
            }
            """, WithSource: true));
        (await RestoreAsync(provider, builder, context, "late")).Restored.Should().BeTrue();
        var restored = builder.Build(context);

        restored.Graph.Nodes.Should().Contain(node => node.Id == "late" && node.HasChildren && node.SourceStatus == null && node.SourceError == null);
        restored.Graph.Nodes.Should().Contain(node => node.Id == "late::thing");
        GetRestorableReferences(builder, context, restored.Graph).Should().BeEmpty();
    }

    [TestMethod]
    public async Task RegistryModulePublishedWithoutSource_ExplainsWhy()
    {
        var services = await this.PublishModules();
        var context = await CompileAndRestore(services, "plain");

        var builder = VisualGraphBuilderTests.CreateBuilder(services.Build());
        var result = builder.Build(context);

        result.Graph.Nodes.Should().ContainSingle().Which.Should().Match<GraphNode>(node =>
            !node.HasChildren && node.SourceStatus == GraphNodeSourceStatus.Unavailable && node.SourceError == null);
        GetRestorableReferences(builder, context, result.Graph).Should().BeEmpty();
    }

    [TestMethod]
    public async Task RegistryModuleWithoutDeclarations_IsALeafWithNothingToExplain()
    {
        var services = await this.PublishModules();
        var context = await CompileAndRestore(services, "empty");

        var graph = VisualGraphBuilderTests.CreateBuilder(services.Build()).Build(context).Graph;

        graph.Nodes.Should().ContainSingle().Which.Should().Match<GraphNode>(node => !node.HasChildren && node.SourceStatus == null);
    }

    [TestMethod]
    public void TemplateSpecModule_ExplainsWhy()
    {
        var result = CompilationHelper.Compile("""
            module spec 'ts:00000000-0000-0000-0000-000000000000/rg/spec:v1' = {
              name: 'spec'
            }
            """);

        var context = new CompilationContext(result.Compilation);
        var builder = VisualGraphBuilderTests.CreateBuilder();
        var graph = builder.Build(context).Graph;

        graph.Nodes.Should().ContainSingle().Which.Should().Match<GraphNode>(node =>
            !node.HasChildren && node.SourceStatus == GraphNodeSourceStatus.Unavailable);
        GetRestorableReferences(builder, context, graph).Should().BeEmpty();
    }

    [TestMethod]
    public async Task RegistryModuleThatContainsItself_StopsAtTheRepeat()
    {
        var clientFactory = CreateClientFactory();

        // A tag published again can make two modules contain each other.
        await PublishModuleToRegistryAsync(new ServiceBuilder(), clientFactory, this.fileSystem, new($"br:{Registry}/test/b:v1", "param p string", WithSource: true));
        await PublishModuleToRegistryAsync(new ServiceBuilder(), clientFactory, this.fileSystem, new($"br:{Registry}/test/a:v1", """
            module b 'br:mockregistry.io/test/b:v1' = {
              name: 'b'
              params: {
                p: 'p'
              }
            }
            """, WithSource: true));
        await PublishModuleToRegistryAsync(new ServiceBuilder(), clientFactory, this.fileSystem, new($"br:{Registry}/test/b:v1", """
            param p string
            module a 'br:mockregistry.io/test/a:v1' = {
              name: 'a'
            }
            """, WithSource: true));
        this.cacheRootDirectory.Delete();

        var services = GetServices(clientFactory);
        var context = await CompileAndRestore(services, "a");
        var provider = services.Build();
        var builder = VisualGraphBuilderTests.CreateBuilder(provider);

        await RestoreAsync(provider, builder, context, "a::b");
        var graph = builder.Build(context).Graph;
        var nodes = graph.Nodes.ToDictionary(node => node.Id);

        nodes["a::b"].HasChildren.Should().BeTrue();
        // The repeat is shown once, as a module without children.
        nodes["a::b::a"].Should().Match<GraphNode>(node => !node.HasChildren && node.SourceStatus == null);
        GetRestorableReferences(builder, context, graph).Should().BeEmpty();
    }

    [TestMethod]
    public async Task NodeSourceHandler_RevealsPublishedSource()
    {
        var services = await this.PublishModules();
        var context = await CompileAndRestore(services, "storage");
        var documentUri = GetDocumentUri(context);
        var handler = new VisualGraphNodeSourceHandler(
            NullLogger<VisualGraphNodeSourceHandler>.Instance,
            CreateCompilationManager(context),
            VisualGraphBuilderTests.CreateBuilder(services.Build()));

        var resource = await handler.Handle(new(new() { Uri = documentUri }, "storage::nsg"), CancellationToken.None);
        var missing = await handler.Handle(new(new() { Uri = documentUri }, "storage::missing"), CancellationToken.None);

        resource.Uri.Should().StartWith("bicep-extsrc:").And.Contain(Uri.EscapeDataString($"br:{Registry}/test/storage:v1"));
        resource.Range!.Start.Line.Should().Be(0);
        missing.Should().Be(new VisualGraphNodeSourceResult(Uri: null, Range: null));
    }

    /// <summary>
    /// The registry reference behind each node of <paramref name="graph"/>, looked up by node id as the restore handler
    /// does. Checks that the lookup agrees with the graph: a node has a reference exactly when its status says expanding
    /// it restores its module.
    /// </summary>
    private static Dictionary<string, OciArtifactReference> GetRestorableReferences(VisualGraphBuilder builder, CompilationContext context, CanonicalGraph graph)
    {
        var references = new Dictionary<string, OciArtifactReference>(StringComparer.Ordinal);

        foreach (var node in graph.Nodes)
        {
            var reference = builder.TryGetRestorableReference(context, node.Id);
            var isRestorable = node.SourceStatus is GraphNodeSourceStatus.Unrestored or GraphNodeSourceStatus.Error;

            (reference is not null).Should().Be(isRestorable, $"node {node.Id} has source status {node.SourceStatus ?? "null"}");
            if (reference is not null)
            {
                references[node.Id] = reference;
            }
        }

        return references;
    }

    /// <summary>Restores a node's module as the visualizer does, with a document whose compilation must not be refreshed.</summary>
    private static Task<VisualGraphRestoreModuleResult> RestoreAsync(IDependencyHelper provider, VisualGraphBuilder builder, CompilationContext context, string nodeId)
    {
        var handler = new VisualGraphRestoreModuleHandler(
            NullLogger<VisualGraphRestoreModuleHandler>.Instance,
            CreateCompilationManager(context),
            builder,
            provider.Construct<IModuleDispatcher>());

        return handler.Handle(new(new() { Uri = GetDocumentUri(context) }, nodeId), CancellationToken.None);
    }

    private static DocumentUri GetDocumentUri(CompilationContext context) =>
        context.Compilation.GetEntrypointSemanticModel().SourceFile.FileHandle.Uri.ToDocumentUri();

    /// <summary>A compilation manager that has only <paramref name="context"/>, and never compiles again.</summary>
    private static ICompilationManager CreateCompilationManager(CompilationContext context)
    {
        var manager = new Mock<ICompilationManager>(MockBehavior.Strict);
        manager.Setup(x => x.GetCompilation(GetDocumentUri(context))).Returns(context);

        return manager.Object;
    }

    private static IContainerRegistryClientFactory CreateClientFactory() =>
        CreateMockRegistryClients(
            new FakeContainerRegistryClient(),
            new RepoDescriptor(Registry, "test/a", ["v1"]),
            new RepoDescriptor(Registry, "test/b", ["v1"]),
            new RepoDescriptor(Registry, "test/empty", ["v1"]),
            new RepoDescriptor(Registry, "test/late", ["v1"]),
            new RepoDescriptor(Registry, "test/leaf", ["v1"]),
            new RepoDescriptor(Registry, "test/plain", ["v1"]),
            new RepoDescriptor(Registry, "test/storage", ["v1"]));

    private ServiceBuilder GetServices(IContainerRegistryClientFactory clientFactory) =>
        new ServiceBuilder()
            .WithContainerRegistryClientFactory(clientFactory)
            .WithFileSystem(this.fileSystem)
            .WithFeatureProviderFactory(BicepTestConstants.CreateFeatureProviderFactory(new FeatureProviderOverrides(CacheRootDirectory: this.cacheRootDirectory)))
            .WithTemplateSpecRepositoryFactory(BicepTestConstants.TemplateSpecRepositoryFactory);

    /// <summary>
    /// Publishes <c>storage</c> with source (a resource, a looped resource, a looped local module, and a nested registry
    /// module), its nested <c>leaf</c> with source, <c>plain</c> without source, and <c>empty</c> with source but no
    /// declarations. Then empties the module cache, so only what a test restores is cached.
    /// </summary>
    private async Task<ServiceBuilder> PublishModules()
    {
        var clientFactory = CreateClientFactory();
        var services = GetServices(clientFactory);
        const string Resource = """
            resource thing 'Microsoft.Network/networkSecurityGroups@2023-04-01' = {
              name: 'thing'
            }
            """;

        await PublishModuleToRegistryAsync(new ServiceBuilder(), clientFactory, this.fileSystem, new($"br:{Registry}/test/leaf:v1", Resource, WithSource: true));
        await PublishModuleToRegistryAsync(new ServiceBuilder(), clientFactory, this.fileSystem, new($"br:{Registry}/test/plain:v1", Resource));
        await PublishModuleToRegistryAsync(new ServiceBuilder(), clientFactory, this.fileSystem, new($"br:{Registry}/test/empty:v1", "param p string = ''", WithSource: true));

        var storage = await CompilationHelper.RestoreAndCompile(
            services,
            ("main.bicep", """
                resource nsg 'Microsoft.Network/networkSecurityGroups@2023-04-01' = {
                  name: 'nsg'
                }

                resource copies 'Microsoft.Network/networkSecurityGroups@2023-04-01' = [for i in range(0, 2): {
                  name: 'copy${i}'
                }]

                module inner 'modules/inner.bicep' = [for i in range(0, 2): {
                  name: 'inner${i}'
                  params: {
                    nsgId: nsg.id
                  }
                }]

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

    /// <summary>Compiles a document that references the registry module <c>test/{module}:v1</c>, restoring it.</summary>
    private static async Task<CompilationContext> CompileAndRestore(ServiceBuilder services, string module)
    {
        var result = await CompilationHelper.RestoreAndCompile(services, ("main.bicep", $$"""
            module {{module}} 'br:mockregistry.io/test/{{module}}:v1' = {
              name: '{{module}}'
            }
            """));
        result.Should().NotHaveAnyDiagnostics();

        return new CompilationContext(result.Compilation);
    }
}
