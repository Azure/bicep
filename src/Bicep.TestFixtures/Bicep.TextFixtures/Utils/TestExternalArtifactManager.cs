// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Core;
using Bicep.Core.Configuration;
using Bicep.Core.Registry;
using Bicep.Core.SourceGraph;
using Bicep.Core.SourceLink;
using Bicep.TextFixtures.Fakes.ContainerRegistry;
using Bicep.TextFixtures.Fakes.TemplateSpec;
using Bicep.TextFixtures.IO;

namespace Bicep.TextFixtures.Utils
{
    public class TestExternalArtifactManager
    {
        public readonly record struct RegistryModulePublishArguments(string ModuleArtifactId, string ModuleContent, bool WithSource = false, string? DocumentationUri = null);

        private readonly FakeContainerRegistryClientFactory containerRegistryClientFactory;
        private readonly FakeTemplateSpecRepository templateSpecRepository;
        private readonly TestServices services;
        private readonly TestCompiler testCompiler;

        public TestExternalArtifactManager()
        {
            this.containerRegistryClientFactory = new();
            this.templateSpecRepository = new();
            this.services = new();
            this.testCompiler = new TestCompiler(services).ConfigureServices(services => services
                .AddExternalArtifactManager(this)
                .AddMockFileSystem()
                .Build());
        }

        public async Task PublishRegistryModule(string moduleArtifactId, string moduleContent, bool withSource = false, string? documentationUri = null)
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

            BinaryData? sourceArchiveData = withSource ? SourceArchive.CreateFrom(compilationResult.Compilation.SourceFileGrouping).PackIntoBinaryData() : null;
            await dispatcher.PublishModule(targetReference, BinaryData.FromString(compilationResult.Template.ToString()), sourceArchiveData, documentationUri);
        }

        public async Task PublishRegistryModules(params RegistryModulePublishArguments[] modules)
        {
            foreach (var module in modules)
            {
                await PublishRegistryModule(module.ModuleArtifactId, module.ModuleContent, module.WithSource, module.DocumentationUri);
            }
        }

        public void UpsertTemplateSpec(string templateSpecId, string templateSpecContent)
        {
            this.templateSpecRepository.UpsertTemplateSpec(templateSpecId, new(templateSpecContent));
        }

        public void Register(TestServices services)
        {
            services.AddContainerRegistryClientFactory(this.containerRegistryClientFactory);
            services.AddTemplateSpecRepositoryFactory(new FakeTemplateSpecRepositoryFactory(this.templateSpecRepository));
        }
    }
}
