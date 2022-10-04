// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Common;
using Bicep.Core.CodeAction;
using Bicep.Core.Diagnostics;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class SecretsInParamsMustBeSecureRule : LinterRuleBase
    {
        public new const string Code = "secure-secrets-in-params";
        public const int MaxNumber = 256;

        private static readonly Regex HasSecretRegex = new("password|pwd|secret|accountkey|acctkey", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // Allow certain patterns we know about in ARM
        private static readonly Regex AllowedRegex = new(
            // secret + Permissions (keyVault secret perms is an accessPolicy property)
            "secretpermissions"
            // secret + version (url or simply the version property of a secret)
            + "|secretversion"
            // secret + url/uri
            + "|secretur[il]"
            // secret + name
            + "|secretname",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public SecretsInParamsMustBeSecureRule() : base(
            code: Code,
            description: CoreResources.SecretsInParamsRule_Description,
            docUri: new Uri($"https://aka.ms/bicep/linter/{Code}"),
            diagnosticLevel: DiagnosticLevel.Warning)
        { }

        public override string FormatMessage(params object[] values)
        {
            string paramName = (string)values[0];
            return string.Format(CoreResources.SecretsInParamsRule_MessageFormat, paramName);
        }

        override public IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model)
        {
            var diagnosticLevel = GetDiagnosticLevel(model);
            foreach (var param in model.Root.ParameterDeclarations)
            {
                if (!param.IsSecure())
                {
                    if (AnalyzeUnsecuredParameter(diagnosticLevel, param) is IDiagnostic diag)
                    {
                        yield return diag;
                    }
                }
            }
        }

        private IDiagnostic? AnalyzeUnsecuredParameter(DiagnosticLevel diagnosticLevel, ParameterSymbol parameterSymbol)
        {
            string name = parameterSymbol.Name;
            TypeSymbol type = parameterSymbol.Type;
            if (type.IsObject() || type.IsString())
            {
                if (HasSecretRegex.IsMatch(name))
                {
                    if (!AllowedRegex.IsMatch(name))
                    {
                        // Create fix
                        var decorator = SyntaxFactory.CreateDecorator("secure");
                        var decoratorText = $"{decorator.ToText()}\n";
                        var fixSpan = new TextSpan(parameterSymbol.DeclaringSyntax.Span.Position, 0);
                        var codeReplacement = new CodeReplacement(fixSpan, decoratorText);

                        return CreateFixableDiagnosticForSpan(
                            diagnosticLevel,
                            parameterSymbol.NameSyntax.Span,
                            new CodeFix("Mark parameter as secure", isPreferred: true, CodeFixKind.QuickFix, codeReplacement),
                            name);
                    }
                }
            }

            return null;
        }
    }
}
