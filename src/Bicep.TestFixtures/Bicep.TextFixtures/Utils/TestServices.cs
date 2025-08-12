// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions;
using Bicep.Core;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Configuration;
using Bicep.Core.Features;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Auth;
using Bicep.Core.Registry.Catalog.Implementation;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.SourceGraph;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.Utils;
using Bicep.Decompiler;
using Bicep.IO.Abstraction;
using Microsoft.Extensions.DependencyInjection;

namespace Bicep.TextFixtures.Utils
{
    public class TestServices
    {
        private readonly IServiceCollection services;
        private IServiceProvider serviceProvider = null!;
        private bool dirty;

        public TestServices()
        {
            // Don't register the file IO types. We are abusing the real file system for tests which
            // causes a lot of TestResults garbages. Tests should be more explicit about the file IO types they use.
            this.services = new ServiceCollection()
                .AddSingleton<INamespaceProvider, NamespaceProvider>()
                .AddSingleton<IResourceTypeProviderFactory, ResourceTypeProviderFactory>()
                .AddSingleton<IContainerRegistryClientFactory, ContainerRegistryClientFactory>()
                .AddSingleton<ITemplateSpecRepositoryFactory, TemplateSpecRepositoryFactory>()
                .AddSingleton<IModuleDispatcher, ModuleDispatcher>()
                .AddSingleton<IArtifactRegistryProvider, DefaultArtifactRegistryProvider>()
                .AddSingleton<ITokenCredentialFactory, TokenCredentialFactory>()
                .AddSingleton<IEnvironment>(TestEnvironment.Default)
                .AddSingleton<IAuxiliaryFileCache, AuxiliaryFileCache>()
                .AddSingleton<IConfigurationManager, ConfigurationManager>()
                .AddSingleton<IBicepAnalyzer, LinterAnalyzer>()
                .AddSingleton<IFeatureProviderFactory, FeatureProviderFactory>()
                .AddSingleton<ILinterRulesProvider, LinterRulesProvider>()
                .AddSingleton<ISourceFileFactory, SourceFileFactory>()
                .AddSingleton<IWorkspace, Workspace>()
                .AddRegistryCatalogServices()
                .AddSingleton<BicepCompiler>()
                .AddSingleton<BicepDecompiler>();
            this.dirty = true;
        }

        public TestServices AddSingleton<TService>(TService implementationInstance)
            where TService : class
        {
            this.services.AddSingleton<TService>(implementationInstance);
            this.dirty = true;

            return this;
        }

        public TestServices AddSingleton<TInterface, TImpl>()
            where TInterface : class
            where TImpl : class, TInterface
        {
            this.services.AddSingleton<TInterface, TImpl>();
            this.dirty = true;

            return this;
        }

        // TODO(file-io-abstraction): Remove this method when the migration to the file IO abstraction is complete.
        public TestServices AddFileSystem(IFileSystem fileSystem)
        {
            this.services.AddSingleton<IFileSystem>(fileSystem);
            this.dirty = true;

            return this;
        }

        public TestServices AddFileExplorer(IFileExplorer fileExplorer) => this.AddSingleton(fileExplorer);

        public TestServices AddContainerRegistryClientFactory(IContainerRegistryClientFactory containerRegistryClientFactory) => this.AddSingleton(containerRegistryClientFactory);

        public TestServices AddTemplateSpecRepositoryFactory(ITemplateSpecRepositoryFactory templateSpecRepositoryFactory) => this.AddSingleton(templateSpecRepositoryFactory);

        public TestServices AddExternalArtifactManager(TestExternalArtifactManager artifactManager)
        {
            artifactManager.Register(this);

            return this;
        }

        public T Get<T>() where T : notnull
        {
            if (this.dirty)
            {
                this.serviceProvider = this.services.BuildServiceProvider();
                this.dirty = false;
            }


            return this.serviceProvider.GetRequiredService<T>();
        }
    }
}
