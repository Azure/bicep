// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.Features;
using Bicep.Core.SourceGraph;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics.Namespaces;

public interface INamespaceProvider
{
    IEnumerable<NamespaceResult> GetNamespaces(
        IArtifactFileLookup artifactFileLookup,
        BicepSourceFile sourceFile,
        ResourceScope targetScope);
}
