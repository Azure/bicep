// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.SourceGraph;

namespace Bicep.Core.Semantics
{
    public class EmptySemanticModel : ISemanticModel
    {
        public BicepSourceFile SourceFile => throw new NotImplementedException();

        public ResourceScope TargetScope => ResourceScope.None;

        public ImmutableSortedDictionary<string, ParameterMetadata> Parameters => ImmutableSortedDictionary<string, ParameterMetadata>.Empty;

        public ImmutableSortedDictionary<string, ExportMetadata> Exports => ImmutableSortedDictionary<string, ExportMetadata>.Empty;

        public ImmutableArray<OutputMetadata> Outputs => [];

        public bool HasErrors() => false;
    }
}
