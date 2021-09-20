// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.TypeSystem;
using Bicep.Core.Workspaces;

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

        public ImmutableArray<TypeProperty> ParameterTypeProperties => this.mainTemplateSemanticModel.ParameterTypeProperties;

        public ImmutableArray<TypeProperty> OutputTypeProperties => this.mainTemplateSemanticModel.OutputTypeProperties;

        public bool HasErrors() => this.SourceFile.HasErrors() || this.mainTemplateSemanticModel.HasErrors();
    }
}
