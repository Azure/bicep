// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Extensions;
using Bicep.Core.Modules;
using Bicep.Core.Workspaces;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.Registry
{
    public static class ArtifactDispatcherExtensions
    {
        public static IEnumerable<ModuleReference> GetValidModuleReferences(this IArtifactDispatcher artifactDispatcher, IEnumerable<ISourceResolutionInfo> sourceResolutionInfos) =>
            sourceResolutionInfos
                .Select(t => t switch
        {
            ModuleSourceResolutionInfo m
                => artifactDispatcher.TryGetModuleReference(m.ModuleDeclaration, m.ParentTemplateFile.FileUri, out var moduleRef, out _) ? moduleRef : null,
            ProviderSourceResolutionInfo p
                => artifactDispatcher.TryGetModuleReference($@"br:asilvermantestbr.azurecr.io/bicep/providers/az:test", p.ParentTemplateFile.FileUri, out var moduleRef, out _) ? moduleRef : null,
            _ => null
        })
        .WhereNotNull();

        public static IEnumerable<ModuleReference> GetValidModuleReferences(this IArtifactDispatcher artifactDispatcher, IEnumerable<ModuleSourceResolutionInfo> sourceResolutionInfos)
            => GetValidModuleReferences(artifactDispatcher, sourceResolutionInfos.Cast<ISourceResolutionInfo>());
    }
}
