// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using Azure.Bicep.Types.Serialization;
using Azure.Containers.ContainerRegistry;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Extensions;
using Bicep.Core.Registry.Oci;
using Bicep.Core.SourceGraph;
using Bicep.Core.SourceLink;
using Bicep.Core.Syntax;
using Bicep.Core.UnitTests.Extensions;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.Registry;
using Bicep.IO.FileSystem;
using Bicep.IO.Utils;
using FluentAssertions;
using Google.Protobuf;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using static Bicep.Core.UnitTests.Registry.FakeContainerRegistryClient;
using static Bicep.Core.UnitTests.Utils.TestContainerRegistryClientFactoryBuilder;

namespace Bicep.Core.UnitTests.Utils;

public static class RegistryHelper
{
    public record class RepoDescriptor(
        string Registry, // e.g. "registry.contoso.io"
        string Repository, // e.g. "test/module1"
        List<RepoTagDescriptor> Tags)
    {
        public RepoDescriptor(
            string Registry, // e.g. "registry.contoso.io"
            string Repository, // e.g. "test/module1"
            IEnumerable<string> Tags) : this(Registry, Repository, ToTagDescriptors(Tags)) { }
    }

    public record RepoTagDescriptor(
        string Tag,
        string? Description = null,
        string? DocumentationUri = null
    );

    public record class ModuleToPublish(
        string PublishTarget, // e.g. "br:registry.contoso.io/test/module1:v1"
        string BicepSource,
        bool WithSource = false, // whether to publish the source with the module
        string? DocumentationUri = null
    )
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

