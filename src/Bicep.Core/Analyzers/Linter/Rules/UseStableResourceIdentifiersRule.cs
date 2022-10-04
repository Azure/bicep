// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Navigation;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class UseStableResourceIdentifiersRule : LinterRuleBase
    {
        public new const string Code = "use-stable-resource-identifiers";

        public UseStableResourceIdentifiersRule() : base(
            code: Code,
            description: CoreResources.UseStableResourceIdentifiersMessage,
            docUri: new Uri($"https://aka.ms/bicep/linter/{Code}"))
        { }

        public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model)
        {
            var diagnosticLevel = GetDiagnosticLevel(model);
            foreach (var resource in model.DeclaredResources)
            {
                foreach (var identifier in resource.Type.UniqueIdentifierProperties)
                {
                    if (resource.Symbol.TryGetBodyPropertyValue(identifier) is { } identifierSyntax)
                    {
                        var visitor = new Visitor(model);
                        identifierSyntax.Accept(visitor);
                        foreach (var (path, functionName) in visitor.PathsToNonDeterministicFunctionsUsed)
                        {
                            yield return CreateDiagnosticForSpan(diagnosticLevel, identifierSyntax.Span, resource.Symbol.Name, identifier, functionName, $"{resource.Symbol.Name}.{identifier} -> {path}");
                        }
                    }
                }
            }
        }

        public override string FormatMessage(params object[] values)
            => string.Format(CoreResources.UseStableResourceIdentifiersMessageFormat, values);

        private class Visitor : SyntaxVisitor
        {
            private static readonly IReadOnlySet<string> NonDeterministicFunctionNames = new HashSet<string>
            {
                "newGuid",
                "utcNow",
            };
            private readonly SemanticModel model;
            private readonly List<(string, string)> pathsToNonDeterministicFunctionsUsed = new();
            private readonly LinkedList<Symbol> pathSegments = new();

            internal Visitor(SemanticModel model)
            {
                this.model = model;
            }

            internal IEnumerable<(string path, string functionName)> PathsToNonDeterministicFunctionsUsed => pathsToNonDeterministicFunctionsUsed;

            public override void VisitFunctionCallSyntax(FunctionCallSyntax syntax)
            {
                if (NonDeterministicFunctionNames.Contains(syntax.Name.IdentifierName))
                {
                    pathsToNonDeterministicFunctionsUsed.Add((FormatPath(syntax.ToText()), syntax.Name.IdentifierName));
                }
                base.VisitFunctionCallSyntax(syntax);
            }

            public override void VisitVariableAccessSyntax(VariableAccessSyntax syntax)
            {
                switch (model.GetSymbolInfo(syntax))
                {
                    case ParameterSymbol @parameter:
                        if (@parameter.DeclaringParameter.Modifier is ParameterDefaultValueSyntax defaultValueSyntax)
                        {
                            pathSegments.AddLast(@parameter);
                            defaultValueSyntax.Accept(this);
                            pathSegments.RemoveLast();
                        }
                        break;
                    case VariableSymbol @variable:
                        // Variable cycles are reported on elsewhere. As far as this visitor is concerned, a cycle does not introduce nondeterminism.
                        if (pathSegments.Contains(@variable))
                        {
                            return;
                        }

                        pathSegments.AddLast(@variable);
                        @variable.DeclaringVariable.Value.Accept(this);
                        pathSegments.RemoveLast();
                        break;
                }

                base.VisitVariableAccessSyntax(syntax);
            }

            private string FormatPath(string functionCall)
            {
                var path = new StringBuilder();
                foreach (var segment in pathSegments)
                {
                    if (segment is ParameterSymbol @parameter)
                    {
                        path.Append(@parameter.Name);
                        path.Append(" (default value) -> ");
                    }
                    else if (segment is VariableSymbol @variable)
                    {
                        path.Append(@variable.Name);
                        path.Append(" -> ");
                    }
                }

                path.Append(functionCall);

                return path.ToString();
            }
        }
    }
}
