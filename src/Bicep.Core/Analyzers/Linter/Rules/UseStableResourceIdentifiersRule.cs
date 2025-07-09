// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class UseStableResourceIdentifiersRule : LinterRuleBase
    {
        public new const string Code = "use-stable-resource-identifiers";

        public UseStableResourceIdentifiersRule() : base(
            code: Code,
            description: CoreResources.UseStableResourceIdentifiersMessage,
            LinterRuleCategory.PotentialCodeIssues)
        { }

        public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
        {
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

        private class Visitor : AstVisitor
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
                VisitFunctionCallBaseSyntax(syntax);
                base.VisitFunctionCallSyntax(syntax);
            }

            public override void VisitInstanceFunctionCallSyntax(InstanceFunctionCallSyntax syntax)
            {
                VisitFunctionCallBaseSyntax(syntax);
                base.VisitInstanceFunctionCallSyntax(syntax);
            }

            private void VisitFunctionCallBaseSyntax(FunctionCallSyntaxBase syntax)
            {
                if (SemanticModelHelper.TryGetFunctionInNamespace(model, SystemNamespaceType.BuiltInName, syntax) is not null &&
                    NonDeterministicFunctionNames.Contains(syntax.Name.IdentifierName))
                {
                    pathsToNonDeterministicFunctionsUsed.Add((FormatPath(syntax.ToString()), syntax.Name.IdentifierName));
                }
            }

            public override void VisitVariableAccessSyntax(VariableAccessSyntax syntax)
            {
                switch (model.GetSymbolInfo(syntax))
                {
                    case Symbol symbol when pathSegments.Contains(symbol):
                        // Symbol cycles are reported on elsewhere. As far as this visitor is concerned, a cycle does not introduce nondeterminism.
                        break;
                    case ParameterSymbol @parameter:
                        if (@parameter.DeclaringParameter.Modifier is ParameterDefaultValueSyntax defaultValueSyntax)
                        {
                            pathSegments.AddLast(@parameter);
                            defaultValueSyntax.Accept(this);
                            pathSegments.RemoveLast();
                        }
                        break;
                    case VariableSymbol @variable:
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
