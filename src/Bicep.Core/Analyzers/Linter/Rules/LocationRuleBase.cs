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
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public abstract class LocationRuleBase : LinterRuleBase
    {
        protected const string Global = "global";

        private const string ResourceGroupFunctionName = "resourceGroup";
        private const string DeploymentFunctionName = "deployment";
        private const string RGOrDeploymentLocationPropertyName = "location";

        public LocationRuleBase(
            string code,
            string description,
            Uri docUri,
            DiagnosticLevel diagnosticLevel = DiagnosticLevel.Warning,
            DiagnosticLabel? diagnosticLabel = null
            )
        : base(code, description, docUri, diagnosticLevel, diagnosticLabel) { }
        
        protected static (string? literalTextValue, VariableSymbol? definingVariable) TryGetLiteralText(SyntaxBase valueSyntax, SemanticModel model)
        {
            if (model.GetTypeInfo(valueSyntax) is StringLiteralType stringLiteral)
            {
                // The type of the expression is a string literal, so either we have a 
                if (valueSyntax is StringSyntax stringSyntax)
                {
                    if (!stringSyntax.IsInterpolated())
                    {
                        // Simple string literal value, e.g. 'westus'

                        Debug.Assert(stringSyntax.TryGetLiteralValue() == stringLiteral.RawStringValue);
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
                        var value = declarationSyntax.Value;
                        var nestedAssignment = TryGetLiteralText(value, model);
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

        protected static bool IsBuiltinFunctionAzCall(FunctionCallSyntaxBase syntax, params string[] expectedFunctionNames)
        {
            return IsBuiltinAzFunctionName(null, syntax.Name.IdentifierName, expectedFunctionNames);
        }

        protected static bool IsBuiltinAzFunctionCall(FunctionCallSyntaxBase syntax, params string[] expectedFunctionNames)
        {
            string? namespaceName = (syntax is InstanceFunctionCallSyntax instanceSyntax &&
                instanceSyntax.BaseExpression is VariableAccessSyntax baseExpression) ?
                baseExpression.Name.IdentifierName
                : null;
            return IsBuiltinAzFunctionName(namespaceName, syntax.Name.IdentifierName, expectedFunctionNames);
        }

        protected static bool IsBuiltinAzFunctionName(string? namespaceName, string functionName, string[] expectedFunctionNames)
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

        protected static bool IsCallToRgOrDeploymentLocation(PropertyAccessSyntax syntax, [NotNullWhen(true)] out string? actualExpression)
        {
            actualExpression = null;

            // Looking for expressions of the form:
            //
            //   [az.]deployment().location
            //   [az.]resourceGroup().location
            //
            if (LanguageConstants.IdentifierComparer.Equals(syntax.PropertyName.IdentifierName, RGOrDeploymentLocationPropertyName))
            {

                // We're accessing property 'location' on something.
                // Is that something a call to {az.,}deployment() or {az.,}resourceGroup()?
                if (syntax.BaseExpression as FunctionCallSyntaxBase is FunctionCallSyntaxBase functionCall)
                {
                    if (IsBuiltinFunctionAzCall(functionCall, DeploymentFunctionName, ResourceGroupFunctionName))
                    {
                        string rgOrDeploymentFunction = functionCall.Name.IdentifierName;
                        actualExpression = rgOrDeploymentFunction;
                        actualExpression = $"{rgOrDeploymentFunction}().{RGOrDeploymentLocationPropertyName}";
                        return true;
                    }
                }
            }

            return false;
        }

        protected static bool ContainsCallToRgOrDeploymentLocation(SyntaxBase syntax, [NotNullWhen(true)] out string? actualExpression)
        {
            var visitor = new ContainsCallToRgOrDeploymentLocationVisitor();
            visitor.Visit(syntax);
            actualExpression = visitor.ActualExpression;
            return actualExpression != null;
        }

        /// <summary>
        /// Returns the parameters defined in a consumed module's bicep file
        /// 
        /// E.g. For this consumed module declaration:
        /// 
        ///    module m1 'module1.bicep' { ... }
        ///    
        /// It retrieves the parameters defined in module1.bicep
        /// </summary>
        protected static ImmutableArray<ParameterDeclarationSyntax> TryGetParameterDefinitionsForConsumedModule(ModuleDeclarationSyntax moduleDeclarationSyntax, SemanticModel model)
        {
            if (model.Compilation.SourceFileGrouping.TryLookUpModuleSourceFile(moduleDeclarationSyntax) is BicepFile bicepFile)
            {
                return bicepFile.ProgramSyntax.Declarations.OfType<ParameterDeclarationSyntax>().ToImmutableArray();
            }

            return ImmutableArray<ParameterDeclarationSyntax>.Empty;
        }

        private static ImmutableArray<ParameterSymbol> GetParametersUsedInResourceLocationProperties(BicepFile bicepFile, SemanticModel semanticModel)
        {
            GetParametersUsedInResourceLocationsVisitor visitor = new GetParametersUsedInResourceLocationsVisitor(semanticModel);
            visitor.Visit(bicepFile.ProgramSyntax);
            return visitor.parameters.ToImmutableArray();
        }

        /// <summary>
        /// For a module, retrieves public parameters that are related to resource locations.  This is either:
        ///   The parameter default contains resourceGroup().location or deployment().location
        ///   The parameter is used inside any resource's top-level location property
        /// </summary>
        protected static ImmutableArray<string> GetLocationRelatedModuleParameters(
            ModuleDeclarationSyntax moduleDeclarationSyntax,
            SemanticModel fileSemanticModel,
            ImmutableArray<ParameterDeclarationSyntax> moduleFormalParameters,
            bool onlyParamsWithDefaultValues
        )
        {
            List<string> locationParameters = new List<string>();

            // Parameters with defaults values referencing resourceGroup().location or deployment().location
            foreach (var moduleFormalParameter in moduleFormalParameters)
            {
                SyntaxBase? defaultValue = (moduleFormalParameter.Modifier as ParameterDefaultValueSyntax)?.DefaultValue;
                if (defaultValue != null && ContainsCallToRgOrDeploymentLocation(defaultValue, out string _))
                {
                    locationParameters.Add(moduleFormalParameter.Name.IdentifierName);
                }
            }

            // Parameters used in any resource's location property
            if (fileSemanticModel.Compilation.SourceFileGrouping.TryLookUpModuleSourceFile(moduleDeclarationSyntax) is BicepFile bicepFile)
            {
                if (fileSemanticModel.Compilation.GetSemanticModel(bicepFile) is SemanticModel moduleSemanticModel)
                {
                    ImmutableArray<ParameterSymbol> parametersUsedInResourceLocationProperties =
                        GetParametersUsedInResourceLocationProperties(bicepFile, moduleSemanticModel).ToImmutableArray();
                    foreach (var moduleFormalParameter in parametersUsedInResourceLocationProperties)
                    {
                        // No duplicates in the list
                        if (!locationParameters.Contains(moduleFormalParameter.Name))
                        {
                            if (!onlyParamsWithDefaultValues ||
                                null != (moduleFormalParameter.DeclaringParameter.Modifier as ParameterDefaultValueSyntax)?.DefaultValue)
                            {
                                locationParameters.Add(moduleFormalParameter.Name);
                            }
                        }
                    }
                }
            }

            return locationParameters.ToImmutableArray();
        }

        /// <summary>
        /// For a module declaration (consumed module), finds all values passed in to all the module's
        /// location-related parameters (via the declaration's 'params' object).
        /// </summary>
        protected ImmutableArray<(string parameterName, SyntaxBase? actualValue)> GetParameterValuesForModuleLocationParameters(
            ModuleDeclarationSyntax moduleDeclarationSyntax,
            SemanticModel model,
            bool onlyParamsWithDefaultValues)
        {
            List<(string parameterName, SyntaxBase? actualValue)> result = new List<(string, SyntaxBase?)>();

            if (moduleDeclarationSyntax.TryGetBody() is ObjectSyntax body)
            {
                ObjectSyntax? paramsObject = (body.TryGetPropertyByName(LanguageConstants.ModuleParamsPropertyName) as ObjectPropertySyntax)
                    ?.Value as ObjectSyntax;

                ImmutableArray<ParameterDeclarationSyntax> moduleFormalParameters = TryGetParameterDefinitionsForConsumedModule(moduleDeclarationSyntax, model); 
                ImmutableArray<string> moduleLocationParameters = GetLocationRelatedModuleParameters(moduleDeclarationSyntax, model, moduleFormalParameters, onlyParamsWithDefaultValues);

                foreach (var parameterName in moduleLocationParameters)
                {
                    // Look for a parameter value being passed in for the formal parameter that we found
                    // Be sure the param value we're looking for matches exactly the name/casing for the formal parameter (ordinal)
                    ObjectPropertySyntax? actualPropertyValue = paramsObject?.Properties.Where(p =>
                       LanguageConstants.IdentifierComparer.Equals(
                           (p.Key as IdentifierSyntax)?.IdentifierName,
                           parameterName))
                        .FirstOrDefault();

                    result.Add((parameterName, actualPropertyValue?.Value));
                }
            }

            return result.ToImmutableArray();
        }

        private class GetParametersUsedInResourceLocationsVisitor : SyntaxVisitor
        {
            private SemanticModel semanticModel;

            public List<ParameterSymbol> parameters = new List<ParameterSymbol>();

            public GetParametersUsedInResourceLocationsVisitor(SemanticModel semanticModel)
            {
                this.semanticModel = semanticModel;
            }

            public override void VisitResourceDeclarationSyntax(ResourceDeclarationSyntax syntax)
            {
                if (syntax.TryGetBody() is ObjectSyntax body)
                {
                    if (body.TryGetPropertyByName(LanguageConstants.ResourceLocationPropertyName)?.Value is SyntaxBase locationValueSyntax)
                    {
                        var visitor = new GetReferencedParametersVisitor(semanticModel);
                        visitor.Visit(locationValueSyntax);
                        parameters.AddRange(visitor.parameters);
                    }
                }

                base.VisitResourceDeclarationSyntax(syntax);
            }
        }

        private class GetReferencedParametersVisitor : SyntaxVisitor
        {
            private SemanticModel semanticModel;

            public List<ParameterSymbol> parameters = new List<ParameterSymbol>();

            public GetReferencedParametersVisitor(SemanticModel semanticModel)
            {
                this.semanticModel = semanticModel;
            }

            public override void VisitVariableAccessSyntax(VariableAccessSyntax syntax)
            {
                if (semanticModel.GetSymbolInfo(syntax) is ParameterSymbol paramSymbol)
                {
                    this.parameters.Add(paramSymbol);
                }

                base.VisitVariableAccessSyntax(syntax);
            }
        }

        private class ContainsCallToRgOrDeploymentLocationVisitor : SyntaxVisitor
        {
            public string? ActualExpression;

            public override void VisitPropertyAccessSyntax(PropertyAccessSyntax syntax)
            {
                if (IsCallToRgOrDeploymentLocation(syntax, out string? actualExpression))
                {
                    this.ActualExpression = actualExpression;
                    return;
                }

                base.VisitPropertyAccessSyntax(syntax);
            }
        }
    }
}