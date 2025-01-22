// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using Azure.Containers.ContainerRegistry;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Extensions;
using Bicep.Core.Registry.Oci;
using Bicep.Core.SourceCode;
using Bicep.Core.UnitTests.Extensions;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.Registry;
using Bicep.IO.FileSystem;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using static Bicep.Core.UnitTests.Registry.FakeContainerRegistryClient;

namespace Bicep.Core.UnitTests.Utils;

public static class RegistryHelper //asdfg turn into an instance class?
{
    //asdfg useful to keep this tuple?  public static IContainerRegistryClientFactory CreateMockRegistryClient(string registry, string repository, string[] tags)
    public static IContainerRegistryClientFactory CreateMockRegistryClient(RepoDescriptor repo)
    {
        return new TestContainerRegistryClientFactoryBuilder()
            .WithRepository(repo)
            .Build().clientFactory;
    }

    //asdfg move/rename
    public static class RepoDescriptorsBuilder //asdfg2
    {
        public static List<RepoTagDescriptor> ToRepoTags(IEnumerable<string> tags)
        {
            return [.. tags.Select(tag => new RepoTagDescriptor(tag))];
        }

        public static ModuleToPublish[] ToModulesToPublish(IEnumerable<RepoDescriptor> descriptors, bool withSource = false)
        {
            return [.. descriptors.SelectMany(
                descriptor => descriptor.Tags.Select(
                    tag => new ModuleToPublish(
                        ModuleToPublish.ToTarget(descriptor.Registry, descriptor.Repository, tag.Tag),
                        BicepSource: "// bicep source",
                        WithSource: withSource,
                        DocumentationUri: tag.DocumentationUri)
                )
            )];
        }

        public static RepoDescriptor[] ToRepoDescriptors(IEnumerable<ModuleToPublish> modules)
        {
            var descriptors = new List<RepoDescriptor>();

            foreach (var module in modules)
            {
                var found = descriptors.SingleOrDefault(d => d.Registry == module.Registry && d.Repository == module.Repository);
                if (found is { })
                {
                    found.Tags.Add(new RepoTagDescriptor(module.Tag));
                }
                else
                {
                    descriptors.Add(new(module.Registry, module.Repository, [.. ToRepoTags([module.Tag])]));
                }
            }

            return [.. descriptors];
        }

        //private readonly List<RepoDescriptor> repos = new();
        //public RepoDescriptorsBuilder Add(string registry, string repository, params RepoTag[] tags)
        //{
        //    repos.Add(new(registry, repository, tags));
        //    return this;
        //}
        //public RepoDescriptorsBuilder Add(string registry, string repository, string tag, string? description = null, string? documentationUri = null)
        //{
        //    repos.Add(new(registry, repository, [new(tag, description, documentationUri)]);
        //    return this;
        //}
        //public RepoDescriptor[] Build() => repos.ToArray();
    }

    public static
        (IContainerRegistryClientFactory factoryMock, ImmutableDictionary<(Uri, string), MockRegistryBlobClient>, FakeContainerRegistryClient containerRegistryClient/*asdfg don't return?*/)
        CreateMockRegistryClients(params RepoDescriptor[] clients)
    {
        return CreateMockRegistryClients(new FakeContainerRegistryClient(), clients);
    }

    public static
    /* create type */ (IContainerRegistryClientFactory factoryMock, ImmutableDictionary<(Uri, string), MockRegistryBlobClient>, FakeContainerRegistryClient containerRegistryClient/*asdfg don't return?*/)
    CreateMockRegistryClients(
        FakeContainerRegistryClient containerRegistryClient,
        params RepoDescriptor[] repos)
    {
        var containerRegistryFactoryBuilder = new TestContainerRegistryClientFactoryBuilder();

        containerRegistryFactoryBuilder.WithFakeContainerRegistryClient(containerRegistryClient);

        var modules = RepoDescriptorsBuilder.ToModulesToPublish(repos);

        foreach (var repo in repos)
        {
            containerRegistryFactoryBuilder.WithRepository(repo);
        }

        return containerRegistryFactoryBuilder.Build();
    }

#if false//asdfg shoud jus have one overload with tags?
    //public static
    ///* asdfg create type */ (IContainerRegistryClientFactory factoryMock, ImmutableDictionary<(Uri, string), MockRegistryBlobClient>, FakeContainerRegistryClient containerRegistryClient/*asdfg don't return?*/)
    //CreateMockRegistryClients(
    //    FakeContainerRegistryClient containerRegistryClient,
    //    params (string registry, string repo, string[] tags)[] clients)
    //{
    //    var containerRegistryFactoryBuilder = new TestContainerRegistryClientFactoryBuilder();

    //    containerRegistryFactoryBuilder.WithFakeContainerRegistryClient(containerRegistryClient);
    //    var repos = new Dictionary<string, FakeContainerRegistryClient.FakeRepository>();

