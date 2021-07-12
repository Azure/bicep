// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Bicep.Core.Semantics;
using Newtonsoft.Json;

namespace Bicep.Core.Emit
{
    public class ArmTemplateWriter : ITemplateWriter
    {
        private readonly ArmTemplateSemanticModel semanticModel;

        public ArmTemplateWriter(ArmTemplateSemanticModel semanticModel)
        {
            this.semanticModel = semanticModel;
        }

        public void Write(JsonTextWriter writer)
        {
            if (this.semanticModel.SourceFile.TemplateObject is null)
            {
                throw new InvalidOperationException($"Expected template to be non-null.");
            }

            this.semanticModel.SourceFile.TemplateObject.WriteTo(writer);
        }
    }
}
