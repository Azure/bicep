// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Semantics;

namespace Bicep.Core.Emit
{
    public class ArmTemplateWriter : ITemplateWriter
    {
        private readonly ArmTemplateSemanticModel semanticModel;

        public ArmTemplateWriter(ArmTemplateSemanticModel semanticModel)
        {
            this.semanticModel = semanticModel;
        }

        public void Write(SourceAwareJsonTextWriter writer)
        {
            if (this.semanticModel.SourceFile.TemplateObject is null)
            {
                throw new InvalidOperationException($"Expected template to be non-null.");
            }

            // a layered artifact links its nested deployments by digest, so those layers must be
            // inlined to produce a self-contained single-file template
            OciLayerInliner
                .Inline(this.semanticModel.SourceFile.TemplateObject, this.semanticModel.SourceFile)
                .WriteTo(writer);
        }
    }
}
