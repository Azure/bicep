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

        ImmutableDictionary<string, ParameterMetadata> Parameters { get; }

        ImmutableDictionary<string, ExportMetadata> Exports { get; }

        ImmutableArray<OutputMetadata> Outputs { get; }

        bool HasErrors();
    }
}
