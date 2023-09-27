// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Workspaces;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Providers;
using Bicep.LanguageServer.Registry;
using OmniSharp.Extensions.LanguageServer.Protocol;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System.Collections.Generic;
using Bicep.Core.TypeSystem;
using System;
using Bicep.Core.Resources;
using System.Collections.Immutable;
using Bicep.Core.TypeSystem.Az;
using Bicep.Core.Registry;

namespace Bicep.LanguageServer;

public static class IServiceCollectionExtensions
{
    private class EmptyModuleRestoreScheduler : IModuleRestoreScheduler
    {
        public void RequestModuleRestore(ICompilationManager compilationManager, DocumentUri documentUri, IEnumerable<ArtifactResolutionInfo> references) {}

        public void Start() {}
    }

    private class EmptyArtifactRegistryProvider : IArtifactRegistryProvider
    {
        public ImmutableArray<IArtifactRegistry> Registries(Uri _) => ImmutableArray<IArtifactRegistry>.Empty;
    }

    private class EmptyPublicRegistryModuleMetadataProvider : IPublicRegistryModuleMetadataProvider
    {
        public Task<IEnumerable<PublicRegistryModule>> GetModules()
            => Task.FromResult<IEnumerable<PublicRegistryModule>>(ImmutableArray<PublicRegistryModule>.Empty);

        public Task<IEnumerable<PublicRegistryModuleVersion>> GetVersions(string modulePath)
            => Task.FromResult<IEnumerable<PublicRegistryModuleVersion>>(ImmutableArray<PublicRegistryModuleVersion>.Empty);
    }

    public static IServiceCollection RegisterStubs(this IServiceCollection services) => services
        .AddSingleton<IModuleRestoreScheduler>(new EmptyModuleRestoreScheduler())
        .AddSingleton<IArtifactRegistryProvider>(new EmptyArtifactRegistryProvider())
        .AddSingleton<IPublicRegistryModuleMetadataProvider>(new EmptyPublicRegistryModuleMetadataProvider());
}
