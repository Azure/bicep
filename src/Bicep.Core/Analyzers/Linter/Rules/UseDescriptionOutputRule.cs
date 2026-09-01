// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Semantics;

namespace Bicep.Core.Analyzers.Linter.Rules;

public sealed class UseDescriptionOutputRule : UseDescriptionRuleBase
{
    public new const string Code = "use-description-outputs";

    public UseDescriptionOutputRule() : base(
        code: Code,
        description: CoreResources.UseDescriptionOutputRuleDescription)
    { }

    public override string FormatMessage(params object[] values)
        => string.Format(CoreResources.UseDescriptionOutputRuleMessageFormat, values);

    protected override IEnumerable<DescriptionTarget> GetTargets(SemanticModel model)
        => model.Root.OutputDeclarations
            .Where(output => output.NameSource.IsValid)
            .Select(output => new DescriptionTarget(output.DeclaringOutput, output.Name, output.NameSource.Span));
}
