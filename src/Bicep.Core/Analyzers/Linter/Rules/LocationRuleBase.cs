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
    public abstract class LocationRuleBase : LinterRuleBase  //asdfg split
    {
        //asdfg move to correct classes
        protected const string LocationParameterName = "location"; // (case-insensitive)
        protected const string DeploymentFunctionName = "deployment";
        protected const string ResourceGroupFunctionName = "resourceGroup";
        // property of deployment() or resourceGroup() that is disallowed everywhere except location param's default value
        protected const string RGOrDeploymentLocationPropertyName = "location";
        protected const string Global = "global";

        public LocationRuleBase(
            string code,
            string description,
            Uri docUri,
            DiagnosticLevel diagnosticLevel = DiagnosticLevel.Warning,
            DiagnosticLabel? diagnosticLabel = null
            )
        : base(code, description, docUri, diagnosticLevel, diagnosticLabel) { }

        //asdff comment
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
                        var nestedAssignment = TryGetLiteralText(value, model);
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

        protected static bool IsBuiltinFunctionAzCall(FunctionCallSyntax syntax, params string[] expectedFunctionNames)
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

        // asdfg can this be fooled by "var az = ..."?
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

        /// <summary>
        /// Returns the parameters defined in a consumed module's bicep file
        /// E.g. For this consumed module declaration:
        ///    module m1 'module1.bicep' { ... }
        /// Retrieves the parameters defined in module1.bicep
        /// </summary>
        protected static IEnumerable<ParameterDeclarationSyntax> TryGetConsumedModuleParameterDefinitions(ModuleDeclarationSyntax moduleDeclarationSyntax, SemanticModel model)
        {
            if (model.Compilation.SourceFileGrouping.TryLookUpModuleSourceFile(moduleDeclarationSyntax) is BicepFile bicepFile) //asdfg
            {
                return bicepFile.ProgramSyntax.Declarations.OfType<ParameterDeclarationSyntax>();
            }

            return Enumerable.Empty<ParameterDeclarationSyntax>();
        }

        //asdfg
        //protected static IEnumerable<ParameterSymbol> GetParametersUsedInResourceLocations(ModuleDeclarationSyntax moduleDeclarationSyntax, SemanticModel model)
        //{
        //    //asdfg can this throw?
        //    if (model.Compilation.SourceFileGrouping.TryLookUpModuleSourceFile(moduleDeclarationSyntax) is BicepFile bicepFile //asdfg arm files
        //        && model.Compilation.GetSemanticModel(bicepFile) is SemanticModel moduleSemanticModel)
        //    {
        //        return TryGetParametersUsedInResourceLocations(bicepFile, moduleSemanticModel);
        //    }

        //    return Enumerable.Empty<ParameterSymbol>();
        //}

        protected static List<ParameterSymbol> GetParametersUsedInResourceLocations(BicepFile bicepFile/*asdfg pass in just syntax*/, SemanticModel semanticModel)
        {
            GetParametersUsedInResourceLocationsVisitor visitor = new GetParametersUsedInResourceLocationsVisitor(semanticModel);
            visitor.Visit(bicepFile.ProgramSyntax);
            return visitor.parameters;
        }

        //asdfg comments
        protected static List<string> GetLocationRelatedParametersForModule(ModuleDeclarationSyntax moduleDeclarationSyntax, SemanticModel fileSemanticModel, IEnumerable<ParameterDeclarationSyntax> moduleFormalParameters/*asdfg not pass in?*/)
        {
            List<string> locationParameters = new List<string>();

            foreach (var moduleFormalParameter in moduleFormalParameters)
            {
                string? defaultValue = (moduleFormalParameter.Modifier as ParameterDefaultValueSyntax)?.DefaultValue?.ToText();
                if (defaultValue != null &&
                    //asdfg
                    (defaultValue.Contains("resourceGroup().location", LanguageConstants.IdentifierComparison)
                    || defaultValue.Contains("deployment().location", LanguageConstants.IdentifierComparison)))
                {
                    locationParameters.Add(moduleFormalParameter.Name.IdentifierName);
                }
            }

            if (fileSemanticModel.Compilation.SourceFileGrouping.TryLookUpModuleSourceFile(moduleDeclarationSyntax) is BicepFile bicepFile)
            {
                if (fileSemanticModel.Compilation.GetSemanticModel(bicepFile) is SemanticModel moduleSemanticModel)
                {
                    {
                        List<ParameterSymbol> parametersUsedInResourceLocationProperties = GetParametersUsedInResourceLocations(bicepFile, moduleSemanticModel);
                        foreach (var moduleFormalParameter in parametersUsedInResourceLocationProperties)
                        {
                            if (!locationParameters.Contains(moduleFormalParameter.Name))
                            {
                                string? defaultValue = (moduleFormalParameter.DeclaringParameter.Modifier as ParameterDefaultValueSyntax)?.DefaultValue?.ToText();
                                if (defaultValue != null)
                                {
                                    locationParameters.Add(moduleFormalParameter.Name);
                                }
                            }
                        }
                    }
                }
            }

            return locationParameters;
        }

        private class GetParametersUsedInResourceLocationsVisitor : SyntaxVisitor //asdfg here?
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

        private class GetReferencedParametersVisitor : SyntaxVisitor//asdfg here?
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
    }
}