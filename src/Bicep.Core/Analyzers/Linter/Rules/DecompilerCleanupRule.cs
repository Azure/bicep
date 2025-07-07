// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.RegularExpressions;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    // Mark decompiler imperfections that should be manually cleaned up
    public sealed class DecompilerCleanupRule : LinterRuleBase
    {
        public new const string Code = "decompiler-cleanup";

        private Regex regexResourceNameNeedsCleanup = new("[a-zA-Z0-9]+_resource$");
        private Regex regexVariableNameNeedsCleanup = new("[a-zA-Z0-9]+_var$");

        public DecompilerCleanupRule() : base(
            code: Code,
            description: CoreResources.DecompilerImperfectionsRule_Description,
            LinterRuleCategory.BestPractice)
        {
        }

        public override string FormatMessage(params object[] values)
        {
            return (string)values[0];
        }

        public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
        {
            // Note: Technically the decompiler allows conflict-resolution names of *_param and *_output, but I'm not aware of any
            //   way for these to actually occur in practice, since params are given first priority and outputs are in a different namespace
            // Thus, we only deal with resources and variables

            foreach (var resource in model.DeclaredResources.Where(r => r.IsAzResource))
            {
                if (resource.TryGetNameSyntax() is StringSyntax nameSyntax && resource.Symbol.Name is String name)
                {
                    if (regexResourceNameNeedsCleanup.IsMatch(name))
                    {
                        var msg =
                            string.Format(CoreResources.DecompilerImperfectionsRule_Resource, name)
                            + " "
                            + CoreResources.DecompilerImperfectionsRule_MayWantToRename;
                        yield return this.CreateDiagnosticForSpan(diagnosticLevel, nameSyntax.Span, msg);
                    }
                }
            }

            foreach (var variable in model.Root.VariableDeclarations)
            {
                if (variable.NameSource is IdentifierSyntax nameSyntax)
                {
                    if (regexVariableNameNeedsCleanup.IsMatch(nameSyntax.IdentifierName))
                    {
                        var msg =
                            string.Format(CoreResources.DecompilerImperfectionsRule_Variable, nameSyntax.IdentifierName)
                            + " "
                            + CoreResources.DecompilerImperfectionsRule_MayWantToRename;
                        yield return this.CreateDiagnosticForSpan(diagnosticLevel, nameSyntax.Span, msg);
                    }
                }
            }
        }
    }
}
