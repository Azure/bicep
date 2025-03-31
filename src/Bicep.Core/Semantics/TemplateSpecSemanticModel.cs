// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.SourceGraph;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics
{
    public class TemplateSpecSemanticModel : ISemanticModel
    {
        private readonly ArmTemplateSemanticModel mainTemplateSemanticModel;

        public TemplateSpecSemanticModel(TemplateSpecFile sourceFile)
        {
            this.mainTemplateSemanticModel = new ArmTemplateSemanticModel(sourceFile.MainTemplateFile);
            this.SourceFile = sourceFile;
        }

        public TemplateSpecFile SourceFile { get; }

        public ResourceScope TargetScope => this.mainTemplateSemanticModel.TargetScope;

        public ImmutableSortedDictionary<string, ParameterMetadata> Parameters => this.mainTemplateSemanticModel.Parameters;

        public ImmutableSortedDictionary<string, ExtensionMetadata> Extensions => this.mainTemplateSemanticModel.Extensions;

        public ImmutableSortedDictionary<string, ExportMetadata> Exports => this.mainTemplateSemanticModel.Exports;

        public ImmutableArray<OutputMetadata> Outputs => this.mainTemplateSemanticModel.Outputs;

        public bool HasErrors() => this.SourceFile.HasErrors() || this.mainTemplateSemanticModel.HasErrors();
    }
}
