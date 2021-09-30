// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Navigation;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Utils;

namespace Bicep.Core.TypeSystem
{
    public sealed class CyclicCheckVisitor : SyntaxVisitor
    {
        private readonly IReadOnlyDictionary<SyntaxBase, Symbol> bindings;

        private readonly IDictionary<DeclaredSymbol, IList<SyntaxBase>> declarationAccessDict;

        private Stack<DeclaredSymbol> currentDeclarations;

        public static ImmutableDictionary<DeclaredSymbol, ImmutableArray<DeclaredSymbol>> FindCycles(ProgramSyntax programSyntax, IReadOnlyDictionary<SyntaxBase, Symbol> bindings)
        {
            var visitor = new CyclicCheckVisitor(bindings);
            visitor.Visit(programSyntax);

            return visitor.FindCycles();
        }

        private ImmutableDictionary<DeclaredSymbol, ImmutableArray<DeclaredSymbol>> FindCycles()
        {
            var symbolGraph = declarationAccessDict
                .SelectMany(kvp => kvp.Value.Select(x => bindings[x]).OfType<DeclaredSymbol>().Select(x => (kvp.Key, x)))
                .ToLookup(x => x.Item1, x => x.Item2);

            return CycleDetector<DeclaredSymbol>.FindCycles(symbolGraph);
        }

        private CyclicCheckVisitor(IReadOnlyDictionary<SyntaxBase, Symbol> bindings)
        {
            this.bindings = bindings;
            this.declarationAccessDict = new Dictionary<DeclaredSymbol, IList<SyntaxBase>>();
            this.currentDeclarations = new Stack<DeclaredSymbol>();
        }

        private void VisitDeclaration<TDeclarationSyntax>(TDeclarationSyntax syntax, Action<TDeclarationSyntax> visitBaseFunc)
            where TDeclarationSyntax : SyntaxBase, ITopLevelNamedDeclarationSyntax
        {
            if (!bindings.TryGetValue(syntax, out var symbol) || 
                symbol is not DeclaredSymbol currentDeclaration ||
                string.IsNullOrEmpty(currentDeclaration.Name) ||
                string.Equals(LanguageConstants.ErrorName, currentDeclaration.Name, StringComparison.Ordinal) ||
                string.Equals(LanguageConstants.MissingName, currentDeclaration.Name, StringComparison.Ordinal))
            {
                // If we've failed to bind the symbol to a name, we should already have an error, and a cycle should not be possible
                return;
            }

            // Maintain the stack of declarations since they can be nested
            declarationAccessDict[currentDeclaration] = new List<SyntaxBase>();
            try
            {
                currentDeclarations.Push(currentDeclaration);
                visitBaseFunc(syntax);
            }
            finally
            {
                currentDeclarations.Pop();
            }
        }

        public override void VisitVariableDeclarationSyntax(VariableDeclarationSyntax syntax)
            => VisitDeclaration(syntax, base.VisitVariableDeclarationSyntax);

        public override void VisitModuleDeclarationSyntax(ModuleDeclarationSyntax syntax)
            => VisitDeclaration(syntax, base.VisitModuleDeclarationSyntax);

        public override void VisitOutputDeclarationSyntax(OutputDeclarationSyntax syntax)
            => VisitDeclaration(syntax, base.VisitOutputDeclarationSyntax);

        public override void VisitParameterDeclarationSyntax(ParameterDeclarationSyntax syntax)
            => VisitDeclaration(syntax, base.VisitParameterDeclarationSyntax);

        public override void VisitResourceDeclarationSyntax(ResourceDeclarationSyntax syntax)
        {
            // Push this resource onto the stack and process its body (including children).
            //
            // We process *this* resource using postorder because VisitDeclaration will do
            // some initialization.
            VisitDeclaration(syntax, base.VisitResourceDeclarationSyntax);

            // Resources are special because a lexically nested resource implies a dependency
            // They are both a source of declarations and a use of them.
            if (!bindings.TryGetValue(syntax, out var symbol) || symbol is not DeclaredSymbol currentDeclaration)
            {
                // If we've failed to bind the symbol, we should already have an error, and a cycle should not be possible
                return;
            }

            if (declarationAccessDict.TryGetValue(currentDeclaration, out var accesses))
            {
                // Walk all ancestors and add a reference from this resource
                foreach (var ancestor in currentDeclarations.OfType<ResourceSymbol>())
                {
                    accesses.Add(ancestor.DeclaringResource);
                }
            }
        }

        public override void VisitVariableAccessSyntax(VariableAccessSyntax syntax)
        {
            if (!currentDeclarations.TryPeek(out var currentDeclaration))
            {
                // we're not inside a declaration, so there should be no risk of a cycle
                return;
            }

            declarationAccessDict[currentDeclaration].Add(syntax);
            base.VisitVariableAccessSyntax(syntax);
        }

        public override void VisitResourceAccessSyntax(ResourceAccessSyntax syntax)
        {
            if (!currentDeclarations.TryPeek(out var currentDeclaration))
            {
                // we're not inside a declaration, so there should be no risk of a cycle
                return;
            }

            declarationAccessDict[currentDeclaration].Add(syntax);
            base.VisitResourceAccessSyntax(syntax);
        }

        public override void VisitFunctionCallSyntax(FunctionCallSyntax syntax)
        {
            if (!currentDeclarations.TryPeek(out var currentDeclaration))
            {
                // we're not inside a declaration, so there should be no risk of a cycle
                return;
            }

            declarationAccessDict[currentDeclaration].Add(syntax);
            base.VisitFunctionCallSyntax(syntax);
        }
    }
}

