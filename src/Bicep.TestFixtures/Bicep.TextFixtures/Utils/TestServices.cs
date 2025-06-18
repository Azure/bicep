// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Bicep.Core;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Configuration;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Auth;
using Bicep.Core.Registry.Catalog.Implementation;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.SourceGraph;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.Utils;
using Bicep.Decompiler;
using Bicep.IO.Abstraction;
using Bicep.IO.InMemory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;

namespace Bicep.TextFixtures.Utils
{
    public class TestServices : ServiceCollection
    {
        public TestServices()
        {
            // Don't register the file IO types. We are abusing the real file system for tests which
            // causes a lot of TestResults garbages. Tests should be more explicit about the file IO types they use.
            this
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
        }

        // TODO(file-io-abstraction): Remove this method when the migration to the file IO abstraction is complete.
        public TestServices AddFileSystem(IFileSystem fileSystem)
        {
            this.AddSingleton<IFileSystem>(fileSystem);
            this.AddSingleton<IFileResolver, FileResolver>();

            return this;
        }

        public TestServices AddFileExplorer(IFileExplorer fileExplorer)
        {
            this.AddSingleton<IFileExplorer>(fileExplorer);

            return this;
        }

        public T Get<T>() where T : notnull => this.BuildServiceProvider().GetRequiredService<T>();
    }
}
