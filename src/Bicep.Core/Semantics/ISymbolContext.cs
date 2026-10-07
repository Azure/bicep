// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Registry;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;

namespace Bicep.Core.Semantics
{
    public interface ISymbolContext
    {
        ITypeManager TypeManager { get; }

        IBinder Binder { get; }

        BicepSourceFile SourceFile { get; }

        IArtifactFileLookup SourceFileLookup { get; }

        ISemanticModelLookup ModelLookup { get; }

        IArtifactReferenceFactory ArtifactReferenceFactory { get; }
    }
}
