// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.TypeSystem;
using Bicep.Core.Workspaces;

namespace Bicep.Core.Semantics
{
    public class TemplateSpecSemanticModel(TemplateSpecFile sourceFile) : ISemanticModel
    {
        private readonly ArmTemplateSemanticModel mainTemplateSemanticModel = new ArmTemplateSemanticModel(sourceFile.MainTemplateFile);

        public TemplateSpecFile SourceFile { get; } = sourceFile;

        public ResourceScope TargetScope => this.mainTemplateSemanticModel.TargetScope;

        public ImmutableSortedDictionary<string, ParameterMetadata> Parameters => this.mainTemplateSemanticModel.Parameters;

        public ImmutableSortedDictionary<string, ExportMetadata> Exports => this.mainTemplateSemanticModel.Exports;

        public ImmutableArray<OutputMetadata> Outputs => this.mainTemplateSemanticModel.Outputs;

        public bool HasErrors() => this.SourceFile.HasErrors() || this.mainTemplateSemanticModel.HasErrors();
    }
}
