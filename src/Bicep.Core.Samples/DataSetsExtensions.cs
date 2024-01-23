// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.IO.Abstractions;
using Bicep.Core.Configuration;
using Bicep.Core.FileSystem;
using Bicep.Core.Modules;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;
using Bicep.Core.Registry.Providers;
using Bicep.Core.Semantics;
using Bicep.Core.SourceCode;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Registry;
using Bicep.Core.UnitTests.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Bicep.Core.Samples
{
    public static class DataSetsExtensions
    {
        public static IEnumerable<object[]> ToDynamicTestData(this IEnumerable<DataSet> source) => source.Select(ToDynamicTestData);

        public static object[] ToDynamicTestData(this DataSet ds) => new object[] { ds };

        public static bool HasCrLfNewlines(this DataSet dataSet)
            => dataSet.Name.EndsWith("_CRLF", StringComparison.Ordinal);

        public static string SaveFilesToTestDirectory(this DataSet dataSet, TestContext testContext)
            => FileHelper.SaveEmbeddedResourcesWithPathPrefix(testContext, typeof(DataSet).Assembly, dataSet.GetStreamPrefix());

        public static async Task<(Compilation compilation, string outputDirectory, Uri fileUri)> SetupPrerequisitesAndCreateCompilation(this DataSet dataSet, TestContext testContext, FeatureProviderOverrides? features = null, bool enablePublishSource = true)
        {
            features ??= new(testContext, RegistryEnabled: dataSet.HasExternalModules);
            var outputDirectory = dataSet.SaveFilesToTestDirectory(testContext);
            var clientFactory = dataSet.CreateMockRegistryClients(enablePublishSource);
            await dataSet.PublishModulesToRegistryAsync(clientFactory, enablePublishSource);
            var templateSpecRepositoryFactory = dataSet.CreateMockTemplateSpecRepositoryFactory(testContext, enablePublishSource);

            var compiler = ServiceBuilder.Create(s => s.AddSingleton(templateSpecRepositoryFactory).AddSingleton(clientFactory).WithFeatureOverrides(features)).GetCompiler();

            var fileUri = PathHelper.FilePathToFileUrl(Path.Combine(outputDirectory, DataSet.TestFileMain));
            var compilation = await compiler.CreateCompilation(fileUri);

            return (compilation, outputDirectory, fileUri);
        }

        public static IContainerRegistryClientFactory CreateMockRegistryClients(this DataSet dataSet, bool enablePublishSource, params (string registryUri, string repository)[] additionalClients)
            => CreateMockRegistryClients(dataSet.RegistryModules, enablePublishSource, additionalClients);

        public static IContainerRegistryClientFactory CreateMockRegistryClients(ImmutableDictionary<string, DataSet.ExternalModuleInfo> registryModules, bool enablePublishSource, params (string registryUri, string repository)[] additionalClients)
        {
            var featureProviderFactory = BicepTestConstants.CreateFeatureProviderFactory(new FeatureProviderOverrides(PublishSourceEnabled: enablePublishSource));
            var dispatcher = ServiceBuilder.Create(s => s.WithDisabledAnalyzersConfiguration()
                .AddSingleton(BicepTestConstants.ClientFactory)
                .AddSingleton(BicepTestConstants.TemplateSpecRepositoryFactory)
                .AddSingleton(featureProviderFactory)
                ).Construct<IModuleDispatcher>();

            var clients = new List<(string, string)>();

            foreach (var (moduleName, publishInfo) in registryModules)
            {
                var target = publishInfo.Metadata.Target;

                if (!dispatcher.TryGetArtifactReference(ArtifactType.Module, target, RandomFileUri()).IsSuccess(out var @ref) || @ref is not OciArtifactReference targetReference)
                {
                    throw new InvalidOperationException($"Module '{moduleName}' has an invalid target reference '{target}'. Specify a reference to an OCI artifact.");
                }

                clients.Add((targetReference.Registry, targetReference.Repository));
            }

            return CreateMockRegistryClients(clients.Concat(additionalClients).ToArray()).factoryMock;
        }

        public static (IContainerRegistryClientFactory factoryMock, ImmutableDictionary<(Uri, string), MockRegistryBlobClient> blobClientMocks) CreateMockRegistryClients(params (string, string)[] clients)
        {
            var containerRegistryFactoryBuilder = new TestContainerRegistryClientFactoryBuilder();

            foreach (var (registryHost, repository) in clients)
            {
                containerRegistryFactoryBuilder.RegisterMockRepositoryBlobClient(registryHost, repository);

            }

            return containerRegistryFactoryBuilder.Build();
        }

        public static ITemplateSpecRepositoryFactory CreateEmptyTemplateSpecRepositoryFactory(bool enablePublishSource = false)
            => CreateMockTemplateSpecRepositoryFactory(ImmutableDictionary<string, DataSet.ExternalModuleInfo>.Empty, enablePublishSource);

        public static ITemplateSpecRepositoryFactory CreateMockTemplateSpecRepositoryFactory(this DataSet dataSet, TestContext _, bool enablePublishSource = false)
            => CreateMockTemplateSpecRepositoryFactory(dataSet.TemplateSpecs, enablePublishSource);

        public static ITemplateSpecRepositoryFactory CreateMockTemplateSpecRepositoryFactory(ImmutableDictionary<string, DataSet.ExternalModuleInfo> templateSpecs, bool enablePublishSource = false)
        {
            var featureProviderFactory = BicepTestConstants.CreateFeatureProviderFactory(new FeatureProviderOverrides(PublishSourceEnabled: enablePublishSource));
            var dispatcher = ServiceBuilder.Create(s => s.WithDisabledAnalyzersConfiguration()
                .AddSingleton(BicepTestConstants.ClientFactory)
                .AddSingleton(BicepTestConstants.TemplateSpecRepositoryFactory)
                .AddSingleton(featureProviderFactory)
                ).Construct<IModuleDispatcher>();
            var repositoryMocksBySubscription = new Dictionary<string, Mock<ITemplateSpecRepository>>();

            foreach (var (moduleName, templateSpecInfo) in templateSpecs)
            {
                if (!dispatcher.TryGetArtifactReference(ArtifactType.Module, templateSpecInfo.Metadata.Target, RandomFileUri()).IsSuccess(out var @ref) || @ref is not TemplateSpecModuleReference reference)
                {
                    throw new InvalidOperationException($"Module '{moduleName}' has an invalid target reference '{templateSpecInfo.Metadata.Target}'. Specify a reference to a template spec.");
                }

                repositoryMocksBySubscription.TryAdd(reference.SubscriptionId, StrictMock.Of<ITemplateSpecRepository>());
                repositoryMocksBySubscription[reference.SubscriptionId]
                    .Setup(x => x.FindTemplateSpecByIdAsync(reference.TemplateSpecResourceId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new TemplateSpecEntity(templateSpecInfo.ModuleSource));
            }

            var repositoryFactoryMock = StrictMock.Of<ITemplateSpecRepositoryFactory>();
            repositoryFactoryMock
                .Setup(x => x.CreateRepository(It.IsAny<RootConfiguration>(), It.IsAny<string>()))
                .Returns<RootConfiguration, string>((_, subscriptionId) =>
                    repositoryMocksBySubscription.TryGetValue(subscriptionId, out var repository)
                        ? repository.Object
                        : throw new InvalidOperationException($"No mock client was registered for subscription '{subscriptionId}'."));

            return repositoryFactoryMock.Object;
        }

        public static async Task PublishModulesToRegistryAsync(this DataSet dataSet, IContainerRegistryClientFactory clientFactory, bool publishSource = true)
            => await PublishModulesToRegistryAsync(dataSet.RegistryModules, clientFactory, publishSource);

        public static async Task PublishModulesToRegistryAsync(ImmutableDictionary<string, DataSet.ExternalModuleInfo> registryModules, IContainerRegistryClientFactory clientFactory, bool publishSource)
        {
            foreach (var (moduleName, publishInfo) in registryModules)
            {
                await PublishModuleToRegistryAsync(clientFactory, moduleName, publishInfo.Metadata.Target, publishInfo.ModuleSource, publishSource, null);
            }
        }

        public static async Task PublishModuleToRegistryAsync(IContainerRegistryClientFactory clientFactory, string moduleName, string target, string moduleSource, bool publishSource, string? documentationUri = null)
        {
            var featureProviderFactory = BicepTestConstants.CreateFeatureProviderFactory(new FeatureProviderOverrides(PublishSourceEnabled: publishSource));
            var dispatcher = ServiceBuilder.Create(s => s.WithDisabledAnalyzersConfiguration()
                .AddSingleton(clientFactory)
                .AddSingleton(BicepTestConstants.TemplateSpecRepositoryFactory)
                .AddSingleton(featureProviderFactory)
                ).Construct<IModuleDispatcher>();

            var targetReference = dispatcher.TryGetArtifactReference(ArtifactType.Module, target, RandomFileUri()).IsSuccess(out var @ref) ? @ref
                : throw new InvalidOperationException($"Module '{moduleName}' has an invalid target reference '{target}'. Specify a reference to an OCI artifact.");

            var result = CompilationHelper.Compile(moduleSource);
            if (result.Template is null)
            {
                throw new InvalidOperationException($"Module {moduleName} failed to procuce a template.");
            }
            
            BinaryData? sourcesStream = publishSource ? BinaryData.FromStream(SourceArchive.PackSourcesIntoStream(result.Compilation.SourceFileGrouping)) : null;
            await dispatcher.PublishModule(targetReference, BinaryData.FromString(result.Template.ToString()), sourcesStream, documentationUri);
        }

        public static async Task PublishProviderToRegistryAsync(IDependencyHelper services, string pathToIndexJson, string target)
        {
            var dispatcher = services.Construct<IModuleDispatcher>();
            var fileSystem = services.Construct<IFileSystem>();

            var targetReference = dispatcher.TryGetArtifactReference(ArtifactType.Provider, target, PathHelper.FilePathToFileUrl(pathToIndexJson)).IsSuccess(out var @ref) ? @ref
                : throw new InvalidOperationException($"Invalid target reference '{target}'. Specify a reference to an OCI artifact.");

            var tgzStream = await TypesV1Archive.GenerateProviderTarStream(fileSystem, pathToIndexJson);

            await dispatcher.PublishProvider(targetReference, tgzStream);
        }

        private static Uri RandomFileUri() => PathHelper.FilePathToFileUrl(Path.GetTempFileName());

        public static async Task PublishAzProvider(IDependencyHelper services, string pathToIndexJson)
        {
            var version = BicepTestConstants.BuiltinAzProviderVersion;
            var repository = "bicep/providers/az";
            await PublishProviderToRegistryAsync(services, pathToIndexJson, $"br:{LanguageConstants.BicepPublicMcrRegistry}/{repository}:{version}");
        }

        public static IContainerRegistryClientFactory CreateOciClientForAzProvider()
            => CreateMockRegistryClients((LanguageConstants.BicepPublicMcrRegistry, $"bicep/providers/az")).factoryMock;
    }
}
