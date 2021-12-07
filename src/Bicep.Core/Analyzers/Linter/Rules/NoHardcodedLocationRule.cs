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

// asdfg Also worth bearing in mind that LookUpModuleSourceFile can return you an ArmTemplateFile or TemplateSpecFile (with ArmTemplateSemanticModel & TemplateSpecSemanticModel) in cases where the module is referencing a template spec or JSON template
namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class NoHardcodedLocationRule : LocationRuleBase
    {
        //asdfg var->param rule should show on the variable, not the usage

        // Sub-rules: asdfg
        //
        // 1) If a location parameter exists, it must be of type string
        // 2) Parameter location may optionally default to resourceGroup().location or deployment().location or the string 'global'.
        // 3) Outside of the default value for parameter location, the expressions resourceGroup().location and deployment().location may not be used in a template.
        // 4) Each resource's location property and each module's location parameter must be either an expression or the string 'global' (usually this expression will be the value of the location parameter)
        // 5) When consuming a module with a location parameter, the parameter must be given an explicit value (it may not be left to its default value)

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

        private void VerifyResourceLocation(List<IDiagnostic> diagnostics, SyntaxBase locationValueSyntax, SemanticModel model, string? moduleParameterName)
        {
            // no-hardcoded-resource-location asdfg
            // A resource's location should not use a hard-coded string or variable value. Please use a parameter value, an expression or the string 'global'.
            // Sub-rule 4: Each resource's location property and each module's location parameter must be either an expression or the string 'global' (usually this expression will be the value of the location parameter) asdfg

            //asdfg test: multi-line strings, interpolated strings, strings with escape chars
            (string? literalValue, VariableSymbol? definingVariable) = TryGetLiteralText(locationValueSyntax, model);

            if (literalValue == null)
            {
                return;
            }

            // The value is a string literal.  In that case, it must be the value 'global' (case-insensitive)
            if (StringComparer.OrdinalIgnoreCase.Equals(literalValue, Global))
            {
                return;
            }

            if (definingVariable != null)
            {
                // It's using a variable that is defined as a literal string.  Suggest they change it to
                // a parameter (the error goes on the variable definition, not the resource location property)
                TextSpan errorSpan = definingVariable.NameSyntax.Span;

                // Is there already a diagnostic for this variable?  Don't repeat
                if (diagnostics.Any(d => d.Code == Code/*asdfg*/ && d.Span.Equals(errorSpan)))
                {
                    return;
                }

                string msg = String.Format("A resource location should not use a hard-coded string or variable value. Change variable '{0}' into a parameter.", definingVariable.Name);
                CodeFix fix = new CodeFix(
                    $"Change variable '{definingVariable.Name}' into a parameter",
                    true,
                    //asdfg do I need to use a syntax tree?  //asdfg test replacement with @description or other
                    new CodeReplacement(
                        definingVariable.DeclaringSyntax.Span,
                        $"param {definingVariable.Name} string = {definingVariable.Value.ToTextPreserveFormatting()}"));
                diagnostics.Add(this.CreateFixableDiagnosticForSpan(errorSpan, fix, msg));

            }
            else
            {
                List<CodeFix> fixes = new List<CodeFix>();

                string newParamName = "location"; //asdfg
                CodeFix fixWithNewParam = new CodeFix(
                    $"Create new parameter 'location' with default value {literalValue}",
                    false,
                    new CodeReplacement(
                        // asdfg find best insertion spot
                        new TextSpan(0, 0),
                        $"@description('Specifies the location for resources.')\n" //asdfg localize  asdfg customize?
                        + $"param {newParamName} string = {locationValueSyntax.ToTextPreserveFormatting()}\n\n"
                    ));
                fixes.Add(fixWithNewParam);

                var preMessage =
                    moduleParameterName == null ?
                     "A resource location should not use a hard-coded string or variable value." :
                     String.Format(
                        "Parameter '{0}' may be used as a resource location in the module and should not use a hard-coded string or variable value.",
                        moduleParameterName
                    );

                var msg = preMessage + " " + String.Format(
                        //asdfg change based on scenario?
                        //asdff CoreResources.NoHardcodedLocation_ResourceLocationShouldBeExpressionOrGlobal,
                        //asdfg change to "a resource's location"
                        "Please use a parameter value, an expression, or the string '{0}'. Found: '{1}'",
                        // asdfg + " AUTO-FIXES AVAILABLE: " + (fix ?? "none"),
                        Global,
                        literalValue);
                diagnostics.Add(CreateFixableDiagnosticForSpan( //asdfg should be using NoHardCodedLocationRule code
                    locationValueSyntax.Span,
                    fixes.ToArray(),
                    msg));
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

            public override void VisitModuleDeclarationSyntax(ModuleDeclarationSyntax moduleDeclarationSyntx)
            {
                // Check the values passed in to any location-related parameters in a consumed module
                if (moduleDeclarationSyntx.TryGetBody() is ObjectSyntax body)
                {
                    IEnumerable<ParameterDeclarationSyntax> moduleFormalParameters = TryGetConsumedModuleParameterDefinitions(moduleDeclarationSyntx, model); //asdfg cache

                    //asdfg extract
                    List<string> locationParameters = GetLocationRelatedParametersForModule(moduleDeclarationSyntx, model, moduleFormalParameters);

                    foreach (var parameterName in locationParameters)
                    {
                        if (body.TryGetPropertyByName(LanguageConstants.ModuleParamsPropertyName) is ObjectPropertySyntax paramsProperty)
                        {
                            var errorSpan = paramsProperty.Key.Span;
                            if (paramsProperty.Value is ObjectSyntax paramsObject)
                            {
                                //asdfg extract
                                // Look for a parameter value being passed in for the formal parameter that we found
                                // Be sure the param value we're looking for matches exactly the name/casing for the formal parameter (ordinal)
                                ObjectPropertySyntax? actualPropertyValue = paramsObject.Properties.Where(p =>
                                   LanguageConstants.IdentifierComparer.Equals(
                                       (p.Key as IdentifierSyntax)?.IdentifierName,
                                       parameterName))
                                    .FirstOrDefault();

                                if (actualPropertyValue != null)
                                {
                                    parent.VerifyResourceLocation(diagnostics, actualPropertyValue.Value, model, parameterName);
                                }
                            }
                        }
                    }
                }

                base.VisitModuleDeclarationSyntax(moduleDeclarationSyntx);
            }

        }
    }
}