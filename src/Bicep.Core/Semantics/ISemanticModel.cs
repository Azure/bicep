// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.TypeSystem;
using Bicep.Core.Semantics.Metadata;

namespace Bicep.Core.Semantics
{
    public interface ISemanticModel
    {
        ResourceScope TargetScope { get; }

        ImmutableSortedDictionary<string, ParameterMetadata> Parameters { get; }

        ImmutableSortedDictionary<string, ExportedTypeMetadata> ExportedTypes { get; }

        ImmutableArray<OutputMetadata> Outputs { get; }

        bool HasErrors();
    }
}
