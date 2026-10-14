// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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

        internal static ITemplateWriter CreateTemplateWriter(ISemanticModel semanticModel, EmitterSettings settings) => semanticModel switch
        {
            SemanticModel bicepModel => new TemplateWriter(bicepModel, settings),
            _ => throw new NotSupportedException($"Emitter settings overrides are only supported for {nameof(SemanticModel)}."),
        };
    }
}
