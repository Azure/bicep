// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Workspaces;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    // When consuming a module, any location-related parameters that have a default value must be assigned an explicit value.
    public sealed class ExplicitValuesForLocationParamsRule : LocationRuleBase
    {
        public new const string Code = "explicit-values-for-loc-params";

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
            Visitor visitor = new(this, model, GetDiagnosticLevel(model));
            visitor.Visit(model.SourceFile.ProgramSyntax);
            return visitor.diagnostics;
        }

        private sealed class Visitor : SyntaxVisitor
        {
            public List<IDiagnostic> diagnostics = new();

            private readonly Dictionary<ISourceFile, ImmutableArray<ParameterSymbol>> _cachedParamsUsedInLocationPropsForFile = new();
            private readonly ExplicitValuesForLocationParamsRule parent;
            private readonly SemanticModel model;
            private readonly DiagnosticLevel diagnosticLevel;

            public Visitor(ExplicitValuesForLocationParamsRule parent, SemanticModel model, DiagnosticLevel diagnosticLevel)
            {
                this.parent = parent;
                this.model = model;
                this.diagnosticLevel = diagnosticLevel;
            }

            public override void VisitModuleDeclarationSyntax(ModuleDeclarationSyntax moduleDeclarationSyntax)
            {
                // Check that explicit values are passed in to any location-related parameters in a consumed module
                ImmutableArray<(string parameterName, SyntaxBase? actualValue)> locationParametersActualValues =
                    parent.GetParameterValuesForModuleLocationParameters(_cachedParamsUsedInLocationPropsForFile, moduleDeclarationSyntax, model, onlyParamsWithDefaultValues: true);

                // Show the error on the params key if it exists, otherwise on the module name
                var moduleParamsPropertyObject = moduleDeclarationSyntax.TryGetBody()?
                    .TryGetPropertyByName(LanguageConstants.ModuleParamsPropertyName) as ObjectPropertySyntax;
                var errorSpan = moduleParamsPropertyObject?.Span ?? moduleDeclarationSyntax.Name.Span;

                foreach (var (parameterName, actualValue) in locationParametersActualValues)
                {
                    if (actualValue is null)
                    {
                        // No value being passed in - this is a failure
                        string moduleName = moduleDeclarationSyntax.Name.IdentifierName;
                        diagnostics.Add(
                            parent.CreateDiagnosticForSpan(diagnosticLevel,
                                errorSpan,
                                String.Format(
                                    CoreResources.NoHardcodedLocation_ModuleLocationNeedsExplicitValue,
                                    parameterName,
                                    moduleName)));
                    }
                }

                base.VisitModuleDeclarationSyntax(moduleDeclarationSyntax);
            }
        }
    }
}
