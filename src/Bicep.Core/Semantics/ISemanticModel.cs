// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Features;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics
{
    public interface ISemanticModel
    {
        ResourceScope TargetScope { get; }

        ImmutableSortedDictionary<string, ParameterMetadata> Parameters { get; }

        ImmutableSortedDictionary<string, ExtensionMetadata> Extensions { get; }

        ImmutableSortedDictionary<string, ExportMetadata> Exports { get; }

        ImmutableArray<OutputMetadata> Outputs { get; }

        bool HasErrors();

        IFeatureProvider Features { get; }
    }
}
