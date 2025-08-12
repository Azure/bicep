// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Registry;
using Bicep.Core.SourceGraph;
using Bicep.Core.SourceLink;
using Bicep.IO.InMemory;
using Bicep.TextFixtures.Fakes.ContainerRegistry;
using Bicep.TextFixtures.Fakes.TemplateSpec;
using Bicep.TextFixtures.IO;
using Bicep.TextFixtures.Mocks;

namespace Bicep.TextFixtures.Utils
{
    public class TestExternalArtifactManager
    {
        public record RegistryModulePublishArguments(string ModuleArtifactId, string ModuleContent, bool WithSource = false, string? DocumentationUri = null);

        private readonly FakeContainerRegistryClientFactory containerRegistryClientFactory;
        private readonly FakeTemplateSpecRepository templateSpecRepository;
        private readonly TestCompiler compiler;

        public TestExternalArtifactManager(TestCompiler? compiler = null)
        {
            this.containerRegistryClientFactory = new();
            this.templateSpecRepository = new();
            this.compiler = (compiler ?? TestCompiler.ForMockFileSystemCompilation()).ConfigureServices(services => services.AddExternalArtifactManager(this));
        }

        public async Task PublishRegistryModule(string moduleArtifactId, string moduleContent, bool withSource = false, string? documentationUri = null)
        {
            var dispatcher = this.compiler.GetService<IModuleDispatcher>();
            var sourceFileFactory = compiler.GetService<ISourceFileFactory>();
            var dummyFile = sourceFileFactory.CreateBicepFile(DummyFileHandle.Default, "");
            var targetReference = dispatcher.TryGetArtifactReference(dummyFile, ArtifactType.Module, moduleArtifactId).Unwrap();
            var compilationResult = await compiler.CompileInline(moduleContent);

            if (compilationResult.Template is null)
            {
                throw new InvalidOperationException($"Module {moduleArtifactId} has errors.");
            }

            BinaryData? sourceArchiveData = withSource ? SourceArchive.CreateFrom(compilationResult.Compilation.SourceFileGrouping).PackIntoBinaryData() : null;
            await dispatcher.PublishModule(targetReference, BinaryData.FromString(compilationResult.Template.ToString()), sourceArchiveData, documentationUri);
        }

        public async Task PublishRegistryModules(IEnumerable<RegistryModulePublishArguments> modules)
        {
            foreach (var module in modules)
            {
                await PublishRegistryModule(module.ModuleArtifactId, module.ModuleContent, module.WithSource, module.DocumentationUri);
            }
        }

        public async Task PublishRegistryModules(params RegistryModulePublishArguments[] modules) => await PublishRegistryModules(modules.AsEnumerable());

        public void UpsertTemplateSpec(string templateSpecId, string templateSpecContent)
        {
            this.templateSpecRepository.UpsertTemplateSpec(templateSpecId, new(templateSpecContent));
        }

        public void UpsertTemplateSpec(MockTemplateSpecData templateSpec) => UpsertTemplateSpec(templateSpec.Id, templateSpec.Content);

        public void UpsertTemplateSpecs(IEnumerable<MockTemplateSpecData> templateSpecs)
        {
            foreach (var templateSpec in templateSpecs)
            {
                UpsertTemplateSpec(templateSpec);
            }
        }

        public void UpsertTemplateSpecs(params MockTemplateSpecData[] templateSpecs) => UpsertTemplateSpecs(templateSpecs.AsEnumerable());

        public async Task PublishExtension(MockExtensionData extension)
        {
            var dispatcher = this.compiler.GetService<IModuleDispatcher>();
            var sourceFileFactory = compiler.GetService<ISourceFileFactory>();
            var dummyFile = sourceFileFactory.CreateBicepFile(DummyFileHandle.Default, "");
            var extensionReference = dispatcher.TryGetArtifactReference(dummyFile, ArtifactType.Extension, extension.ExtensionRepoReference).Unwrap();
            var extensionPackage = new ExtensionPackage(extension.TypesTgzData, false, []);

            await dispatcher.PublishExtension(extensionReference, extensionPackage);
        }

        public async Task PublishExtensions(IEnumerable<MockExtensionData> extensions)
        {
            foreach (var extension in extensions)
            {
                await PublishExtension(extension);
            }
        }

        public async Task PublishExtensions(params MockExtensionData[] extensions) => await PublishExtensions(extensions.AsEnumerable());

        public void Register(TestServices services)
        {
            services.AddContainerRegistryClientFactory(this.containerRegistryClientFactory);
            services.AddTemplateSpecRepositoryFactory(new FakeTemplateSpecRepositoryFactory(this.templateSpecRepository));
        }

        public IContainerRegistryClientFactory ContainerRegistryClientFactory => this.containerRegistryClientFactory;

        public ITemplateSpecRepositoryFactory TemplateSpecRepositoryFactory => new FakeTemplateSpecRepositoryFactory(this.templateSpecRepository);
    }
}
