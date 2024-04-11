// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.Workspaces;

namespace Bicep.Core.Semantics
{
    public class InMemoryFileSemanticModel : ISemanticModel
    {
        public InMemoryFileSemanticModel(string content)
        {
            this.SourceFile = new BicepParamFile(
                new Uri("in-memory-file://"),
                ImmutableArray<int>.Empty,
                new ProgramSyntax(Enumerable.Empty<SyntaxBase>(), SyntaxFactory.EndOfFileToken),
                EmptyDiagnosticLookup.Instance,
                EmptyDiagnosticLookup.Instance
            );
        }

        public BicepSourceFile SourceFile { get; }

        public ResourceScope TargetScope => ResourceScope.None;

        public ImmutableSortedDictionary<string, ParameterMetadata> Parameters => ImmutableSortedDictionary<string, ParameterMetadata>.Empty;

        public ImmutableSortedDictionary<string, ExportMetadata> Exports => ImmutableSortedDictionary<string, ExportMetadata>.Empty;

        public ImmutableArray<OutputMetadata> Outputs => ImmutableArray<OutputMetadata>.Empty;

        public bool HasErrors() => false;
    }
}
