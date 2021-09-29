// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure.Core;
using Bicep.Core.FileSystem;
using Bicep.Core.Modules;
using Bicep.Core.Registry;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.TypeSystem.Az;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Registry;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Workspaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;

namespace Bicep.Core.Samples
{
    public static class DataSetsExtensions
    {
        public static IEnumerable<object[]> ToDynamicTestData(this IEnumerable<DataSet> source) => source.Select(ToDynamicTestData);

        public static object[] ToDynamicTestData(this DataSet ds) => new object[] {ds};

        public static bool HasCrLfNewlines(this DataSet dataSet)
            => dataSet.Name.EndsWith("_CRLF",  StringComparison.Ordinal);
            
        public static string SaveFilesToTestDirectory(this DataSet dataSet, TestContext testContext)
            => FileHelper.SaveEmbeddedResourcesWithPathPrefix(testContext, typeof(DataSet).Assembly, dataSet.GetStreamPrefix());

        public static async Task<(Compilation compilation, string outputDirectory, Uri fileUri)> SetupPrerequisitesAndCreateCompilation(this DataSet dataSet, TestContext testContext)
        {
            var features = BicepTestConstants.CreateFeaturesProvider(testContext, registryEnabled: dataSet.HasExternalModules);
            var outputDirectory = dataSet.SaveFilesToTestDirectory(testContext);
            var clientFactory = dataSet.CreateMockRegistryClients(testContext);
            await dataSet.PublishModulesToRegistryAsync(clientFactory, testContext);
            var templateSpecRepositoryFactory = dataSet.CreateMockTemplateSpecRepositoryFactory(testContext);
            var fileUri = PathHelper.FilePathToFileUrl(Path.Combine(outputDirectory, DataSet.TestFileMain));
            var dispatcher = new ModuleDispatcher(new DefaultModuleRegistryProvider(BicepTestConstants.FileResolver, clientFactory, templateSpecRepositoryFactory, features));
            var workspace = new Workspace();
            var namespaceProvider = new DefaultNamespaceProvider(new AzResourceTypeLoader(), features);
            var sourceFileGrouping = SourceFileGroupingBuilder.Build(BicepTestConstants.FileResolver, dispatcher, workspace, fileUri);
            if (await dispatcher.RestoreModules(dispatcher.GetValidModuleReferences(sourceFileGrouping.ModulesToRestore)))
            {
                sourceFileGrouping = SourceFileGroupingBuilder.Rebuild(dispatcher, workspace, sourceFileGrouping);
            }

            return (new Compilation(namespaceProvider, sourceFileGrouping, BicepTestConstants.BuiltInConfiguration), outputDirectory, fileUri);
        }

        public static IContainerRegistryClientFactory CreateMockRegistryClients(this DataSet dataSet, TestContext testContext, params (Uri registryUri, string repository)[] additionalClients)
        {
            var clientsBuilder = ImmutableDictionary.CreateBuilder<(Uri registryUri, string repository), MockRegistryBlobClient>();

            var dispatcher = new ModuleDispatcher(new DefaultModuleRegistryProvider(BicepTestConstants.FileResolver, BicepTestConstants.ClientFactory, BicepTestConstants.TemplateSpecRepositoryFactory, BicepTestConstants.CreateFeaturesProvider(testContext, registryEnabled: dataSet.HasRegistryModules)));

            foreach (var (moduleName, publishInfo) in dataSet.RegistryModules)
            {
                if(dispatcher.TryGetModuleReference(publishInfo.Metadata.Target, out _) is not OciArtifactModuleReference targetReference)
                {
                    throw new InvalidOperationException($"Module '{moduleName}' has an invalid target reference '{publishInfo.Metadata.Target}'. Specify a reference to an OCI artifact.");
                }

                Uri registryUri = new Uri($"https://{targetReference.Registry}");
                clientsBuilder.TryAdd((registryUri, targetReference.Repository), new MockRegistryBlobClient());
            }

            foreach (var additionalClient in additionalClients)
            {
                clientsBuilder.TryAdd((additionalClient.registryUri, additionalClient.repository), new MockRegistryBlobClient());
            }

            var repoToClient = clientsBuilder.ToImmutable();

            var clientFactory = new Mock<IContainerRegistryClientFactory>(MockBehavior.Strict);
            clientFactory
                .Setup(m => m.CreateBlobClient(It.IsAny<Uri>(), It.IsAny<string>(), It.IsAny<TokenCredential>()))
                .Returns<Uri, string, TokenCredential>((registryUri, repository, _) =>
                {
                    if (repoToClient.TryGetValue((registryUri, repository), out var client))
                    {
                        return client;
                    }

                    throw new InvalidOperationException($"No mock client was registered for Uri '{registryUri}' and repository '{repository}'.");
                });

            return clientFactory.Object;
        }

