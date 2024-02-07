// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Semantics;

namespace Bicep.Core.Emit
{
    public class ArmTemplateWriter(ArmTemplateSemanticModel semanticModel) : ITemplateWriter
    {
        private readonly ArmTemplateSemanticModel semanticModel = semanticModel;

        public void Write(SourceAwareJsonTextWriter writer)
        {
            if (this.semanticModel.SourceFile.TemplateObject is null)
            {
                throw new InvalidOperationException($"Expected template to be non-null.");
            }

            this.semanticModel.SourceFile.TemplateObject.WriteTo(writer);
        }
    }
}
