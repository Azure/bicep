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
        public static IEnumerable<ModuleReference> GetValidArtifactReferences(
            this IArtifactDispatcher artifactDispatcher,
            IEnumerable<IArtifactResolutionInfo> sourceResolutionInfos)
                => sourceResolutionInfos
                .Select(t => t switch
        {
            ModuleSourceResolutionInfo m
                => artifactDispatcher.TryGetModuleReference(
                    m.ModuleDeclaration, 
                    m.ParentTemplateFile.FileUri, out var moduleRef, out _) ? moduleRef : null,
            ProviderSourceResolutionInfo p
                => artifactDispatcher.TryGetModuleReference(
                    $@"br:asilvermantestbr.azurecr.io/bicep/providers/{p.ImportDeclaration.Specification.Name}:test", 
                    p.ParentTemplateFile.FileUri, out var moduleRef, out _) ? moduleRef : null,
            _ => null
        })
        .WhereNotNull();
    }
}