    //    foreach (var (registryHost, repository, tags) in clients)
    //    {
    //        repos[repository] = new(registryHost, repository, [.. tags]);
    //    }

    //    foreach (var (registryHost, repository, tags) in repos.Values)
    //    {
    //        containerRegistryFactoryBuilder.WithRepository(registryHost, repository, [.. tags]);
    //    }

    //    return containerRegistryFactoryBuilder.Build();
    //}
#endif

    // Example target: br:mockregistry.io/test/module1:v1
    public static async Task PublishModuleToRegistryAsync(
        IContainerRegistryClientFactory clientFactory,
        IFileSystem fileSystem,
        string moduleName, //asdfg2 needed?
        ModuleToPublish module)
    {
        var fileExplorer = new FileSystemFileExplorer(fileSystem);
        var configurationManager = new ConfigurationManager(fileExplorer);
        var featureProviderFactory = new OverriddenFeatureProviderFactory(new FeatureProviderFactory(configurationManager, fileExplorer), BicepTestConstants.FeatureOverrides);

        var services = new ServiceBuilder()
            .WithDisabledAnalyzersConfiguration()
            .WithContainerRegistryClientFactory(clientFactory)
            .WithFileSystem(fileSystem)
            .WithTemplateSpecRepositoryFactory(BicepTestConstants.TemplateSpecRepositoryFactory)
            .WithFeatureProviderFactory(featureProviderFactory);

        var dispatcher = services.Build().Construct<IModuleDispatcher>();

        var targetReference = dispatcher.TryGetArtifactReference(ArtifactType.Module, module.PublishTarget, RandomFileUri()).IsSuccess(out var @ref) ? @ref
            : throw new InvalidOperationException($"Module '{moduleName}' has an invalid target reference '{module.PublishTarget}'. Specify a reference to an OCI artifact.");

        var result = await CompilationHelper.RestoreAndCompile(services, module.BicepSource);
        if (result.Template is null)
        {
            throw new InvalidOperationException($"Module {moduleName} failed to produce a template.");
        }

        var features = featureProviderFactory.GetFeatureProvider(result.BicepFile.Uri);
        BinaryData? sourcesStream = module.WithSource ? BinaryData.FromStream(SourceArchive.PackSourcesIntoStream(dispatcher, result.Compilation.SourceFileGrouping, features.CacheRootDirectory)) : null;
        await dispatcher.PublishModule(targetReference, BinaryData.FromString(result.Template.ToString()), sourcesStream, module.DocumentationUri);
    }

    // Example target: br:mockregistry.io/test/module1:v1
    // Module name is automatically extracted from target (in this case, "module1")
    public static async Task PublishModuleToRegistryAsync(IContainerRegistryClientFactory clientFactory, IFileSystem fileSystem, ModuleToPublish module)
    {
        await PublishModuleToRegistryAsync(
              clientFactory,
              fileSystem,
              module.Repository.Split('/').Last(), //asdfg2 needed?
              module);
    }

    public record RepoTagDescriptor(
        string Tag,
        string? Description = null,
        string? DocumentationUri = null);

    //asdfg move? rename?
    public record class RepoDescriptor(
        string Registry, // e.g. "registry.contoso.io"
        string Repository, // e.g. "test/module1"
        List<RepoTagDescriptor> Tags)
    {
        public RepoDescriptor(
            string Registry, // e.g. "registry.contoso.io"
            string Repository, // e.g. "test/module1"
            IEnumerable<string> Tags) : this(Registry, Repository, RepoDescriptorsBuilder.ToRepoTags(Tags)) { }
    }

    //asdfg move?
    public record class ModuleToPublish(
        string PublishTarget, // e.g. "br:registry.contoso.io/test/module1:v1"
        string BicepSource,
        bool WithSource = false, // whether to publish the source with the module
        string? DocumentationUri = null)
    {
        public static string ToTarget(string registry, string repo, string tag) => $"br:{registry}/{repo}:{tag}";

        private string TargetWithoutScheme
        {
            get
            {
                PublishTarget.Should().StartWith("br:");
                return PublishTarget.Substring("br:".Length);
            }
        }

        private IArtifactAddressComponents ParsedTarget
        {
            get
            {
                if (OciArtifactReference.TryParseFullyQualifiedComponents(TargetWithoutScheme).IsSuccess(out var parsedTarget, out var errorBuilder))
                {
                    return parsedTarget;
                }
                else
                {
                    throw new InvalidOperationException($"Failed to parse target '{errorBuilder(DiagnosticBuilder.ForPosition(new(0, 0))).Message}'.");
                }
            }
        }

        public string Registry => ParsedTarget.Registry;
        public string Repository => ParsedTarget.Repository;
        public string Tag => ParsedTarget.Tag!;

        //asdfg        public static RepoDescriptor ToDescriptor(ModuleToPublish module) => new(module.Registry, module.Repository, [new(module.Tag)]/*asdfg??*/, module.DocumentationUri);
    }