        private IOciArtifactAddressComponents ParsedTarget
        {
            get
            {
                if (OciArtifactAddressComponents.TryParse(TargetWithoutScheme).IsSuccess(out var parsedTarget, out var errorBuilder))
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
        public string ModuleName => Repository.Split('/').Last();
    }

    public static IContainerRegistryClientFactory CreateMockRegistryClient(params RepoDescriptor[] repos)
    {
        var containerRegistryFactoryBuilder = new TestContainerRegistryClientFactoryBuilder();

        foreach (var repo in repos)
        {
            containerRegistryFactoryBuilder.WithRepository(repo);
        }

        return containerRegistryFactoryBuilder.Build();
    }

    public static IContainerRegistryClientFactory CreateMockRegistryClients(
        FakeContainerRegistryClient containerRegistryClient,
        params RepoDescriptor[] repos)
    {
        var containerRegistryFactoryBuilder = new TestContainerRegistryClientFactoryBuilder();

        containerRegistryFactoryBuilder.WithFakeContainerRegistryClient(containerRegistryClient);

        var modules = DescriptorsToModulesToPublish(repos);

        foreach (var repo in repos)
        {
            containerRegistryFactoryBuilder.WithRepository(repo);
        }

        return containerRegistryFactoryBuilder.Build();
    }

    public static async Task PublishModuleToRegistryAsync(
        ServiceBuilder serviceBuilder,
        IContainerRegistryClientFactory clientFactory,
        IFileSystem fileSystem,
        ModuleToPublish module)
    {
        var fileExplorer = new FileSystemFileExplorer(fileSystem);
        var configurationManager = new ConfigurationManager(fileExplorer);
        var featureProviderFactory = new OverriddenFeatureProviderFactory(new FeatureProviderFactory(configurationManager, fileExplorer), BicepTestConstants.FeatureOverrides);

        serviceBuilder = serviceBuilder
            .WithDisabledAnalyzersConfiguration()
            .WithContainerRegistryClientFactory(clientFactory)
            .WithFileSystem(fileSystem)
            .WithTemplateSpecRepositoryFactory(BicepTestConstants.TemplateSpecRepositoryFactory)
            .WithFeatureProviderFactory(featureProviderFactory);

        var services = serviceBuilder.Build();
        var dispatcher = services.Construct<IModuleDispatcher>();
        var sourceFileFactory = services.Construct<ISourceFileFactory>();
        var dummyFile = sourceFileFactory.CreateBicepFile(PathHelper.FilePathToFileUrl(fileSystem.Path.GetFullPath("main.bicep")).ToIOUri(), "");

        var targetReference = dispatcher.TryGetArtifactReference(dummyFile, ArtifactType.Module, module.PublishTarget).IsSuccess(out var @ref) ? @ref
            : throw new InvalidOperationException($"Module '{module.ModuleName}' has an invalid target reference '{module.PublishTarget}'. Specify a reference to an OCI artifact.");

        var result = await CompilationHelper.RestoreAndCompile(serviceBuilder, module.BicepSource);
        if (result.Template is null)
        {
            throw new InvalidOperationException($"Module {module.ModuleName} failed to produce a template.");
        }

        BinaryData? sourcesStream = module.WithSource ? SourceArchive.CreateFrom(result.Compilation.SourceFileGrouping).PackIntoBinaryData() : null;
        await dispatcher.PublishModule(targetReference, BinaryData.FromString(result.Template.ToString()), sourcesStream, module.DocumentationUri);
    }

    // Creates a new registry client factory and publishes the specified modules to the registry.
    // Example usage:
    //   var clientFactory = await RegistryHelper.CreateMockRegistryClientWithPublishedModulesAsync(
    //      new MockFileSystem(),
    //      [
    //        new("br:mockregistry.io/test/module1:v1", "param p1 bool", WithSource: true),
    //        new("br:mockregistry.io/test/module2:v1", "param p2 string", WithSource: true),
    //        new("br:mockregistry.io/test/module1:v2", "param p12 string"),
    //      ]);
    public static async Task<IContainerRegistryClientFactory> CreateMockRegistryClientWithPublishedModulesAsync(
        IFileSystem fileSystem,
        FakeContainerRegistryClient containerRegistryClient,
        params ModuleToPublish[] modules
    )
    {
        var repos = ModulesToPublishToDescriptors(modules);

        var clientFactory = CreateMockRegistryClients(containerRegistryClient, repos);

        foreach (var module in modules)
        {
            await PublishModuleToRegistryAsync(
                new ServiceBuilder(),
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

        var tgzData = await GenerateExtensionTarStream(fileSystem, pathToIndexJson);

        await PublishExtensionToRegistryAsync(services, target, tgzData);
    }

    // TODO(file-io-abstraction): This is only used by tests. Needs refactoring.
    public static async Task<BinaryData> GenerateExtensionTarStream(IFileSystem fileSystem, string indexJsonPath)
    {
        using var stream = new MemoryStream();
        using (var tgzWriter = new TgzWriter(stream, leaveOpen: true))
        {
            var indexJson = await fileSystem.File.ReadAllTextAsync(indexJsonPath);
            await tgzWriter.WriteEntryAsync("index.json", indexJson);

            var indexJsonParentPath = Path.GetDirectoryName(indexJsonPath);
            var uniqueTypePaths = GetAllUniqueTypePaths(indexJsonPath, fileSystem);

            foreach (var relativePath in uniqueTypePaths)
            {
                var absolutePath = Path.Combine(indexJsonParentPath!, relativePath);
                var typesJson = await fileSystem.File.ReadAllTextAsync(absolutePath);
                await tgzWriter.WriteEntryAsync(relativePath, typesJson);
            }
        }

        stream.Seek(0, SeekOrigin.Begin);

        return BinaryData.FromStream(stream);
    }

    private static IEnumerable<string> GetAllUniqueTypePaths(string pathToIndex, IFileSystem fileSystem)
    {
        using var indexStream = fileSystem.FileStream.New(pathToIndex, FileMode.Open, FileAccess.Read);

        var index = TypeSerializer.DeserializeIndex(indexStream);

        var typeReferences = index.Resources.Values.ToList();
        if (index.Settings?.ConfigurationType is { } configType)
        {
            typeReferences.Add(configType);
        }
        if (index.FallbackResourceType is { } fallbackType)
        {
            typeReferences.Add(fallbackType);
        }

        return typeReferences.Select(x => x.RelativePath).Distinct();
    }

    public static async Task PublishExtensionToRegistryAsync(IDependencyHelper services, string target, BinaryData tgzData, Uri? bicepFileUri = null)
        => await PublishExtensionToRegistryAsync(services, target, new ExtensionPackage(tgzData, false, []), bicepFileUri);

    public static Task PublishExtensionToRegistryAsync(IDependencyHelper services, string target, ExtensionPackage package, Uri? bicepFileUri = null)
        => PublishExtensionToRegistryAsync(services.Construct<IModuleDispatcher>(), services.Construct<ISourceFileFactory>(), target, package, bicepFileUri);

    public static Task PublishExtensionToRegistryAsync(IServiceProvider services, string target, ExtensionPackage package, Uri? bicepFileUri = null)
        => PublishExtensionToRegistryAsync(services.GetRequiredService<IModuleDispatcher>(), services.GetRequiredService<ISourceFileFactory>(), target, package, bicepFileUri);

    private static async Task PublishExtensionToRegistryAsync(IModuleDispatcher dispatcher, ISourceFileFactory sourceFileFactory, string target, ExtensionPackage package, Uri? bicepFileUri = null)
    {
        if (!target.StartsWith("br:"))
        {
            // convert to a relative path, as this is the only format supported for the local filesystem
            var targetUri = PathHelper.FilePathToFileUrl(PathHelper.ResolvePath(target));
            target = Path.GetFileName(targetUri.LocalPath);
        }

        var bicepFile = bicepFileUri is not null ? sourceFileFactory.CreateBicepFile(bicepFileUri.ToIOUri(), "") : BicepTestConstants.DummyBicepFile;

        if (!dispatcher.TryGetArtifactReference(bicepFile, ArtifactType.Extension, target).IsSuccess(out var targetReference, out var errorBuilder))
        {
            throw new InvalidOperationException($"Failed to get reference '{errorBuilder(DiagnosticBuilder.ForDocumentStart()).Message}'.");
        }

        await dispatcher.PublishExtension(targetReference, package);
    }

    private static List<RepoTagDescriptor> ToTagDescriptors(IEnumerable<string> tags)
    {
        return [.. tags.Select(tag => new RepoTagDescriptor(tag))];
    }

    private static ModuleToPublish[] DescriptorsToModulesToPublish(IEnumerable<RepoDescriptor> descriptors, bool withSource = false)
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

    private static RepoDescriptor[] ModulesToPublishToDescriptors(IEnumerable<ModuleToPublish> modules)
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
                descriptors.Add(new(module.Registry, module.Repository, [.. ToTagDescriptors([module.Tag])]));
            }
        }

        return [.. descriptors];
    }

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
       => CreateMockRegistryClient(new RepoDescriptor(
            LanguageConstants.BicepPublicMcrRegistry,
            "bicep/extensions/az",
            new List<RepoTagDescriptor> { new("tag") }
        ));

    public static IContainerRegistryClientFactory CreateOciClientForMsGraphExtension()
        => CreateMockRegistryClient(
            new RepoDescriptor(LanguageConstants.BicepPublicMcrRegistry, $"bicep/extensions/microsoftgraph/beta", ["tag"]),
            new RepoDescriptor(LanguageConstants.BicepPublicMcrRegistry, $"bicep/extensions/microsoftgraph/v1", ["tag"])
            );
}
