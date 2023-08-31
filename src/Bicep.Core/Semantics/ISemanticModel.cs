// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Semantics.Metadata;
using Bicep.Core.TypeSystem;
using System.Collections.Immutable;

namespace Bicep.Core.Semantics
{
    public interface ISemanticModel
    {
        ResourceScope TargetScope { get; }

        ImmutableDictionary<string, ParameterMetadata> Parameters { get; }

        ImmutableDictionary<string, ExportedTypeMetadata> ExportedTypes { get; }

        ImmutableArray<OutputMetadata> Outputs { get; }

        bool HasErrors();
    }
}
