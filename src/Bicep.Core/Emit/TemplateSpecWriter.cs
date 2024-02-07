// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Semantics;

namespace Bicep.Core.Emit
{
    public class TemplateSpecWriter(TemplateSpecSemanticModel semanticModel) : ITemplateWriter
    {
        private readonly TemplateSpecSemanticModel semanticModel = semanticModel;

        public void Write(SourceAwareJsonTextWriter writer)
        {
            if (this.semanticModel.SourceFile?.TemplateSpecId is not { } templateSpecId)
            {
                throw new InvalidOperationException("Expected non-null template spec ID.");
            }

            writer.WriteStartObject();
            writer.WritePropertyName("id");
            writer.WriteValue(templateSpecId);
            writer.WriteEndObject();
        }
    }
}
