// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;

namespace Bicep.Core.Analyzers.Linter.Rules;

public sealed class UseDescriptionTypePropertyRule : UseDescriptionRuleBase
{
    public new const string Code = "use-description-type-property";

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
            // Aggregating over the whole declaration also covers properties of nested object types.
            foreach (var property in SyntaxAggregator.AggregateByType<ObjectTypePropertySyntax>(type.DeclaringType))
            {
                if (property.TryGetKeyText() is { } name)
                {
                    yield return new DescriptionTarget(property, name, property.Key.Span);
                }
            }
        }
    }
}
