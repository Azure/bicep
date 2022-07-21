// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
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
            foreach (var param in model.Root.ParameterDeclarations)
            {
                if (!param.IsSecure())
                {
                    if (AnalyzeUnsecuredParameter(param) is IDiagnostic diag)
                    {
                        yield return diag;
                    }
                }
            }
        }

        //asdfg fix
        private IDiagnostic? AnalyzeUnsecuredParameter(ParameterSymbol parameterSymbol)
        {
            string name = parameterSymbol.Name;
            if (HasSecretRegex.IsMatch(name))
            {
                if (!AllowedRegex.IsMatch(name))
                {
                    return CreateDiagnosticForSpan(parameterSymbol.NameSyntax.Span, name);
                }
            }

            return null;
        }
    }
}
