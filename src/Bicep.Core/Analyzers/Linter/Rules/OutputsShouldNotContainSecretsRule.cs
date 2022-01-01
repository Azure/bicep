// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Deployments.Core.Extensions;
using Bicep.Core.Diagnostics;
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
            var visitor = new OutputVisitor(this, model);
            visitor.Visit(model.SourceFile.ProgramSyntax);
            return visitor.diagnostics;
        }

        private class OutputVisitor : SyntaxVisitor
        {
            public List<IDiagnostic> diagnostics = new List<IDiagnostic>();

            private readonly OutputsShouldNotContainSecretsRule parent;
            private readonly SemanticModel model;

            public OutputVisitor(OutputsShouldNotContainSecretsRule parent, SemanticModel model)
            {
                this.parent = parent;
                this.model = model;
            }

            public override void VisitOutputDeclarationSyntax(OutputDeclarationSyntax syntax)
            {
                // Does the output name contain 'password' (suggesting it contains an actual password)?
                if (syntax.Name.IdentifierName.Contains("password", StringComparison.OrdinalIgnoreCase))
                {
                    string foundMessage = string.Format(CoreResources.OutputsShouldNotContainSecretsOutputName, syntax.Name.IdentifierName);
                    this.diagnostics.Add(parent.CreateDiagnosticForSpan(syntax.Span, foundMessage));
                }

                var visitor = new OutputValueVisitor(this.parent, diagnostics, model);
                visitor.Visit(syntax);

                // Note: No need to navigate deeper, don't call base
            }
        }

        private class OutputValueVisitor : SyntaxVisitor
        {
            private List<IDiagnostic> diagnostics;

            private readonly OutputsShouldNotContainSecretsRule parent;
            private readonly SemanticModel model;
            private const string ListFunctionPrefix = "list";

            public OutputValueVisitor(OutputsShouldNotContainSecretsRule parent, List<IDiagnostic> diagnostics, SemanticModel model)
            {
                this.parent = parent;
                this.model = model;
                this.diagnostics = diagnostics;
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
                        this.diagnostics.Add(parent.CreateDiagnosticForSpan(syntax.Name.Span, foundMessage));
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
                    && listFunction.Name.IdentifierName.StartsWithOrdinalInsensitively(ListFunctionPrefix))
                {
                    string foundMessage = string.Format(CoreResources.OutputsShouldNotContainSecretsFunction, syntax.Name.IdentifierName);
                    this.diagnostics.Add(parent.CreateDiagnosticForSpan(syntax.Span, foundMessage));
                }

                base.VisitFunctionCallSyntax(syntax);
            }

            public override void VisitInstanceFunctionCallSyntax(InstanceFunctionCallSyntax syntax)
            {
                if (syntax.Name.IdentifierName.StartsWithOrdinalInsensitively(ListFunctionPrefix))
                {
                    bool isFailure = false;

                    Symbol? baseSymbol = model.GetSymbolInfo(syntax.BaseExpression);
                    if (baseSymbol is ResourceSymbol resource)
                    {
                        // It's a usage of a list*() member function for a resource value, e.g.:
                        //
                        //   output badResult object = stg.listKeys().keys[0].value
                        //
                        isFailure = true;
                    }
                    else if (baseSymbol is BuiltInNamespaceSymbol)
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
                        this.diagnostics.Add(parent.CreateDiagnosticForSpan(syntax.Span, foundMessage));
                    }
                }

                base.VisitInstanceFunctionCallSyntax(syntax);
            }
        }
    }
}
