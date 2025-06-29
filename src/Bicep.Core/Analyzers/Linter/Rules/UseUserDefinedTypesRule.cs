// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.CodeAction;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Comparers;
using Bicep.Core.Syntax.Visitors;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.Analyzers.Linter.Rules;

public sealed class UseUserDefinedTypesRule : LinterRuleBase
{
    public new const string Code = "use-user-defined-types";

    public UseUserDefinedTypesRule() : base(
        code: Code,
        description: CoreResources.UseUserDefinedTypesRule_Description,
        LinterRuleCategory.BestPractice,
        // this is an optional coding standard, not something that should be enforced by default
        overrideCategoryDefaultDiagnosticLevel: DiagnosticLevel.Off)
    { }

    public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
    {
        foreach (var typeVariable in SyntaxAggregator.AggregateByType<TypeVariableAccessSyntax>(model.Root.Syntax))
        {
            if (typeVariable.NameEquals(LanguageConstants.ObjectType) ||
                typeVariable.NameEquals(LanguageConstants.ArrayType))
            {
                yield return CreateDiagnosticForSpan(diagnosticLevel, typeVariable.Span);
            }
        }
    }
}
