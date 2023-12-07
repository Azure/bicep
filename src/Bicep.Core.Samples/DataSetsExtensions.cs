// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.Configuration;
using Bicep.Core.FileSystem;
using Bicep.Core.Modules;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;
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
using Newtonsoft.Json;

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

        public static IContainerRegistryClientFactory CreateMockRegistryClients(this DataSet dataSet, bool enablePublishSource, params (Uri registryUri, string repository)[] additionalClients)
            => CreateMockRegistryClients(dataSet.RegistryModules, enablePublishSource, additionalClients);

        public static IContainerRegistryClientFactory CreateMockRegistryClientsForProviders(this DataSet dataSet, params (Uri registryUri, string repository)[] additionalClients)
            => CreateMockRegistryClientsForProviders(dataSet.RegistryProviders, additionalClients);

        public static IContainerRegistryClientFactory CreateMockRegistryClients(ImmutableDictionary<string, DataSet.ExternalModuleInfo> registryModules, bool enablePublishSource, params (Uri registryUri, string repository)[] additionalClients)
        {
            var featureProviderFactory = BicepTestConstants.CreateFeatureProviderFactory(new FeatureProviderOverrides(PublishSourceEnabled: enablePublishSource));
            var dispatcher = ServiceBuilder.Create(s => s.WithDisabledAnalyzersConfiguration()
                .AddSingleton(BicepTestConstants.ClientFactory)
                .AddSingleton(BicepTestConstants.TemplateSpecRepositoryFactory)
                .AddSingleton(featureProviderFactory)
                ).Construct<IModuleDispatcher>();

            var clients = new List<(Uri registryUri, string repository)>();

            foreach (var (moduleName, publishInfo) in registryModules)
            {
                var target = publishInfo.Metadata.Target;

                if (!dispatcher.TryGetArtifactReference(ArtifactType.Module, target, RandomFileUri()).IsSuccess(out var @ref) || @ref is not OciArtifactReference targetReference)
                {
                    throw new InvalidOperationException($"Module '{moduleName}' has an invalid target reference '{target}'. Specify a reference to an OCI artifact.");
                }

                Uri registryUri = new($"https://{targetReference.Registry}");
                clients.Add((registryUri, targetReference.Repository));
            }

            return CreateMockRegistryClients(enablePublishSource, clients.Concat(additionalClients).ToArray()).factoryMock;
        }

        public static IContainerRegistryClientFactory CreateMockRegistryClientsForProviders(ImmutableDictionary<string, DataSet.ExternalProviderInfo> registryModules, params (Uri registryUri, string repository)[] additionalClients)
        {
            var dispatcher = ServiceBuilder.Create(s => s.WithDisabledAnalyzersConfiguration()
                .AddSingleton(BicepTestConstants.ClientFactory)
                .AddSingleton(BicepTestConstants.TemplateSpecRepositoryFactory)
                ).Construct<IModuleDispatcher>();

            var clients = new List<(Uri registryUri, string repository)>();

            /*foreach (var (moduleName, publishInfo) in registryModules)
            {
                var target = publishInfo.Metadata.Target;

                if (!dispatcher.TryGetArtifactReference(ArtifactType.Module, target, RandomFileUri()).IsSuccess(out var @ref) || @ref is not OciArtifactReference targetReference)
                {
                    throw new InvalidOperationException($"Module '{moduleName}' has an invalid target reference '{target}'. Specify a reference to an OCI artifact.");
                }

                Uri registryUri = new($"https://{targetReference.Registry}");
                clients.Add((registryUri, targetReference.Repository));
            }*/

            return CreateMockRegistryClientsForProviders(clients.Concat(additionalClients).ToArray()).factoryMock;
        }

        public static (IContainerRegistryClientFactory factoryMock, ImmutableDictionary<(Uri, string), MockRegistryBlobClient> blobClientMocks) CreateMockRegistryClients(bool? publishSource, params (Uri registryUri, string repository)[] clients)
        {
            var containerRegistryFactoryBuilder = new TestContainerRegistryClientFactoryBuilder();

            foreach (var (registryUri, repository) in clients)
            {
                containerRegistryFactoryBuilder.RegisterMockRepositoryBlobClient(registryUri, repository);

            }

            return containerRegistryFactoryBuilder.Build();
        }

        public static (IContainerRegistryClientFactory factoryMock, ImmutableDictionary<(Uri, string), MockRegistryBlobClient> blobClientMocks) CreateMockRegistryClientsForProviders(params (Uri registryUri, string repository)[] clients)
        {
            var containerRegistryFactoryBuilder = new TestContainerRegistryClientFactoryBuilder();

            foreach (var (registryUri, repository) in clients)
            {
                containerRegistryFactoryBuilder.RegisterMockRepositoryBlobClient(registryUri, repository);

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

            var stream = new MemoryStream();
            using (var streamWriter = new StreamWriter(stream, leaveOpen: true))
            using (var writer = new JsonTextWriter(streamWriter))
            {
                await result.Template.WriteToAsync(writer);
            }
            stream.Position = 0;

            using Stream? sourcesStream = publishSource ? SourceArchive.PackSourcesIntoStream(result.Compilation.SourceFileGrouping) : null;

            await dispatcher.PublishModule(targetReference, stream, sourcesStream, documentationUri);
        }

        public static async Task PublishTypesToRegistryAsync(this DataSet dataSet, IContainerRegistryClientFactory clientFactory)
            => await PublishProvidersToRegistryAsync(dataSet.RegistryProviders, clientFactory);

        public static async Task PublishProvidersToRegistryAsync(ImmutableDictionary<string, DataSet.ExternalProviderInfo> registryType, IContainerRegistryClientFactory clientFactory)
        {
            foreach (var (fileName, publishInfo) in registryType)
            {
                await PublishProvidersToRegistryAsync(clientFactory, fileName, publishInfo.Metadata.Target, publishInfo.ProviderSource);
            }
        }

        public static async Task PublishProvidersToRegistryAsync(IContainerRegistryClientFactory clientFactory, string typeName, string target, string typeSource)
        {
            var dispatcher = ServiceBuilder.Create(s => s.WithDisabledAnalyzersConfiguration()
                .AddSingleton(clientFactory)
                .AddSingleton(BicepTestConstants.TemplateSpecRepositoryFactory)
                ).Construct<IModuleDispatcher>();

            var targetReference = dispatcher.TryGetArtifactReference(ArtifactType.Provider, target, RandomFileUri()).IsSuccess(out var @ref) ? @ref
                : throw new InvalidOperationException($"Type '{typeName}' has an invalid target reference '{target}'. Specify a reference to an OCI artifact.");

            byte[] byteArray = Encoding.UTF8.GetBytes(typeSource);
            var stream = new MemoryStream(byteArray);

            await dispatcher.PublishProvider(targetReference, stream);
        }

        private static Uri RandomFileUri() => PathHelper.FilePathToFileUrl(Path.GetTempFileName());
    }
}
