// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;

namespace Bicep.Core.Analyzers.Linter.Rules;

public sealed class UseDescriptionTypePropertyRule : UseDescriptionRuleBase
{
    public new const string Code = "use-description-type-properties";

    private const string AdditionalPropertiesName = "*";

    public UseDescriptionTypePropertyRule() : base(
        code: Code,
        description: CoreResources.UseDescriptionTypePropertyRuleDescription)
    { }

    public override string FormatMessage(params object[] values)
        => string.Format(CoreResources.UseDescriptionTypePropertyRuleMessageFormat, values);

    protected override IEnumerable<DescriptionTarget> GetTargets(SemanticModel model)
    {
        foreach (var type in model.Root.TypeDeclarations)
        {
            var members = SyntaxAggregator.Aggregate(
                type.DeclaringType,
                syntax => syntax is ObjectTypePropertySyntax or ObjectTypeAdditionalPropertiesSyntax);

            foreach (var member in members)
            {
                switch (member)
                {
                    case ObjectTypePropertySyntax property when property.TryGetKeyText() is { } name:
                        yield return new DescriptionTarget(property, name, property.Key.Span);
                        break;
                    case ObjectTypeAdditionalPropertiesSyntax additionalProperties:
                        yield return new DescriptionTarget(
                            additionalProperties,
                            AdditionalPropertiesName,
                            additionalProperties.Asterisk.Span);
                        break;
                }
            }
        }
    }
}
