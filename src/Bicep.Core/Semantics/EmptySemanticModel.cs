// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Configuration;
using Bicep.Core.Features;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.SourceGraph;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics
{
    public class EmptySemanticModel : ISemanticModel
    {
        public BicepSourceFile SourceFile => throw new NotImplementedException();

        public ResourceScope TargetScope => ResourceScope.None;

        public ImmutableSortedDictionary<string, ParameterMetadata> Parameters => ImmutableSortedDictionary<string, ParameterMetadata>.Empty;

        public ImmutableSortedDictionary<string, ExtensionMetadata> Extensions => ImmutableSortedDictionary<string, ExtensionMetadata>.Empty;

        public ImmutableSortedDictionary<string, ExportMetadata> Exports => ImmutableSortedDictionary<string, ExportMetadata>.Empty;

        public ImmutableArray<OutputMetadata> Outputs => [];

        public bool HasErrors() => false;

        public IFeatureProvider Features => ExperimentalFeaturesEnabled.AllDisabled;
    }
}
