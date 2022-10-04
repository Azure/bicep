// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class OutputsShouldNotContainSecretsRule : LinterRuleBase
    {
        public new const string Code = "outputs-should-not-contain-secrets";

        public OutputsShouldNotContainSecretsRule() : base(
            code: Code,
            description: CoreResources.OutputsShouldNotContainSecretsRuleDescription,
            docUri: new Uri($"https://aka.ms/bicep/linter/{Code}")
        )
        {
        }

        public override string FormatMessage(params object[] values)
        {
            return string.Format(
                CoreResources.OutputsShouldNotContainSecretsMessageFormat,
                this.Description,
                values.First());
        }

        public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model)
        {
            var visitor = new OutputVisitor(this, model, GetDiagnosticLevel(model));
            visitor.Visit(model.SourceFile.ProgramSyntax);
            return visitor.diagnostics;
        }

        private class OutputVisitor : SyntaxVisitor
        {
            public List<IDiagnostic> diagnostics = new();

            private readonly OutputsShouldNotContainSecretsRule parent;
            private readonly SemanticModel model;
            private readonly DiagnosticLevel diagnosticLevel;

            public OutputVisitor(OutputsShouldNotContainSecretsRule parent, SemanticModel model, DiagnosticLevel diagnosticLevel)
            {
                this.parent = parent;
                this.model = model;
                this.diagnosticLevel = diagnosticLevel;
            }

            public override void VisitOutputDeclarationSyntax(OutputDeclarationSyntax syntax)
            {
                // Does the output name contain 'password' (suggesting it contains an actual password)?
                if (syntax.Name.IdentifierName.Contains("password", StringComparison.OrdinalIgnoreCase))
                {
                    string foundMessage = string.Format(CoreResources.OutputsShouldNotContainSecretsOutputName, syntax.Name.IdentifierName);
                    this.diagnostics.Add(parent.CreateDiagnosticForSpan(diagnosticLevel, syntax.Span, foundMessage));
                }

                var visitor = new OutputValueVisitor(this.parent, diagnostics, model, diagnosticLevel);
                visitor.Visit(syntax);

                // Note: No need to navigate deeper, don't call base
            }
        }

        private class OutputValueVisitor : SyntaxVisitor
        {
            private readonly List<IDiagnostic> diagnostics;

            private readonly OutputsShouldNotContainSecretsRule parent;
            private readonly SemanticModel model;
            private readonly DiagnosticLevel diagnosticLevel;

            public OutputValueVisitor(OutputsShouldNotContainSecretsRule parent, List<IDiagnostic> diagnostics, SemanticModel model, DiagnosticLevel diagnosticLevel)
            {
                this.parent = parent;
                this.model = model;
                this.diagnostics = diagnostics;
                this.diagnosticLevel = diagnosticLevel;
            }

            public override void VisitVariableAccessSyntax(VariableAccessSyntax syntax)
            {
                // Look for references of secure parameters, e.g.:
                //
                //   @secure()
                //   param secureParam string
                //   output badResult string = 'this is the value ${secureParam}'

                Symbol? symbol = model.GetSymbolInfo(syntax);
                if (symbol is ParameterSymbol param)
                {
                    if (param.IsSecure())
                    {
                        string foundMessage = string.Format(CoreResources.OutputsShouldNotContainSecretsSecureParam, syntax.Name.IdentifierName);
                        this.diagnostics.Add(parent.CreateDiagnosticForSpan(diagnosticLevel, syntax.Name.Span, foundMessage));
                    }
                }

                base.VisitVariableAccessSyntax(syntax);
            }

            public override void VisitFunctionCallSyntax(FunctionCallSyntax syntax)
            {
                // Look for usage of list*() stand-alone function, e.g.:
                //
                //   output badResult object = listKeys(resourceId('Microsoft.Storage/storageAccounts', 'storageName'), '2021-02-01')
                //

                if (SemanticModelHelper.TryGetFunctionInNamespace(model, AzNamespaceType.BuiltInName, syntax) is FunctionCallSyntaxBase listFunction
                    && listFunction.Name.IdentifierName.StartsWithOrdinalInsensitively(LanguageConstants.ListFunctionPrefix))
                {
                    string foundMessage = string.Format(CoreResources.OutputsShouldNotContainSecretsFunction, syntax.Name.IdentifierName);
                    this.diagnostics.Add(parent.CreateDiagnosticForSpan(diagnosticLevel, syntax.Span, foundMessage));
                }

                base.VisitFunctionCallSyntax(syntax);
            }

            public override void VisitInstanceFunctionCallSyntax(InstanceFunctionCallSyntax syntax)
            {
                if (syntax.Name.IdentifierName.StartsWithOrdinalInsensitively(LanguageConstants.ListFunctionPrefix))
                {
                    bool isFailure = false;

                    if (model.ResourceMetadata.TryLookup(syntax.BaseExpression) is { })
                    {
                        // It's a usage of a list*() member function for a resource value, e.g.:
                        //
                        //   output badResult object = stg.listKeys().keys[0].value
                        //
                        isFailure = true;
                    }
                    else if (SemanticModelHelper.TryGetFunctionInNamespace(model, AzNamespaceType.BuiltInName, syntax) is FunctionCallSyntaxBase listFunction)
                    {
                        // It's a usage of a built-in list*() function as a member of the built-in "az" module, e.g.:
                        //
                        //   output badResult object = az.listKeys(resourceId('Microsoft.Storage/storageAccounts', 'storageName'), '2021-02-01')
                        //
                        isFailure = true;
                    }

                    if (isFailure)
                    {
                        string foundMessage = string.Format(CoreResources.OutputsShouldNotContainSecretsFunction, syntax.Name.IdentifierName);
                        this.diagnostics.Add(parent.CreateDiagnosticForSpan(diagnosticLevel, syntax.Span, foundMessage));
                    }
                }

                base.VisitInstanceFunctionCallSyntax(syntax);
            }
        }
    }
}
