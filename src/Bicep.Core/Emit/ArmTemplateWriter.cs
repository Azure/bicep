// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Semantics;
using System;

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

            this.semanticModel.SourceFile.TemplateObject.WriteTo(writer);
        }
    }
}
