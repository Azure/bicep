// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;

namespace Bicep.Core.Analyzers.Linter.Rules;

public sealed class UseDescriptionTypeRule : UseDescriptionRuleBase
{
    public new const string Code = "use-description-types";

    public UseDescriptionTypeRule() : base(
        code: Code,
        description: CoreResources.UseDescriptionTypeRuleDescription)
    { }

    public override string FormatMessage(params object[] values)
        => string.Format(CoreResources.UseDescriptionTypeRuleMessageFormat, values);

    protected override IEnumerable<DescriptionTarget> GetTargets(SemanticModel model)
        => model.Root.TypeDeclarations
            .Where(type => type.NameSource.IsValid && !HasDiscriminator(model, type))
            .Select(type => new DescriptionTarget(type.DeclaringType, type.Name, type.NameSource.Span));

    private static bool HasDiscriminator(SemanticModel model, TypeAliasSymbol type)
        => SemanticModelHelper.TryGetDecoratorInNamespace(
            model,
            type.DeclaringType,
            SystemNamespaceType.BuiltInName,
            LanguageConstants.TypeDiscriminatorDecoratorName) is not null;
}
