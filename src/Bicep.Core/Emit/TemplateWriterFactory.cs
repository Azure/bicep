// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Bicep.Core.Semantics;

namespace Bicep.Core.Emit
{
    public static class TemplateWriterFactory
    {
        public static ITemplateWriter CreateTemplateWriter(ISemanticModel semanticModel) => semanticModel switch
        {
            SemanticModel bicepModel => new TemplateWriter(bicepModel),
            ArmTemplateSemanticModel armTemplateModel => new ArmTemplateWriter(armTemplateModel),
            TemplateSpecSemanticModel templateSpecModel => new TemplateSpecWriter(templateSpecModel),
            _ => throw new ArgumentException($"Unknown semantic model type: \"{semanticModel.GetType()}\"."),
        };
    }
}
