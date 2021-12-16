// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.CodeAction;
using Bicep.Core.Diagnostics;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.Workspaces;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class NoHardcodedLocationRule : LocationRuleBase
    {
        // A resource's location should not use a hard-coded string or variable value. It
        // should use a parameter, an expression (but not resourceGroup().location or
        // deployment().location) or the string 'global'.

        public new const string Code = "no-hardcoded-location";

        public NoHardcodedLocationRule() : base(
            code: Code,
            description: CoreResources.NoHardcodedLocationRuleDescription,
            docUri: new Uri($"https://aka.ms/bicep/linter/{Code}"))
        {
        }

        public override string FormatMessage(params object[] values)
            => string.Format((string)values[0]);

        public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model)
        {
            RuleVisitor visitor = new RuleVisitor(this, model);
            visitor.Visit(model.SourceFile.ProgramSyntax);
            return visitor.diagnostics;
        }

        private static string GetAvailableDefinitionName(string baseName, SemanticModel model)
        {
            int increment = 1;
            while (true)
            {
                string newName = increment == 1 ? baseName : $"{baseName}{increment}";
                if (!model.Root.GetDeclarationsByName(newName).Any())
                {
                    return newName;
                }

                ++increment;
            }
        }

        private void VerifyResourceLocation(List<IDiagnostic> diagnostics, SyntaxBase locationValueSyntax, SemanticModel model, string? moduleParameterName)
        {
            (string? literalValue, VariableSymbol? definingVariable) = TryGetLiteralTextValueAndDefiningVariable(locationValueSyntax, model);
            if (literalValue == null)
            {
                return;
            }

            // The value is a string literal.

            if (StringComparer.OrdinalIgnoreCase.Equals(literalValue, Global))
            {
                // The value 'global' (case-insensitive) is allowed
                return;
            }

            if (definingVariable != null)
            {
                // It's using a variable that is defined as a literal string.  Suggest they change it to
                // a parameter (the error goes on the variable definition, not the resource location property)
                TextSpan errorSpan = definingVariable.NameSyntax.Span;

                // Is there already a diagnostic for this variable definition?  Don't repeat
                if (diagnostics.Any(d => d.Code == Code && d.Span.Equals(errorSpan)))
                {
                    return;
                }

                string msg = String.Format(
                    CoreResources.NoHardcodedLocation_ErrorChangeVarToParam,
                    definingVariable.Name);

                // Fix: change the variable into a parameter
                CodeFix fix = new CodeFix(
                    String.Format(CoreResources.NoHardcodedLocation_FixChangeVarToParam, definingVariable.Name),
                    true,
                    new CodeReplacement(
                        definingVariable.DeclaringSyntax.Span,
                        $"param {definingVariable.Name} string = {definingVariable.Value.ToTextPreserveFormatting()}"));
                diagnostics.Add(this.CreateFixableDiagnosticForSpan(errorSpan, fix, msg));
            }
            else
            {
                // Just a string literal, e.g.:
                //   location: 'westus'

                // Fix: Create a new parameter
                string newParamName = GetAvailableDefinitionName("location", model);
                string newDefaultValue = locationValueSyntax.ToTextPreserveFormatting();
                CodeReplacement insertNewParamDefinition = new CodeReplacement(
                        new TextSpan(0, 0),
                        $"@description('Specifies the location for resources.')\n"
                        + $"param {newParamName} string = {newDefaultValue}\n\n");
                CodeReplacement replacementWithNewParam = new CodeReplacement(
                        locationValueSyntax.Span,
                        newParamName);
                CodeFix fixWithNewParam = new CodeFix(
                    // Create new parameter '{0}' with default value {1}
                    String.Format(CoreResources.NoHardcodedLocation_FixNewParam, newParamName, newDefaultValue),
                    false, // isPreferred
                    replacementWithNewParam,
                    insertNewParamDefinition);

                var errorMessage =
                    moduleParameterName == null ?
                     CoreResources.NoHardcodedLocation_ErrorForResourceLocation :
                     String.Format(
                        CoreResources.NoHardcodedLocation_ErrorForModuleParam,
                        moduleParameterName
                    );
                var solutionMessage = String.Format(
                        // Please use a parameter value, an expression, or the string '{0}'. Found: '{1}'
                        CoreResources.NoHardcodedLocation_ErrorSolution,
                        Global,
                        literalValue);
                var fullMessage = errorMessage + " " + solutionMessage;
                diagnostics.Add(CreateFixableDiagnosticForSpan(
                    locationValueSyntax.Span,
                    fixWithNewParam,
                    fullMessage));
            }
        }

        private sealed class RuleVisitor : SyntaxVisitor
        {
            public List<IDiagnostic> diagnostics = new List<IDiagnostic>();

            private NoHardcodedLocationRule parent;
            private SemanticModel model;

            public RuleVisitor(NoHardcodedLocationRule parent, SemanticModel model)
            {
                this.parent = parent;
                this.model = model;
            }

            public override void VisitResourceDeclarationSyntax(ResourceDeclarationSyntax syntax)
            {
                // Check the location property provided to a resource
                SyntaxBase? locationValue = syntax.TryGetBody()
                   ?.TryGetPropertyByName(LanguageConstants.ResourceLocationPropertyName)?.Value;
                if (locationValue != null)
                {
                    parent.VerifyResourceLocation(diagnostics, locationValue, model, null);
                }

                base.VisitResourceDeclarationSyntax(syntax);
            }

            public override void VisitModuleDeclarationSyntax(ModuleDeclarationSyntax moduleDeclarationSyntax)
            {
                // Check the values passed in to any location-related parameters in a consumed module
                ImmutableArray<(string parameterName, SyntaxBase? actualValue)> locationParametersActualValues =
                    parent.GetParameterValuesForModuleLocationParameters(moduleDeclarationSyntax, model, true /*onlyParametersWithDefaultValues*/);

                foreach (var parameter in locationParametersActualValues)
                {
                    if (parameter.actualValue != null)
                    {
                        parent.VerifyResourceLocation(diagnostics, parameter.actualValue, model, parameter.parameterName);
                    }
                }

                base.VisitModuleDeclarationSyntax(moduleDeclarationSyntax);
            }
        }
    }
}