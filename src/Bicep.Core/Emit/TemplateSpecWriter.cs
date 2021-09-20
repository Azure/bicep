// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Bicep.Core.Semantics;
using Newtonsoft.Json;

namespace Bicep.Core.Emit
{
    public class TemplateSpecWriter : ITemplateWriter
    {
        private readonly TemplateSpecSemanticModel semanticModel;

        public TemplateSpecWriter(TemplateSpecSemanticModel semanticModel)
        {
            this.semanticModel = semanticModel;
        }

        public void Write(JsonTextWriter writer)
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
