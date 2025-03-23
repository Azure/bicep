// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Modules;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using Bicep.IO.Abstraction;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using static Bicep.Core.UnitTests.Utils.RegistryHelper;

namespace Bicep.Core.Samples
{
    public static class DataSetsExtensions
    {
        public static IEnumerable<object[]> ToDynamicTestData(this IEnumerable<DataSet> source) => source.Select(ToDynamicTestData);

        public static object[] ToDynamicTestData(this DataSet ds) => [ds];

        public static bool HasCrLfNewlines(this DataSet dataSet)
            => dataSet.Name.EndsWith("_CRLF", StringComparison.Ordinal);

        public static string SaveFilesToTestDirectory(this DataSet dataSet, TestContext testContext)
            => FileHelper.SaveEmbeddedResourcesWithPathPrefix(testContext, typeof(DataSet).Assembly, dataSet.GetStreamPrefix());

        public static async Task<(Compilation compilation, string outputDirectory, Uri fileUri)> SetupPrerequisitesAndCreateCompilation(this DataSet dataSet, TestContext testContext, FeatureProviderOverrides? features = null)
        {
            features ??= new(testContext, RegistryEnabled: dataSet.HasExternalModules);
            var outputDirectory = dataSet.SaveFilesToTestDirectory(testContext);
            var clientFactory = dataSet.CreateMockRegistryClients();
            await dataSet.PublishModulesToRegistryAsync(clientFactory);
            var templateSpecRepositoryFactory = dataSet.CreateMockTemplateSpecRepositoryFactory(testContext);

            var compiler = ServiceBuilder.Create(s => s.AddSingleton(templateSpecRepositoryFactory).AddSingleton(clientFactory).WithFeatureOverrides(features).WithAnalyzersCodesToDisableConfiguration(BicepTestConstants.TestAnalyzersToSkip)).GetCompiler();

            var fileUri = PathHelper.FilePathToFileUrl(Path.Combine(outputDirectory, DataSet.TestFileMain));
            var compilation = await compiler.CreateCompilation(fileUri);

            return (compilation, outputDirectory, fileUri);
        }

        public static IContainerRegistryClientFactory CreateMockRegistryClients(
            this DataSet dataSet,
            params RepoDescriptor[] additionalClients)
            => CreateMockRegistryClients(dataSet.RegistryModules, additionalClients);

        public static IContainerRegistryClientFactory CreateMockRegistryClients(
            ImmutableDictionary<string, DataSet.ExternalModuleInfo> registryModules,
            params RepoDescriptor[] additionalClients)
        {
            var services = ServiceBuilder.Create(s => s.WithDisabledAnalyzersConfiguration()
                .AddSingleton(BicepTestConstants.ClientFactory)
                .AddSingleton(BicepTestConstants.TemplateSpecRepositoryFactory));

            var dispatcher = services.Construct<IModuleDispatcher>();
            var sourceFileFactory = services.Construct<ISourceFileFactory>();
            var dummyReferencingFile = BicepTestConstants.DummyBicepFile;

            var clients = new List<RepoDescriptor>();

            foreach (var (moduleName, publishInfo) in registryModules)
            {
                var target = publishInfo.Metadata.Target;

                if (!dispatcher.TryGetArtifactReference(dummyReferencingFile, ArtifactType.Module, target).IsSuccess(out var @ref) || @ref is not OciArtifactReference targetReference)
                {
                    throw new InvalidOperationException($"Module '{moduleName}' has an invalid target reference '{target}'. Specify a reference to an OCI artifact.");
                }

                clients.Add(new(targetReference.Registry, targetReference.Repository, ["tag"]));
            }

            return RegistryHelper.CreateMockRegistryClient([.. clients, .. additionalClients]);
        }

        public static ITemplateSpecRepositoryFactory CreateEmptyTemplateSpecRepositoryFactory()
            => CreateMockTemplateSpecRepositoryFactory(ImmutableDictionary<string, DataSet.ExternalModuleInfo>.Empty);

        public static ITemplateSpecRepositoryFactory CreateMockTemplateSpecRepositoryFactory(this DataSet dataSet, TestContext _)
            => CreateMockTemplateSpecRepositoryFactory(dataSet.TemplateSpecs);

        public static ITemplateSpecRepositoryFactory CreateMockTemplateSpecRepositoryFactory(ImmutableDictionary<string, DataSet.ExternalModuleInfo> templateSpecs)
        {
            var services = ServiceBuilder.Create(s => s.WithDisabledAnalyzersConfiguration()
                .AddSingleton(BicepTestConstants.ClientFactory)
                .AddSingleton(BicepTestConstants.TemplateSpecRepositoryFactory));

            var dispatcher = services.Construct<IModuleDispatcher>();
            var sourceFileFactory = services.Construct<ISourceFileFactory>();
            var dummyReferencingFile = BicepTestConstants.DummyBicepFile;
            var repositoryMocksBySubscription = new Dictionary<string, Mock<ITemplateSpecRepository>>();

            foreach (var (moduleName, templateSpecInfo) in templateSpecs)
            {
                if (!dispatcher.TryGetArtifactReference(dummyReferencingFile, ArtifactType.Module, templateSpecInfo.Metadata.Target).IsSuccess(out var @ref) || @ref is not TemplateSpecModuleReference reference)
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
                await RegistryHelper.PublishModuleToRegistryAsync(
                    new ServiceBuilder(),
                    clientFactory,
                    BicepTestConstants.FileSystem,
                    new(publishInfo.Metadata.Target, publishInfo.ModuleSource, WithSource: publishSource, DocumentationUri: null));
            }
        }
    }
}
