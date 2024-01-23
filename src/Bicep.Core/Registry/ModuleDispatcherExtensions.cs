// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Workspaces;

namespace Bicep.Core.Registry
{
    public static class ModuleDispatcherExtensions
    {
        public static IEnumerable<ArtifactReference> GetValidModuleReferences(this IModuleDispatcher moduleDispatcher, IEnumerable<ArtifactResolutionInfo> artifacts)
            => artifacts
                .Select(t => moduleDispatcher.TryGetArtifactReference(t.DeclarationSyntax, t.ParentTemplateFile.FileUri).TryUnwrap())
                .WhereNotNull();

        public static ResultWithDiagnostic<ArtifactReference> TryGetModuleReference(this IModuleDispatcher moduleDispatcher, string reference, Uri parentModuleUri)
            => moduleDispatcher.TryGetArtifactReference(ArtifactType.Module, reference, parentModuleUri);
    }
}
