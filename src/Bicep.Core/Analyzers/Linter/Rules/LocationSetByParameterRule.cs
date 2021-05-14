// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    // TODO: nested/modules/for loops?
    public sealed class LocationSetByParameterRule : LinterRuleBase
    {
        // From https://docs.microsoft.com/en-us/azure/azure-resource-manager/templates/test-cases#location-uses-parameter
        // Your templates should have a parameter named location. Use this parameter for setting the location of resources in your template.
        //
        // Note: In the Azure TTK, main templates and nested/linked templates are distinguished by filename, with the additional rule that a location
        // parameter in a nested/linked template may not have a default value.  That distinction is not made here.
        //
        // Users of your template may have limited regions available to them. When you hard code the resource location, users may be blocked
        // from creating a resource in that region. Users could be blocked even if you set the resource location to "[resourceGroup().location]".
        // The resource group may have been created in a region that other users can't access. Those users are blocked from using the template.
        //
        // By providing a location parameter that defaults to the resource group location, users can use the default value
        // when convenient but also specify a different location.
        //
        // Rule summary:
        //   A resource's location property must be a reference to a parameter (of any name)
        //   That parameter must:
        //     a) have no default value
        //     or b) default to the string literal 'global'
        //     or c) default to the expression resourceGroup().location

        public LocationSetByParameterRule() : base(
            code: "Location set by parameter",
            description: CoreResources.LocationSetByParameterRuleDescription,
            docUri: "https://bicep/linter/rules/BCPL1040")// TODO: setup up doc pages
        { }

        override internal IEnumerable<IBicepAnalyzerDiagnostic> AnalyzeInternal(SemanticModel model)
        {
            var spanDiagnostics = new List<TextSpan>();

            var visitor = new ResourceVisitor(spanDiagnostics, model);
            visitor.Visit(model.SyntaxTree.ProgramSyntax);

            return spanDiagnostics.Select(span => CreateDiagnosticForSpan(span));
        }

        private sealed class ResourceVisitor : SyntaxVisitor
        {
            private readonly List<TextSpan> diagnostics;
            private readonly SemanticModel model;
            private readonly LocationPropertyVisitor locationPropertyVisitor;

            public ResourceVisitor(List<TextSpan> diagnostics, SemanticModel semanticModel)
            {
                this.diagnostics = diagnostics;
                this.model = semanticModel;
                this.locationPropertyVisitor = new LocationPropertyVisitor(this.diagnostics, model);
            }

            public override void VisitResourceDeclarationSyntax(ResourceDeclarationSyntax syntax)
            {
                var topLevelPropsNamedLocation = syntax.GetBody().Properties.Where(prop => prop.NameEquals("location"));
                foreach (var prop in topLevelPropsNamedLocation)
                {
                    this.locationPropertyVisitor.Visit(prop);
                }

                base.VisitResourceDeclarationSyntax(syntax);
            }
        }

        private sealed class LocationPropertyVisitor : SyntaxVisitor
        {
            private readonly List<TextSpan> diagnostics;
            private readonly SemanticModel model;

            public LocationPropertyVisitor(List<TextSpan> diagnostics, SemanticModel semanticModel)
            {
                this.diagnostics = diagnostics;
                this.model = semanticModel;
            }

            public override void VisitObjectPropertySyntax(ObjectPropertySyntax locationPropertySyntax)
            {
                bool isValid = false;

                // Is "location" property's value must be a reference to a parameter
                if (locationPropertySyntax.Value is VariableAccessSyntax variableAccessSyntax)
                {
                    var symbolInfo = this.model.GetSymbolInfo(variableAccessSyntax);
                    if (symbolInfo is ParameterSymbol parameter)
                    {
                        // CONSIDER: We only need to check a particular parameter definition's default value as being
                        // valid once, not for every reference to it like we're currently doing
                        var defaultValue = SyntaxHelper.TryGetDefaultValue(parameter.DeclaringParameter);

                        // Default value for the param can be null...
                        if (defaultValue == null)
                        {
                            isValid = true;
                        }
                        else if (defaultValue != null)
                        {
                            // ... or string literal "global"
                            if (defaultValue is StringSyntax defaultString && defaultString.IsStringLiteral())
                            {
                                if (defaultString.TryGetLiteralValue() == "global")
                                {
                                    isValid = true;
                                }
                            }
                            // ... or expression resourceGroup().location
                            else if (defaultValue is PropertyAccessSyntax propAccessSyntax && propAccessSyntax.PropertyNameEquals("location"))
                            {
                                if (propAccessSyntax.BaseExpression is FunctionCallSyntax funcCall
                                    && funcCall.NameEquals("resourceGroup")
                                    && funcCall.Arguments.Length == 0)
                                {
                                    isValid = true;
                                }
                            }
                        }
                    }
                }

                if (!isValid)
                {
                    this.diagnostics.Add(locationPropertySyntax.Span);
                }

                // Don't call base - no need to dig deeper into the expression
            }
        }
    }
}
