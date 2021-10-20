// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class NoHardcodedLocationRule : LinterRuleBase
    {
        // Sub-rules:
        //
        // 1) If a location parameter exists, it must be of type string
        // 2) Parameter location may optionally default to resourceGroup().location or deployment().location or the string 'global'.
        // 3) Outside of the default value for parameter location, the expressions resourceGroup().location and deployment().location may not be used in a template.
        // 4) Each resource's location property and each module's location parameter must be either an expression or the string 'global' (usually this expression will be the value of the location parameter)
        // 5) When consuming a module with a location parameter, the parameter must be given an explicit value (it may not be left to its default value)

        public new const string Code = "no-hardcoded-location";

        private const string LocationParameterName = "location"; // (case-insensitive)
        private const string DeploymentFunctionName = "deployment";
        private const string ResourceGroupFunctionName = "resourceGroup";
        // property of deployment() or resourceGroup() that is disallowed everywhere except location param's default value
        private const string RGOrDeploymentLocationPropertyName = "location";
        private const string GlobalLocationValue = "global";

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
            Visitor visitor = new Visitor(this, model);
            visitor.Visit(model.SourceFile.ProgramSyntax);
            return visitor.diagnostics;
        }

        private sealed class Visitor : SyntaxVisitor
        {
            public List<IDiagnostic> diagnostics = new List<IDiagnostic>();

            private NoHardcodedLocationRule parent;
            private SemanticModel model;

            public Visitor(NoHardcodedLocationRule parent, SemanticModel model)
            {
                this.parent = parent;
                this.model = model;
            }

            public override void VisitParameterDeclarationSyntax(ParameterDeclarationSyntax syntax)
            {
                if (StringComparer.OrdinalIgnoreCase.Equals(syntax.Name.IdentifierName, NoHardcodedLocationRule.LocationParameterName))
                {
                    // We found a parameter with name 'location'

                    // Sub-rule 1: If a location parameter exists, it must be of type string
                    var typeInfo = model.GetTypeInfo(syntax);
                    if ((typeInfo.Type.TypeKind != TypeSystem.TypeKind.Primitive || typeInfo.Type.Name != "string")
                        && syntax.ParameterType?.Span != null)
                    {
                        var fix = new CodeAction.CodeFix(
                            CoreResources.NoHardcodedLocation_LocationMustBeTypeString_FixDescription,
                            true, // isPreferred
                            new CodeAction.CodeReplacement(syntax.ParameterType.Span, "string"));
                        this.diagnostics.Add(
                            parent.CreateFixableDiagnosticForSpan(
                                syntax.ParameterType.Span,
                                fix,
                                CoreResources.NoHardcodedLocation_LocationMustBeTypeString));
                    }

                    // Sub-rule 2: Parameter location may optionally default to resourceGroup().location or deployment().location or the string 'global'.
                    if (syntax.Modifier is ParameterDefaultValueSyntax defaultValue)
                    {
                        var defaultValueText = defaultValue.DefaultValue.ToText();
                        if (StringComparer.OrdinalIgnoreCase.Equals(defaultValueText, $"'{GlobalLocationValue}'"))
                        {
                            // okay
                        }
                        else
                        {
                            switch (defaultValueText)
                            {
                                case "deployment().location":
                                case "az.deployment().location":
                                case "resourceGroup().location":
                                case "az.resourceGroup().location":
                                    break; // okay

                                default:
                                    // Anything else is a failure
                                    this.diagnostics.Add(
                                        parent.CreateDiagnosticForSpan(
                                            defaultValue.Span,
                                            CoreResources.NoHardcodedLocation_LocationDefaultInvalidValue));
                                    break;
                            }
                        }
                    }

                    // {deployment,resourceGroup}().location (sub-rule 3) are acceptable inside of the "location" parameter's
                    //   default value, so don't traverse into the parameter's default value any further because that would
                    //   flag those expressions.
                    return;
                }

                base.VisitParameterDeclarationSyntax(syntax);
            }

            public override void VisitModuleDeclarationSyntax(ModuleDeclarationSyntax syntax)
            {
                // Sub-rule  5: When consuming a module with a location parameter, the parameter must be given an explicit value (it may not be left to its default value)

                if (syntax.TryGetBody() is ObjectSyntax body)
                {
                    // Look for a formal parameter in the module named 'location' (case-insensitive)
                    TypeProperty? locationFormalParameter = null;

                    TypeSystem.TypeSymbol? ti = model.GetTypeInfo(syntax).UnwrapArrayType();
                    if (ti is ModuleType moduleType)
                    {
                        if (moduleType.Body is ObjectType objectType)
                        {
                            if (objectType.Properties.TryGetValue(LanguageConstants.ModuleParamsPropertyName, out TypeProperty? moduleFormalParamsType))
                            {
                                if (moduleFormalParamsType.TypeReference is ObjectType objectType1)
                                {
                                    locationFormalParameter = objectType1.Properties.Where(p => StringComparer.OrdinalIgnoreCase.Equals(p.Key, LocationParameterName)).Select(p => p.Value).FirstOrDefault();
                                }
                            }
                        }
                    }

                    if (locationFormalParameter != null)
                    {
                        // Found one.  Now look if a value is being passed in for that parameter
                        ObjectPropertySyntax? locationActualValue = null;
                        TextSpan errorSpan = syntax.Name.Span; // span will be the params key if it exists, otherwise the module name

                        if (body.SafeGetPropertyByName(LanguageConstants.ModuleParamsPropertyName) is ObjectPropertySyntax paramsProperty)
                        {
                            errorSpan = paramsProperty.Key.Span;
                            if (paramsProperty.Value is ObjectSyntax paramsObject)
                            {
                                // Look for a parameter value being passed in for the formal parameter that we found
                                // Be sure the param value we're looking for matches exactly the name/casing for the formal parameter (ordinal)
                                locationActualValue = paramsObject.Properties.Where(p =>
                                    StringComparer.Ordinal.Equals(
                                        (p.Key as IdentifierSyntax)?.IdentifierName,
                                        locationFormalParameter.Name))
                                    .FirstOrDefault();
                            }
                        }

                        if (locationActualValue == null)
                        {
                            // No value being passed in - this is a failure
                            diagnostics.Add(
                                parent.CreateDiagnosticForSpan(errorSpan,
                                    String.Format(
                                        CoreResources.NoHardcodedLocation_ModuleLocationNeedsExplicitValue,
                                        locationFormalParameter.Name,
                                        syntax.Name.IdentifierName)));
                        }
                        else
                        {
                            // A value has been passed in.  Verify it's an expression or 'global', just like for a resource's location property
                            VerifyLocationIsExpressionOrGlobal(locationActualValue.Value);
                        }
                    }
                }

                base.VisitModuleDeclarationSyntax(syntax);
            }

            public override void VisitPropertyAccessSyntax(PropertyAccessSyntax syntax)
            {
                // Sub-rule 3: Outside of the default value for parameter location, the expressions resourceGroup().location and deployment().location may not be used in a template.
                // Looking for expressions of the form:
                //
                //   [az.]deployment().location
                //   [az.]resourceGroup().location
                //
                if (LanguageConstants.IdentifierComparer.Equals(syntax.PropertyName.IdentifierName, RGOrDeploymentLocationPropertyName))
                {
                    // We're accessing property 'location' on something.
                    // Is that something a call to {az.,}deployment() or {az.,}resourceGroup()?
                    string? resourceGroupOrDeploymentFunction = syntax.BaseExpression switch
                    {
                        FunctionCallSyntax functionCall =>
                            IsBuiltinFunctionAzCall(
                                functionCall,
                                NoHardcodedLocationRule.DeploymentFunctionName,
                                NoHardcodedLocationRule.ResourceGroupFunctionName)
                            ? functionCall.Name.IdentifierName
                            : null,

                        InstanceFunctionCallSyntax instanceFunctionCall =>
                            IsBuiltinAzFunctionCall(
                                instanceFunctionCall,
                                NoHardcodedLocationRule.DeploymentFunctionName,
                                NoHardcodedLocationRule.ResourceGroupFunctionName)
                            ? instanceFunctionCall.Name.IdentifierName
                            : null,

                        _ => null
                    };

                    if (resourceGroupOrDeploymentFunction != null)
                    {
                        this.diagnostics.Add(
                            parent.CreateDiagnosticForSpan(syntax.Span,
                            String.Format(
                                CoreResources.NoHardcodedLocation_DoNotUseDeploymentOrResourceGroupLocation,
                                $"{resourceGroupOrDeploymentFunction}().{RGOrDeploymentLocationPropertyName}"
                                )
                            ));
                    }
                }

                base.VisitPropertyAccessSyntax(syntax);
            }

            public override void VisitResourceDeclarationSyntax(ResourceDeclarationSyntax syntax)
            {
                // Verify resource's location property is valid
                SyntaxBase? locationValue = syntax.TryGetBody()
                    ?.SafeGetPropertyByName(LanguageConstants.ResourceLocationPropertyName)?.Value;
                if (locationValue != null)
                {
                    VerifyLocationIsExpressionOrGlobal(locationValue);
                }

                base.VisitResourceDeclarationSyntax(syntax);
            }

            private void VerifyLocationIsExpressionOrGlobal(SyntaxBase locationValueSyntax)
            {
                // Sub-rule 4: Each resource's location property and each module's location parameter must be either an expression or the string 'global' (usually this expression will be the value of the location parameter)
                if (locationValueSyntax != null && model.GetTypeInfo(locationValueSyntax) is StringLiteralType literalType)
                {
                    // The value is a string literal.  In that case, it must be "global" (case-insensitive)
                    if (!StringComparer.OrdinalIgnoreCase.Equals(literalType.RawStringValue, GlobalLocationValue))
                    {
                        diagnostics.Add(parent.CreateDiagnosticForSpan(
                            locationValueSyntax.Span,
                            String.Format(
                                CoreResources.NoHardcodedLocation_ResourceLocationShouldBeExpressionOrGlobal,
                                GlobalLocationValue,
                                literalType.RawStringValue)
                        ));
                    }
                }
            }

            private bool IsBuiltinFunctionAzCall(FunctionCallSyntax syntax, params string[] expectedFunctionNames)
            {
                return IsBuiltinAzFunctionName(null, syntax.Name.IdentifierName, expectedFunctionNames);
            }

            private bool IsBuiltinAzFunctionCall(FunctionCallSyntaxBase syntax, params string[] expectedFunctionNames)
            {
                string? namespaceName = (syntax is InstanceFunctionCallSyntax instanceSyntax &&
                    instanceSyntax.BaseExpression is VariableAccessSyntax baseExpression) ?
                    baseExpression.Name.IdentifierName
                    : null;
                return IsBuiltinAzFunctionName(namespaceName, syntax.Name.IdentifierName, expectedFunctionNames);
            }

            private bool IsBuiltinAzFunctionName(string? namespaceName, string functionName, string[] expectedFunctionNames)
            {
                if (namespaceName == null || LanguageConstants.IdentifierComparer.Equals(namespaceName, AzNamespaceType.BuiltInName))
                {
                    foreach (string expectedFunctionName in expectedFunctionNames)
                    {
                        if (LanguageConstants.IdentifierComparer.Equals(functionName, expectedFunctionName))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }
    }
}
