// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.Features;
using Bicep.Core.TypeSystem;
using Bicep.Core.Workspaces;

namespace Bicep.Core.Semantics.Namespaces;

public interface INamespaceProvider
{
    IEnumerable<NamespaceResult> GetNamespaces(
        RootConfiguration rootConfig,
        IFeatureProvider features,
        IArtifactFileLookup artifactFileLookup,
        BicepSourceFile sourceFile,
        ResourceScope targetScope);
}
