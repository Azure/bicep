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
    public sealed class NoHardcodedLocationRule : LinterRuleBase  //asdfg split
    {
        // Sub-rules: asdfg
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
                //if (StringComparer.OrdinalIgnoreCase.Equals(syntax.Name.IdentifierName, NoHardcodedLocationRule.LocationParameterName))
                //{
                // We found a parameter with name 'location'

                //// Sub-rule 1: If a location parameter exists, it must be of type string
                //var typeInfo = model.GetTypeInfo(syntax);
                //if ((typeInfo.Type.TypeKind != TypeSystem.TypeKind.Primitive || typeInfo.Type.Name != "string")
                //    && syntax.ParameterType?.Span != null)
                //{
                //    var fix = new CodeAction.CodeFix(
                //        CoreResources.NoHardcodedLocation_LocationMustBeTypeString_FixDescription,
                //        true, // isPreferred
                //        new CodeAction.CodeReplacement(syntax.ParameterType.Span, "string"));
                //    this.diagnostics.Add(
                //        parent.CreateFixableDiagnosticForSpan(
                //            syntax.ParameterType.Span,
                //            fix,
                //            CoreResources.NoHardcodedLocation_LocationMustBeTypeString));
                //}

                // Sub-rule 2: Parameter location may optionally default to resourceGroup().location or deployment().location or the string 'global'.
                //if (syntax.Modifier is ParameterDefaultValueSyntax defaultValue)
                //{
                //    var defaultValueText = defaultValue.DefaultValue.ToText();
                //    if (StringComparer.OrdinalIgnoreCase.Equals(defaultValueText, $"'{GlobalLocationValue}'"))
                //    {
                //        // okay
                //    }
                //    else
                //    {
                //        switch (defaultValueText)
                //        {
                //            case "deployment().location":
                //            case "az.deployment().location":
                //            case "resourceGroup().location":
                //            case "az.resourceGroup().location":
                //                break; // okay

                //            default:
                //                // Anything else is a failure
                //                this.diagnostics.Add(
                //                    parent.CreateDiagnosticForSpan(
                //                        defaultValue.Span,
                //                        CoreResources.NoHardcodedLocation_LocationDefaultInvalidValue));
                //                break;
                //        }
                //    }
                //}

                // {deployment,resourceGroup}().location (sub-rule 3) are acceptable inside of the "location" parameter's
                //   default value, so don't traverse into the parameter's default value any further because that would
                //   flag those expressions.
                return;

                //base.VisitParameterDeclarationSyntax(syntax);
            }

            public override void VisitModuleDeclarationSyntax(ModuleDeclarationSyntax syntax)
            {
                // Sub-rule  5: When consuming a module with a location parameter, the parameter must be given an explicit value (it may not be left to its default value)
                //asdfg

                if (syntax.TryGetBody() is ObjectSyntax body)
                {
                    IEnumerable<ParameterDeclarationSyntax> moduleFormalParameters = GetConsumedModuleParameterDefinitionsOrEmpty(syntax);
                    List<string> locationFormalParameters = new List<string>();

                    foreach (var moduleFormalParameter in moduleFormalParameters)
                    {
                        string? defaultValue = (moduleFormalParameter.Modifier as ParameterDefaultValueSyntax)?.DefaultValue?.ToText();
                        if (defaultValue != null &&
                            //asdfg
                            (defaultValue.Contains("resourceGroup().location", LanguageConstants.IdentifierComparison)
                            || defaultValue.Contains("deployment().location", LanguageConstants.IdentifierComparison)))
                        {
                            locationFormalParameters.Add(moduleFormalParameter.Name.IdentifierName);
                        }
                    }

                    foreach (var parameterName in locationFormalParameters)
                    {
                        // Look if a value is being passed in for this parameter
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
                                    LanguageConstants.IdentifierComparer.Equals(
                                        (p.Key as IdentifierSyntax)?.IdentifierName,
                                        parameterName))
                                    .FirstOrDefault();
                            }
                        }

                        if (locationActualValue == null)
                        {
                            // No value being passed in - this is a failure
                            //                            List<CodeFix> fixes = new List<CodeFix>();
                            // fixes.Add(new CodeFix(
                            //     `${parameterName}: location`,
                            //     false,
                            //     new CodeReplacement(new TextSpan(0), "TODO")
                            // ));
                            diagnostics.Add(
                                parent.CreateDiagnosticForSpan(errorSpan,
                                    String.Format(
                                        CoreResources.NoHardcodedLocation_ModuleLocationNeedsExplicitValue,
                                        parameterName,
                                        syntax.Name.IdentifierName)));
                        }
                        else
                        {
                            // A value has been passed in.  Verify it's an expression or 'global', just like for a resource's location property
                            VerifyResourceLocation(locationActualValue.Value);
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
                        string functionCall = $"{resourceGroupOrDeploymentFunction}().{RGOrDeploymentLocationPropertyName}";
                        string msg = String.Format( //asdfg fixes
                                                    //asdfg CoreResources.NoHardcodedLocation_DoNotUseDeploymentOrResourceGroupLocation,
                                "'{0}' should only be used as the default value of a parameter.",
                                functionCall
                                );
                        this.diagnostics.Add(parent.CreateDiagnosticForSpan(syntax.Span, msg));
                        ;
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
                    VerifyResourceLocation(locationValue);
                }

                base.VisitResourceDeclarationSyntax(syntax);
            }

            private void VerifyResourceLocation(SyntaxBase locationValueSyntax)
            {
                // no-hardcoded-resource-location asdfg
                // A resource's location should not use a hard-coded string or variable value. It should use a parameter value, an expression or the string 'global'.
                // Sub-rule 4: Each resource's location property and each module's location parameter must be either an expression or the string 'global' (usually this expression will be the value of the location parameter) asdfg

                //asdfg test: multi-line strings, interpolated strings, strings with escape chars
                (string? literalValue, VariableSymbol? definingVariable) = TryGetLiteralText(locationValueSyntax);

                if (literalValue != null)
                {
                    // The value is a string literal.  In that case, it must be "global" (case-insensitive)
                    if (!StringComparer.OrdinalIgnoreCase.Equals(literalValue, GlobalLocationValue))
                    {
                        List<CodeFix> fixes = new List<CodeFix>();
                        if (definingVariable != null)
                        {
                            CodeFix fix = new CodeFix(
                                $"Change variable '{definingVariable.Name}' into a parameter",
                                true,
                                //asdfg do I need to use a syntax tree?
                                new CodeReplacement(definingVariable.DeclaringSyntax.Span, $"param {definingVariable.Name} string = {definingVariable.Value.ToTextPreserveFormatting()}"));
                            fixes.Add(fix);
                        }
                        else
                        {

                            //asdfg find candidate parameters
                            //asdfg fixWithNewParam += " " + $"Change '{literalValue}' into <an existing parameter>."; //asdfg any existing param?

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
                        }

                        var msg = String.Format(
                                //asdfg change based on scenario?
                                //asdff CoreResources.NoHardcodedLocation_ResourceLocationShouldBeExpressionOrGlobal,
                                //asdfg change to "a resource's location"
                                "A resource location should not use a hard-coded string or variable value. It should use a parameter value, an expression, or the string '{0}'. Found: '{1}'",
                                // asdfg + " AUTO-FIXES AVAILABLE: " + (fix ?? "none"),
                                GlobalLocationValue,
                                literalValue);
                        diagnostics.Add(parent.CreateFixableDiagnosticForSpan(
                            locationValueSyntax.Span,
                            fixes.ToArray(),
                            msg));
                    }
                }
                else
                {
                    // Any other value or expression is acceptable
                }
            }

            //asdff comment
            private (string? literalTextValue, VariableSymbol? definingVariable) TryGetLiteralText(SyntaxBase valueSyntax)
            {
                if (model.GetTypeInfo(valueSyntax) is StringLiteralType stringLiteral)
                {
                    // The type of the expression is a string literal, so either we have a 
                    if (valueSyntax is StringSyntax stringSyntax)
                    {
                        if (!stringSyntax.IsInterpolated())
                        {
                            // Simple string literal value, e.g. 'westus'

                            Debug.Assert(stringSyntax.TryGetLiteralValue() == stringLiteral.RawStringValue); //asdfg?                                                                                                             
                            return (stringLiteral.RawStringValue, null);
                        }
                    }
                    else if (valueSyntax is VariableAccessSyntax variableAccessSyntax
                            && model.GetSymbolInfo(valueSyntax) is VariableSymbol variableSymbol)
                    {
                        var variableDefinitionAssignment = model.GetDeclaredTypeAssignment(valueSyntax);
                        if (variableDefinitionAssignment?.DeclaringSyntax is VariableDeclarationSyntax declarationSyntax)
                        {
                            // We have a variable declaration, e.g. "var a = <value>"

                            //var name = declarationSyntax.Name; // This should be same as variableSymbol.Name
                            //var namet = model.GetDeclaredType(name);
                            var value = declarationSyntax.Value;
                            //var vt = model.GetDeclaredType(value);
                            var nestedAssignment = TryGetLiteralText(value);
                            //var v3 = model.GetSymbolInfo(declarationSyntax);
                            //var v4 = model.GetSymbolInfo(value);
                            if (nestedAssignment.literalTextValue != null)
                            {
                                // We have something like:
                                //   var a = 'westus'
                                //   var b = a
                                // resource ... { location = b }
                                return (nestedAssignment.literalTextValue, nestedAssignment.definingVariable ?? variableSymbol);
                            }
                            else
                            {
                                return (null, null);
                            }
                        }
                        return (stringLiteral.RawStringValue, variableSymbol);
                    }
                }

                return (null, null);
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

            // Given a consumed module, e.g.:
            //   module m1 'module1.bicep' { ... }
            //
            // ... Returns the parameters defined in that module's bicep file (in above example module1.bicep)
            private IEnumerable<ParameterDeclarationSyntax> GetConsumedModuleParameterDefinitionsOrEmpty(ModuleDeclarationSyntax moduleDeclarationSyntax)
            {
                //if (model.GetTypeInfo(moduleDeclarationSyntax).UnwrapArrayType() is ModuleType moduleType) //asdfg remove
                //{
                //    if (moduleType.Body is ObjectType objectType)
                //    {
                //        if (objectType.Properties.TryGetValue(LanguageConstants.ModuleParamsPropertyName, out TypeProperty? moduleFormalParamsType))
                //        {
                //            if (moduleFormalParamsType.TypeReference is ObjectType moduleParamDefinitionsObject)
                //            {
                //                //return moduleParamDefinitionsObject.Properties.Values;
                //            }
                //        }
                //    }
                //}

                if (model.Compilation.SourceFileGrouping.LookUpModuleSourceFile(moduleDeclarationSyntax) is BicepFile bicepFile)
                {
                    return bicepFile.ProgramSyntax.Declarations.OfType<ParameterDeclarationSyntax>();
                }


                return Enumerable.Empty<ParameterDeclarationSyntax>();
            }
        }
    }
}