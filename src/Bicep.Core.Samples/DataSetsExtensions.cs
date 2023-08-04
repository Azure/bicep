// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.Containers.ContainerRegistry;
using Azure.Identity;
using Bicep.Core.Configuration;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Modules;
using Bicep.Core.Registry;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.TypeSystem.Az;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Configuration;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Registry;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Workspaces;
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

        public static async Task<(Compilation compilation, string outputDirectory, Uri fileUri)> SetupPrerequisitesAndCreateCompilation(this DataSet dataSet, TestContext testContext, FeatureProviderOverrides? features = null, bool publishSource = true)
        {
            features ??= new(testContext, RegistryEnabled: dataSet.HasExternalModules);
            var outputDirectory = dataSet.SaveFilesToTestDirectory(testContext);
            var clientFactory = dataSet.CreateMockRegistryClients().Object;
            await dataSet.PublishModulesToRegistryAsync(clientFactory, publishSource);
            var templateSpecRepositoryFactory = dataSet.CreateMockTemplateSpecRepositoryFactory(testContext);

            var compiler = ServiceBuilder.Create(s => s.AddSingleton(templateSpecRepositoryFactory).AddSingleton(clientFactory).WithFeatureOverrides(features)).GetCompiler();

            var fileUri = PathHelper.FilePathToFileUrl(Path.Combine(outputDirectory, DataSet.TestFileMain));
            var compilation = await compiler.CreateCompilation(fileUri);

            return (compilation, outputDirectory, fileUri);
        }

        public static Mock<IContainerRegistryClientFactory> CreateMockRegistryClients(this DataSet dataSet, params (Uri registryUri, string repository)[] additionalClients)
        {
            var dispatcher = ServiceBuilder.Create(s => s.WithDisabledAnalyzersConfiguration()
                .AddSingleton(BicepTestConstants.ClientFactory)
                .AddSingleton(BicepTestConstants.TemplateSpecRepositoryFactory))
                .Construct<IModuleDispatcher>();

            var clients = new List<(Uri registryUri, string repository)>();

            foreach (var (moduleName, publishInfo) in dataSet.RegistryModules)
            {
                var target = publishInfo.Metadata.Target;

                if (!dispatcher.TryGetModuleReference(target, RandomFileUri(), out var @ref, out _) || @ref is not OciArtifactModuleReference targetReference)
                {
                    throw new InvalidOperationException($"Module '{moduleName}' has an invalid target reference '{target}'. Specify a reference to an OCI artifact.");
                }

                Uri registryUri = new Uri($"https://{targetReference.Registry}");
                clients.Add((registryUri, targetReference.Repository));
            }

            return CreateMockRegistryClients(clients.Concat(additionalClients).ToArray()).factoryMock;
        }

        public static (Mock<IContainerRegistryClientFactory> factoryMock, ImmutableDictionary<(Uri, string), MockRegistryBlobClient> blobClientMocks) CreateMockRegistryClients(params (Uri registryUri, string repository)[] clients)
        {
            var clientsBuilder = ImmutableDictionary.CreateBuilder<(Uri registryUri, string repository), MockRegistryBlobClient>();
            var dispatcher = ServiceBuilder.Create(s => s.WithDisabledAnalyzersConfiguration()
                .AddSingleton(BicepTestConstants.ClientFactory)
                .AddSingleton(BicepTestConstants.TemplateSpecRepositoryFactory))
                .Construct<IModuleDispatcher>();

            foreach (var client in clients)
            {
                clientsBuilder.TryAdd((client.registryUri, client.repository), new MockRegistryBlobClient());
            }

            var repoToClient = clientsBuilder.ToImmutable();

            var clientFactory = StrictMock.Of<IContainerRegistryClientFactory>();

            clientFactory
                .Setup(m => m.CreateAuthenticatedBlobClient(It.IsAny<RootConfiguration>(), It.IsAny<Uri>(), It.IsAny<string>()))
                .Returns<RootConfiguration, Uri, string>((_, registryUri, repository) =>
                {
                    if (repoToClient.TryGetValue((registryUri, repository), out var client))
                    {
                        return client;
                    }

                    throw new InvalidOperationException($"No mock authenticated client was registered for Uri '{registryUri}' and repository '{repository}'.");
                });

            clientFactory
                .Setup(m => m.CreateAnonymousBlobClient(It.IsAny<RootConfiguration>(), It.IsAny<Uri>(), It.IsAny<string>()))
                .Returns<RootConfiguration, Uri, string>((_, registryUri, repository) =>
                {
                    if (repoToClient.TryGetValue((registryUri, repository), out var client))
                    {
                        return client;
                    }

                    throw new InvalidOperationException($"No mock anonymous client was registered for Uri '{registryUri}' and repository '{repository}'.");
                });

            return (clientFactory, repoToClient);
        }

        public static ITemplateSpecRepositoryFactory CreateMockTemplateSpecRepositoryFactory(this DataSet dataSet, TestContext testContext)
        {
            var dispatcher = ServiceBuilder.Create(s => s.WithDisabledAnalyzersConfiguration()
                .AddSingleton(BicepTestConstants.ClientFactory)
                .AddSingleton(BicepTestConstants.TemplateSpecRepositoryFactory))
                .Construct<IModuleDispatcher>();
            var repositoryMocksBySubscription = new Dictionary<string, Mock<ITemplateSpecRepository>>();

            foreach (var (moduleName, templateSpecInfo) in dataSet.TemplateSpecs)
            {
                if (!dispatcher.TryGetModuleReference(templateSpecInfo.Metadata.Target, RandomFileUri(), out var @ref, out _) || @ref is not TemplateSpecModuleReference reference)
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
        {
            foreach (var (moduleName, publishInfo) in dataSet.RegistryModules)
            {
                await PublishModuleToRegistryAsync(clientFactory, moduleName, publishInfo.Metadata.Target, publishInfo.ModuleSource, publishSource, null);
            }
        }

        public static async Task PublishModuleToRegistryAsync(IContainerRegistryClientFactory clientFactory, string moduleName, string target, string moduleSource, bool publishSource, string? documentationUri = null)
        {
            var dispatcher = ServiceBuilder.Create(s => s.WithDisabledAnalyzersConfiguration()
                .AddSingleton(clientFactory)
                .AddSingleton(BicepTestConstants.TemplateSpecRepositoryFactory))
                .Construct<IModuleDispatcher>();

            var targetReference = dispatcher.TryGetModuleReference(target, RandomFileUri(), out var @ref, out _) ? @ref
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

            using Stream? sourcesStream = publishSource ? SourceArchive.PackSources(result.Compilation.SourceFileGrouping) : null;

            await dispatcher.PublishModule(targetReference, stream, sourcesStream, documentationUri);
        }

        private static Uri RandomFileUri() => PathHelper.FilePathToFileUrl(Path.GetTempFileName());
    }
}
