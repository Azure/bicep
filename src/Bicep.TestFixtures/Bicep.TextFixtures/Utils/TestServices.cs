// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions;
using Bicep.Core;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Analyzers.Linter;
using Bicep.Core.AzureApi;
using Bicep.Core.Configuration;
using Bicep.Core.Features;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Catalog.Implementation;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.SourceGraph;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.TypeSystem.Types;
using Bicep.Core.Utils;
using Bicep.Decompiler;
using Bicep.IO.Abstraction;
using Bicep.TextFixtures.Fakes.TypeSystem;
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
                .AddSingleton<IEnvironment>(TestEnvironment.Default)
                .AddBicepCore()
                .AddBicepDecompiler()
                .AddSingleton<IActiveSourceFileSet, ActiveSourceFileSet>();
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

        public TestServices AddFileSystem(IFileSystem fileSystem) => this.AddSingleton(fileSystem);

        public TestServices AddFileExplorer(IFileExplorer fileExplorer) => this.AddSingleton(fileExplorer);

        public TestServices AddContainerRegistryClientFactory(IContainerRegistryClientFactory containerRegistryClientFactory) => this.AddSingleton(containerRegistryClientFactory);

        public TestServices AddTemplateSpecRepositoryFactory(ITemplateSpecRepositoryFactory templateSpecRepositoryFactory) => this.AddSingleton(templateSpecRepositoryFactory);

        public TestServices AddExternalArtifactManager(TestExternalArtifactManager artifactManager)
        {
            artifactManager.Register(this);

            return this;
        }

        public TestServices AddResourceTypeProviderFactory(IResourceTypeProviderFactory resourceTypeProviderFactory) => this.AddSingleton(resourceTypeProviderFactory);

        public TestServices AddAzureResourceTypes(IEnumerable<ResourceTypeComponents> resourceTypes) => this.AddResourceTypeProviderFactory(FakeResourceTypeProviderFactory.ForAzureResourceTypes(resourceTypes));

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
