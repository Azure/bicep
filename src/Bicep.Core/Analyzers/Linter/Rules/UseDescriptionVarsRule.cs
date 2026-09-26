// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Semantics;

namespace Bicep.Core.Analyzers.Linter.Rules;

public sealed class UseDescriptionVarsRule : UseDescriptionRuleBase
{
    public new const string Code = "use-description-vars";

    public UseDescriptionVarsRule() : base(
        code: Code,
        description: CoreResources.UseDescriptionVarsRule_Description)
    { }

    public override string FormatMessage(params object[] values)
        => string.Format(CoreResources.UseDescriptionVarsRule_MessageFormat, values);

    protected override IEnumerable<DescriptionTarget> GetTargets(SemanticModel model)
        => model.Root.VariableDeclarations
            .Where(variable => variable.NameSource.IsValid)
            .Select(variable => new DescriptionTarget(variable.DeclaringVariable, variable.Name, variable.NameSource.Span));
}
