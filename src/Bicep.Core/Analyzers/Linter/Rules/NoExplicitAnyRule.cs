// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;

namespace Bicep.Core.Analyzers.Linter.Rules;

public sealed class NoExplicitAnyRule : LinterRuleBase
{
    public new const string Code = "no-explicit-any";

    public NoExplicitAnyRule() : base(
        code: Code,
        description: CoreResources.NoExplicitAnyRule_Description,
        category: LinterRuleCategory.BestPractice)
    { }

    public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
    {
        foreach (var (syntax, symbol) in model.Binder.Bindings)
        {
            if (symbol is AmbientTypeSymbol ambientType &&
                ambientType.DeclaringNamespace.ExtensionName == SystemNamespaceType.BuiltInName &&
                ambientType.Name == LanguageConstants.TypeNameAny)
            {
                yield return CreateDiagnosticForSpan(diagnosticLevel, syntax.Span);
            }

            if (symbol is BuiltInNamespaceSymbol namespaceSymbol &&
                namespaceSymbol.TryGetNamespaceType()?.ExtensionName == SystemNamespaceType.BuiltInName &&
                model.Binder.GetParent(syntax) is TypePropertyAccessSyntax typePropertyAccess &&
                typePropertyAccess.PropertyName.IdentifierName == LanguageConstants.TypeNameAny)
            {
                yield return CreateDiagnosticForSpan(diagnosticLevel, syntax.Span);
            }
        }
    }
}