    // Creates a new registry client factory and publishes the specified modules to the registry.
    // Example usage:
    //   var clientFactory = await RegistryHelper.CreateMockRegistryClientWithPublishedModulesAsync(
    //      new MockFileSystem(),
    //      [
    //        new("br:mockregistry.io/test/module1:v1", "param p1 bool", WithSource: true),
    //        new("br:mockregistry.io/test/module2:v1", "param p2 string", WithSource: true),
    //        new("br:mockregistry.io/test/module1:v2", "param p12 string", WithSource: false),
    //      ]);
    public static async Task<IContainerRegistryClientFactory> CreateMockRegistryClientWithPublishedModulesAsync(
        IFileSystem fileSystem,
        FakeContainerRegistryClient containerRegistryClient,
        params ModuleToPublish[] modules
    )
    {
        var repos = RepoDescriptorsBuilder.ToRepoDescriptors(modules);

        var clientFactory = CreateMockRegistryClients(containerRegistryClient, repos).factoryMock;

        foreach (var module in modules)
        {
            await PublishModuleToRegistryAsync(
                  clientFactory,
                  fileSystem,
                  module);
        }

        return clientFactory;
    }

    public static async Task<IContainerRegistryClientFactory> CreateMockRegistryClientWithPublishedModulesAsync(
        IFileSystem fileSystem,
        params ModuleToPublish[] modules
    )
    {
        return await CreateMockRegistryClientWithPublishedModulesAsync(fileSystem, new FakeContainerRegistryClient(), modules);
    }

    public static async Task PublishExtensionToRegistryAsync(IDependencyHelper services, string pathToIndexJson, string target)
    {
        var fileSystem = services.Construct<IFileSystem>();

        var tgzData = await TypesV1Archive.GenerateExtensionTarStream(fileSystem, pathToIndexJson);

        await PublishExtensionToRegistryAsync(services, target, tgzData);
    }

    public static async Task PublishExtensionToRegistryAsync(IDependencyHelper services, string target, BinaryData tgzData)
    {
        var dispatcher = services.Construct<IModuleDispatcher>();

        var targetUri = PathHelper.FilePathToFileUrl(PathHelper.ResolvePath("dummy"));
        if (!target.StartsWith("br:"))
        {
            // convert to a relative path, as this is the only format supported for the local filesystem
            targetUri = PathHelper.FilePathToFileUrl(PathHelper.ResolvePath(target));
            target = Path.GetFileName(targetUri.LocalPath);
        }

        if (!dispatcher.TryGetArtifactReference(ArtifactType.Extension, target, targetUri).IsSuccess(out var targetReference, out var errorBuilder))
        {
            throw new InvalidOperationException($"Failed to get reference '{errorBuilder(DiagnosticBuilder.ForDocumentStart()).Message}'.");
        }

        await dispatcher.PublishExtension(targetReference, new(tgzData, false, []));
    }

    private static Uri RandomFileUri() => PathHelper.FilePathToFileUrl(Path.GetTempFileName());

    public static async Task PublishAzExtension(IDependencyHelper services, string pathToIndexJson)
    {
        var version = BicepTestConstants.BuiltinAzExtensionVersion;
        var repository = "bicep/extensions/az";
        await PublishExtensionToRegistryAsync(services, pathToIndexJson, $"br:{LanguageConstants.BicepPublicMcrRegistry}/{repository}:{version}");
    }

    public static async Task PublishMsGraphExtension(IDependencyHelper services, string pathToIndexJson, string repoVersion, string extensionVersion)
    {
        var repository = "bicep/extensions/microsoftgraph/" + repoVersion;
        await PublishExtensionToRegistryAsync(services, pathToIndexJson, $"br:{LanguageConstants.BicepPublicMcrRegistry}/{repository}:{extensionVersion}");
    }

    public static IContainerRegistryClientFactory CreateOciClientForAzExtension()
       => CreateMockRegistryClients(new RepoDescriptor(
            LanguageConstants.BicepPublicMcrRegistry,
            "bicep/extensions/az",
            new List<RepoTagDescriptor> { new("tag") }
        )).factoryMock;

    public static IContainerRegistryClientFactory CreateOciClientForMsGraphExtension()
        => CreateMockRegistryClients(
            new RepoDescriptor(LanguageConstants.BicepPublicMcrRegistry, $"bicep/extensions/microsoftgraph/beta", ["tag"]),
            new RepoDescriptor(LanguageConstants.BicepPublicMcrRegistry, $"bicep/extensions/microsoftgraph/v1", ["tag"])
            ).factoryMock;
}
