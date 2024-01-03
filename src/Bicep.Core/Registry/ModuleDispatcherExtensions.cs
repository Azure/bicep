// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Syntax;
using Bicep.Core.Workspaces;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;

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
