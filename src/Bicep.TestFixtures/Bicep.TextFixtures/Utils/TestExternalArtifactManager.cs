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
        public record RegistryModulePublishArguments(string ModuleArtifactId, string ModuleContent, bool WithSource = false, string? DocumentationUri = null);

        private readonly FakeContainerRegistryClientFactory containerRegistryClientFactory;
        private readonly FakeTemplateSpecRepository templateSpecRepository;
        private readonly TestCompiler compiler;

        public TestExternalArtifactManager()
        {
            this.containerRegistryClientFactory = new();
            this.templateSpecRepository = new();
            this.compiler = TestCompiler
                .ForMockFileSystemCompilation()
                .ConfigureServices(services => services.AddExternalArtifactManager(this));
        }

        public async Task PublishRegistryModule(string moduleArtifactId, string moduleContent, bool withSource = false, string? documentationUri = null)
        {
            var dispatcher = this.compiler.GetService<IModuleDispatcher>();
            var sourceFileFactory = compiler.GetService<ISourceFileFactory>();
            var dummyFile = sourceFileFactory.CreateBicepFile(TestFileUri.FromMockFileSystemPath("dummy.bicep").ToUri(), "");
            var targetReference = dispatcher.TryGetArtifactReference(dummyFile, ArtifactType.Module, moduleArtifactId).Unwrap();
            var compilationResult = await compiler.CompileInline(moduleContent);

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