        public static ITemplateSpecRepositoryFactory CreateMockTemplateSpecRepositoryFactory(this DataSet dataSet, TestContext testContext)
        {
            var dispatcher = new ModuleDispatcher(new DefaultModuleRegistryProvider(BicepTestConstants.FileResolver, BicepTestConstants.ClientFactory, BicepTestConstants.TemplateSpecRepositoryFactory, BicepTestConstants.CreateFeaturesProvider(testContext, registryEnabled: dataSet.HasTemplateSpecs)));
            var repositoryMocksBySubscription = new Dictionary<(Uri? endpointUri, string subscriptionId), Mock<ITemplateSpecRepository>>();

            foreach (var (moduleName, templateSpecInfo) in dataSet.TemplateSpecs)
            {
                if(dispatcher.TryGetModuleReference(templateSpecInfo.Metadata.Target, out _) is not TemplateSpecModuleReference reference)
                {
                    throw new InvalidOperationException($"Module '{moduleName}' has an invalid target reference '{templateSpecInfo.Metadata.Target}'. Specify a reference to a template spec.");
                }

                var templateSpecElement = JsonDocument.Parse(templateSpecInfo.ModuleSource).RootElement;
                var templateSpecEntity = TemplateSpecEntity.FromJsonElement(templateSpecElement);

                repositoryMocksBySubscription.TryAdd((reference.EndpointUri, reference.SubscriptionId), StrictMock.Of<ITemplateSpecRepository>());
                repositoryMocksBySubscription[(reference.EndpointUri, reference.SubscriptionId)]
                    .Setup(x => x.FindTemplateSpecByIdAsync(reference.TemplateSpecResourceId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(templateSpecEntity);
            }

            var repositoryFactoryMock = StrictMock.Of<ITemplateSpecRepositoryFactory>();
            repositoryFactoryMock
                .Setup(x => x.CreateRepository(It.IsAny<Uri?>(), It.IsAny<string>(), It.IsAny<TokenCredential>()))
                .Returns<Uri?, string, TokenCredential>((endpointUri, subscriptionId, _) =>
                    repositoryMocksBySubscription.TryGetValue((endpointUri, subscriptionId), out var repository)
                        ? repository.Object
                        : throw new InvalidOperationException($"No mock client was registered for endpoint '{endpointUri}' and subscription '{subscriptionId}'."));

            return repositoryFactoryMock.Object;
        }

        public static async Task PublishModulesToRegistryAsync(this DataSet dataSet, IContainerRegistryClientFactory clientFactory, TestContext testContext)
        {
            var dispatcher = new ModuleDispatcher(new DefaultModuleRegistryProvider(BicepTestConstants.FileResolver, clientFactory, BicepTestConstants.TemplateSpecRepositoryFactory, BicepTestConstants.CreateFeaturesProvider(testContext, registryEnabled: dataSet.HasRegistryModules)));

            foreach (var (moduleName, publishInfo) in dataSet.RegistryModules)
            {
                var targetReference = dispatcher.TryGetModuleReference(publishInfo.Metadata.Target, out _) ?? throw new InvalidOperationException($"Module '{moduleName}' has an invalid target reference '{publishInfo.Metadata.Target}'. Specify a reference to an OCI artifact.");

                var result = CompilationHelper.Compile(publishInfo.ModuleSource);
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
                await dispatcher.PublishModule(targetReference, stream);
            }
        }
    }
}

