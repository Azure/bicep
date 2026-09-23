// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Semantics;

namespace Bicep.Core.Analyzers.Linter.Rules;

public sealed class UseDescriptionParametersRule : UseDescriptionRuleBase
{
    public new const string Code = "use-description-params";

    public UseDescriptionParametersRule() : base(
        code: Code,
        description: CoreResources.UseDescriptionParametersRule_Description)
    { }

    public override string FormatMessage(params object[] values)
        => string.Format(CoreResources.UseDescriptionParametersRule_MessageFormat, values);

    protected override IEnumerable<DescriptionTarget> GetTargets(SemanticModel model)
        => model.Root.ParameterDeclarations
            .Where(parameter => parameter.NameSource.IsValid)
            .Select(parameter => new DescriptionTarget(parameter.DeclaringParameter, parameter.Name, parameter.NameSource.Span));
}
