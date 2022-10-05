// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Configuration;
using Bicep.Core.Features;
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

        public static object[] ToDynamicTestData(this DataSet ds) => new object[] { ds };

        public static bool HasCrLfNewlines(this DataSet dataSet)
            => dataSet.Name.EndsWith("_CRLF", StringComparison.Ordinal);

        public static string SaveFilesToTestDirectory(this DataSet dataSet, TestContext testContext)
            => FileHelper.SaveEmbeddedResourcesWithPathPrefix(testContext, typeof(DataSet).Assembly, dataSet.GetStreamPrefix());

        public static async Task<(Compilation compilation, string outputDirectory, Uri fileUri)> SetupPrerequisitesAndCreateCompilation(this DataSet dataSet, TestContext testContext, IFeatureProvider? features = null)
        {
            features ??= BicepTestConstants.CreateFeatureProvider(testContext, registryEnabled: dataSet.HasExternalModules);
            var featuresFactory = IFeatureProviderFactory.WithStaticFeatureProvider(features);
            var outputDirectory = dataSet.SaveFilesToTestDirectory(testContext);
            var clientFactory = dataSet.CreateMockRegistryClients(testContext);
            await dataSet.PublishModulesToRegistryAsync(clientFactory, testContext);
            var templateSpecRepositoryFactory = dataSet.CreateMockTemplateSpecRepositoryFactory(testContext);
            var fileUri = PathHelper.FilePathToFileUrl(Path.Combine(outputDirectory, DataSet.TestFileMain));
            var configuration = BicepTestConstants.ConfigurationManager.GetConfiguration(fileUri).WithAnalyzersDisabled(BicepTestConstants.AnalyzerRulesToDisableInTests);
            var configManager = IConfigurationManager.WithStaticConfiguration(configuration);
            var dispatcher = new ModuleDispatcher(new DefaultModuleRegistryProvider(BicepTestConstants.FileResolver, clientFactory, templateSpecRepositoryFactory, featuresFactory, configManager), configManager);
            var workspace = new Workspace();
            var namespaceProvider = new DefaultNamespaceProvider(new AzResourceTypeLoader());
            var sourceFileGrouping = SourceFileGroupingBuilder.Build(BicepTestConstants.FileResolver, dispatcher, workspace, fileUri);
            if (await dispatcher.RestoreModules(dispatcher.GetValidModuleReferences(sourceFileGrouping.GetModulesToRestore())))
            {
                sourceFileGrouping = SourceFileGroupingBuilder.Rebuild(dispatcher, workspace, sourceFileGrouping);
            }

            return (new Compilation(featuresFactory, namespaceProvider, sourceFileGrouping, IConfigurationManager.WithStaticConfiguration(configuration), BicepTestConstants.ApiVersionProviderFactory, BicepTestConstants.LinterAnalyzer), outputDirectory, fileUri);
        }

        public static IContainerRegistryClientFactory CreateMockRegistryClients(this DataSet dataSet, TestContext testContext, params (Uri registryUri, string repository)[] additionalClients)
        {
            var clientsBuilder = ImmutableDictionary.CreateBuilder<(Uri registryUri, string repository), MockRegistryBlobClient>();
            var configManager = IConfigurationManager.WithStaticConfiguration(BicepTestConstants.BuiltInConfigurationWithAllAnalyzersDisabled);
            var featuresFactory = IFeatureProviderFactory.WithStaticFeatureProvider(BicepTestConstants.CreateFeatureProvider(testContext, registryEnabled: dataSet.HasRegistryModules));

            var dispatcher = new ModuleDispatcher(new DefaultModuleRegistryProvider(BicepTestConstants.FileResolver, BicepTestConstants.ClientFactory, BicepTestConstants.TemplateSpecRepositoryFactory, featuresFactory, configManager), configManager);

            foreach (var (moduleName, publishInfo) in dataSet.RegistryModules)
            {
                if (!dispatcher.TryGetModuleReference(publishInfo.Metadata.Target, RandomFileUri(), out var @ref, out _) || @ref is not OciArtifactModuleReference targetReference)
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
                .Setup(m => m.CreateAuthenticatedBlobClient(It.IsAny<RootConfiguration>(), It.IsAny<Uri>(), It.IsAny<string>()))
                .Returns<RootConfiguration, Uri, string>((_, registryUri, repository) =>
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
            var configManager = IConfigurationManager.WithStaticConfiguration(BicepTestConstants.BuiltInConfigurationWithAllAnalyzersDisabled);
            var featuresFactory = IFeatureProviderFactory.WithStaticFeatureProvider(BicepTestConstants.CreateFeatureProvider(testContext, registryEnabled: dataSet.HasRegistryModules));
            var dispatcher = new ModuleDispatcher(new DefaultModuleRegistryProvider(BicepTestConstants.FileResolver, BicepTestConstants.ClientFactory, BicepTestConstants.TemplateSpecRepositoryFactory, featuresFactory, configManager), configManager);
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

        public static async Task PublishModulesToRegistryAsync(this DataSet dataSet, IContainerRegistryClientFactory clientFactory, TestContext testContext)
        {
            var configManager = IConfigurationManager.WithStaticConfiguration(BicepTestConstants.BuiltInConfigurationWithAllAnalyzersDisabled);
            var featuresFactory = IFeatureProviderFactory.WithStaticFeatureProvider(BicepTestConstants.CreateFeatureProvider(testContext, registryEnabled: dataSet.HasRegistryModules));
            var dispatcher = new ModuleDispatcher(new DefaultModuleRegistryProvider(BicepTestConstants.FileResolver, clientFactory, BicepTestConstants.TemplateSpecRepositoryFactory, featuresFactory, configManager), configManager);

            foreach (var (moduleName, publishInfo) in dataSet.RegistryModules)
            {
                var targetReference = dispatcher.TryGetModuleReference(publishInfo.Metadata.Target, RandomFileUri(), out var @ref, out _) ? @ref : throw new InvalidOperationException($"Module '{moduleName}' has an invalid target reference '{publishInfo.Metadata.Target}'. Specify a reference to an OCI artifact.");

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

        private static Uri RandomFileUri() => PathHelper.FilePathToFileUrl(Path.GetTempFileName());
    }
}
