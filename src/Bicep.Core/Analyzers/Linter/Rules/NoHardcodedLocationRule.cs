// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.CodeAction;
using Bicep.Core.Diagnostics;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Workspaces;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
            RuleVisitor visitor = new(this, model, GetDiagnosticLevel(model));
            visitor.Visit(model.SourceFile.ProgramSyntax);
            return visitor.diagnostics;
        }

        /// <summary>
        /// Find a name starting with `baseName` that is not already used as a top-level definition
        /// </summary>
        private static string GetUnusedTopLevelName(string baseName, SemanticModel model)
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

        private void ValidateResourceLocationValue(List<IDiagnostic> diagnostics, HashSet<VariableSymbol> variablesToChangeToParam, SyntaxBase locationValueSyntax, SemanticModel model, string? moduleParameterName, DiagnosticLevel diagnosticLevel)
        {
            // Is the value a string literal (or a variable defined as a string literal)?
            (string? literalValue, VariableSymbol? definingVariable) = TryGetLiteralTextValueAndDefiningVariable(locationValueSyntax, model);
            if (literalValue == null)
            {
                // No - it's okay
                return;
            }

            if (StringComparer.OrdinalIgnoreCase.Equals(literalValue, Global))
            {
                // The string value 'global' (case-insensitive) is allowed
                return;
            }

            if (definingVariable != null)
            {
                // It's using a variable that is defined as a literal string. Suggest they change it to
                // a parameter (the error goes on the variable definition, not the resource location property)
                //
                // e.g.
                //
                // var location = 'westus'    << suggest change this to param
                // resource ... {
                //   location: location

                TextSpan errorSpan = definingVariable.NameSyntax.Span;

                // Is there already a diagnostic for this variable definition? Don't add a duplicate
                if (variablesToChangeToParam.Contains(definingVariable))
                {
                    return;
                }
                else
                {
                    variablesToChangeToParam.Add(definingVariable);
                }

                string msg = String.Format(
                    CoreResources.NoHardcodedLocation_ErrorChangeVarToParam,
                    definingVariable.Name);

                CodeFix fix = new(
                    String.Format(CoreResources.NoHardcodedLocation_FixChangeVarToParam, definingVariable.Name),
                    true,
                    CodeFixKind.QuickFix,
                    new CodeReplacement(
                        definingVariable.DeclaringSyntax.Span,
                        $"param {definingVariable.Name} string = {definingVariable.Value.ToTextPreserveFormatting()}"));
                diagnostics.Add(this.CreateFixableDiagnosticForSpan(diagnosticLevel, errorSpan, fix, msg));
            }
            else
            {
                // A plain string literal, e.g.:
                //
                // resource ... {
                //   location: 'westus'

                // Fix: Create a new parameter
                string newParamName = GetUnusedTopLevelName("location", model);
                string newDefaultValue = locationValueSyntax.ToTextPreserveFormatting();
                CodeReplacement insertNewParamDefinition = new(
                        TextSpan.TextDocumentStart,
                        $"@description('Specifies the location for resources.')\n"
                        + $"param {newParamName} string = {newDefaultValue}\n\n");
                CodeReplacement replacementWithNewParam = new(
                        locationValueSyntax.Span,
                        newParamName);
                CodeFix fixWithNewParam = new(
                    // Create new parameter '{0}' with default value {1}
                    String.Format(CoreResources.NoHardcodedLocation_FixNewParam, newParamName, newDefaultValue),
                    false, // isPreferred
                    CodeFixKind.QuickFix,
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
                var fullMessage = $"{errorMessage} {solutionMessage}";
                diagnostics.Add(CreateFixableDiagnosticForSpan(
                    diagnosticLevel,
                    locationValueSyntax.Span,
                    fixWithNewParam,
                    fullMessage));
            }
        }

        private sealed class RuleVisitor : SyntaxVisitor
        {
            public List<IDiagnostic> diagnostics = new();
            private readonly HashSet<VariableSymbol> variablesToChangeToParam = new();

            private readonly Dictionary<ISourceFile, ImmutableArray<ParameterSymbol>> cachedParamsUsedInLocationPropsForFile = new();
            private readonly NoHardcodedLocationRule parent;
            private readonly SemanticModel model;
            private readonly DiagnosticLevel diagnosticLevel;

            public RuleVisitor(NoHardcodedLocationRule parent, SemanticModel model, DiagnosticLevel diagnosticLevel)
            {
                this.parent = parent;
                this.model = model;
                this.diagnosticLevel = diagnosticLevel;
            }

            public override void VisitResourceDeclarationSyntax(ResourceDeclarationSyntax syntax)
            {
                // Check the resource's location property value
                SyntaxBase? locationValue = syntax.TryGetBody()
                   ?.TryGetPropertyByName(LanguageConstants.ResourceLocationPropertyName)?.Value;
                if (locationValue != null)
                {
                    parent.ValidateResourceLocationValue(diagnostics, variablesToChangeToParam, locationValue, model, null, diagnosticLevel);
                }

                base.VisitResourceDeclarationSyntax(syntax);
            }

            public override void VisitModuleDeclarationSyntax(ModuleDeclarationSyntax moduleDeclarationSyntax)
            {
                // Check the values passed in to any location-related parameters in a consumed module
                ImmutableArray<(string parameterName, SyntaxBase? actualValue)> locationParametersActualValues =
                    parent.GetParameterValuesForModuleLocationParameters(cachedParamsUsedInLocationPropsForFile, moduleDeclarationSyntax, model, onlyParamsWithDefaultValues: true);

                foreach (var (parameterName, actualValue) in locationParametersActualValues)
                {
                    if (actualValue != null)
                    {
                        parent.ValidateResourceLocationValue(diagnostics, variablesToChangeToParam, actualValue, model, parameterName, diagnosticLevel);
                    }
                }

                base.VisitModuleDeclarationSyntax(moduleDeclarationSyntax);
            }
        }
    }
}
