// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Registry;
using Bicep.Core.SourceGraph;
using Bicep.Core.SourceLink;
using Bicep.TextFixtures.Fakes.ContainerRegistry;
using Bicep.TextFixtures.Utils.IO;

namespace Bicep.TextFixtures.Utils
{
    public class TestRegistryArtifactManager
    {
        public readonly record struct ModulePublishArguments(string ModuleArtifactId, string ModuleContent, bool WithSource = false, string? DocumentationUri = null);

        public FakeContainerRegistryClientFactory containerRegistryClientFactory;
        private readonly TestServices services;
        private readonly TestCompiler testCompiler;

        public TestRegistryArtifactManager()
        {
            this.containerRegistryClientFactory = new FakeContainerRegistryClientFactory();
            this.services = new TestServices();
            this.testCompiler = new TestCompiler(services).ConfigureServices(services => services
                .AddMockFileSystem()
                .AddContainerRegistryClientFactory(this.containerRegistryClientFactory)
                .Build());
        }

        public async Task PublishModule(string moduleArtifactId, string moduleContent, bool withSource = false, string? documentationUri = null)
        {
            var dispatcher = this.services.Get<IModuleDispatcher>();
            var sourceFileFactory = services.Get<ISourceFileFactory>();
            var dummyFile = sourceFileFactory.CreateBicepFile(TestFileUri.FromMockFileSystemPath("dummy.bicep").ToUri(), "");
            var targetReference = dispatcher.TryGetArtifactReference(dummyFile, ArtifactType.Module, moduleArtifactId).Unwrap();
            var compilationResult = await testCompiler.RestoreAndCompileMockFileSystemFile(moduleContent);

            if (compilationResult.Template is null)
            {
                throw new InvalidOperationException($"Module {moduleArtifactId} has errors.");
            }

            BinaryData? sourceArchiveData = withSource ? SourceArchive.CreateFor2(compilationResult.Compilation.SourceFileGrouping).PackIntoBinaryData() : null;
            await dispatcher.PublishModule(targetReference, BinaryData.FromString(compilationResult.Template.ToString()), sourceArchiveData, documentationUri);
        }

        public async Task PublishModules(params ModulePublishArguments[] modules)
        {
            foreach (var module in modules)
            {
                await PublishModule(module.ModuleArtifactId, module.ModuleContent, module.WithSource, module.DocumentationUri);
            }
        }

        public void RegisterContainerRegistryClientFactory(TestServices services)
        {
            services.AddContainerRegistryClientFactory(this.containerRegistryClientFactory);
        }
    }
}
