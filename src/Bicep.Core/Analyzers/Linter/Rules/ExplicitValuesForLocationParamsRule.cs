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
    public sealed class ExplicitValuesForLocationParamsRule : LocationRuleBase
    {
        public new const string Code = "explicit-values-for-location-params";

        public ExplicitValuesForLocationParamsRule() : base(
            code: Code,
            description: "When consuming a module, any location-related parameters that have a default value must be assigned an explicit value.",
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

            private ExplicitValuesForLocationParamsRule parent;
            private SemanticModel model;

            public Visitor(ExplicitValuesForLocationParamsRule parent, SemanticModel model)
            {
                this.parent = parent;
                this.model = model;
            }

            public override void VisitModuleDeclarationSyntax(ModuleDeclarationSyntax moduleDeclarationSyntax)
            {
                // Sub-rule  5: When consuming a module with a location parameter, the parameter must be given an explicit value (it may not be left to its default value)
                //asdfg

                if (moduleDeclarationSyntax.TryGetBody() is ObjectSyntax body)
                {
                    IEnumerable<ParameterDeclarationSyntax> moduleFormalParameters = TryGetConsumedModuleParameterDefinitions(moduleDeclarationSyntax, model);

                    //asdfg extract
                    List<string> locationParameters = GetLocationRelatedParametersForModule(moduleDeclarationSyntax, model, moduleFormalParameters);

                    foreach (var parameterName in locationParameters)
                    {
                        // Look if a value is being passed in for this parameter
                        ObjectPropertySyntax? locationActualValue = null;
                        TextSpan errorSpan = moduleDeclarationSyntax.Name.Span; // span will be the params key if it exists, otherwise the module name

                        if (body.TryGetPropertyByName(LanguageConstants.ModuleParamsPropertyName) is ObjectPropertySyntax paramsProperty)
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
                            string moduleName = moduleDeclarationSyntax.Name.IdentifierName;
                            diagnostics.Add(
                                parent.CreateDiagnosticForSpan(errorSpan,
                                    String.Format(
                                        CoreResources.NoHardcodedLocation_ModuleLocationNeedsExplicitValue,
                                        parameterName,
                                        moduleName)));
                        }
                    }
                }

                base.VisitModuleDeclarationSyntax(moduleDeclarationSyntax);
            }

            //public override void VisitResourceDeclarationSyntax(ResourceDeclarationSyntax syntax)
            //{
            //    // Verify resource's location property is valid
            //    SyntaxBase? locationValue = syntax.TryGetBody()
            //        ?.TryGetPropertyByName(LanguageConstants.ResourceLocationPropertyName)?.Value;
            //    if (locationValue != null)
            //    {
            //        parent/*asdfg*/.VerifyResourceLocation(locationValue, model);
            //    }

            //    base.VisitResourceDeclarationSyntax(syntax);
            //}

        }
    }
}