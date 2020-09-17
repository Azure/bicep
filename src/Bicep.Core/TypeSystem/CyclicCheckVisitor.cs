// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parser;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;

namespace Bicep.Core.TypeSystem
{
    public sealed class CyclicCheckVisitor : SyntaxVisitor
    {
        private readonly IReadOnlyDictionary<string, DeclaredSymbol> declarations;

        private readonly IReadOnlyDictionary<SyntaxBase, Symbol> bindings;

        private readonly IDictionary<DeclaredSymbol, IList<SyntaxBase>> declarationAccessDict;

        private DeclaredSymbol? currentDeclaration;

        public static ImmutableDictionary<SyntaxBase, DeclaredSymbol[]> FindCycles(ProgramSyntax programSyntax, IReadOnlyDictionary<string, DeclaredSymbol> declarations, IReadOnlyDictionary<SyntaxBase, Symbol> bindings)
        {
            var visitor = new CyclicCheckVisitor(declarations, bindings);
            visitor.Visit(programSyntax);

            return visitor.FindCycles();
        }

        private enum VisitorState
        {
            Partial,
            Complete,
        }

        private ImmutableDictionary<SyntaxBase, DeclaredSymbol[]> FindCycles()
        {
            var shortestCycleBySyntax = new Dictionary<SyntaxBase, DeclaredSymbol[]>();
            var visitState = new Dictionary<DeclaredSymbol, VisitorState>();
            foreach (var kvp in declarationAccessDict)
            {
                var declaration = kvp.Key;
                if (visitState.ContainsKey(declaration))
                {
                    continue;
                }

                var syntaxBySymbol = kvp.Value.GroupBy(x => bindings[x]);
                var symbolStack = new Stack<DeclaredSymbol>();
                symbolStack.Push(declaration);
                visitState[declaration] = VisitorState.Partial;

                FindCyclesDfs(symbolStack, visitState, shortestCycleBySyntax);
            }

            return shortestCycleBySyntax.ToImmutableDictionary();
        }

        private void FindCyclesDfs(Stack<DeclaredSymbol> symbolStack, Dictionary<DeclaredSymbol, VisitorState> visitState, IDictionary<SyntaxBase, DeclaredSymbol[]> shortestCycleBySyntax)
        {
            var declaration = symbolStack.Peek();
            var syntaxBySymbol = declarationAccessDict[declaration].GroupBy(x => bindings[x]);

            foreach (var grouping in syntaxBySymbol)
            {
                if (!(grouping.Key is DeclaredSymbol referencedDeclaration))
                {
                    continue;
                }

                if (!visitState.TryGetValue(referencedDeclaration, out var referencedState))
                {
                    symbolStack.Push(referencedDeclaration);
                    visitState[referencedDeclaration] = VisitorState.Partial;
                    FindCyclesDfs(symbolStack, visitState, shortestCycleBySyntax);
                    continue;
                }
                
                if (referencedState == VisitorState.Partial)
                {
                    AddCycleInformation(symbolStack, referencedDeclaration, shortestCycleBySyntax);
                }
            }

            visitState[declaration] = VisitorState.Complete;
            symbolStack.Pop();
        }

        private void AddCycleInformation(Stack<DeclaredSymbol> symbolStack, DeclaredSymbol referencedDeclaration, IDictionary<SyntaxBase, DeclaredSymbol[]> shortestCycleBySyntax)
        {
            var cycle = symbolStack
                .TakeWhile(x => x != referencedDeclaration)
                .Concat(new [] { referencedDeclaration })
                .Reverse()
                .ToArray();

            var cyclicSyntaxBySymbol = cycle
                .SelectMany(s => declarationAccessDict[s])
                .ToLookup(s => bindings[s]);

            foreach (var symbol in cycle)
            {
                foreach (var syntax in cyclicSyntaxBySymbol[symbol])
                {
                    if (shortestCycleBySyntax.TryGetValue(syntax, out var otherCycle) && otherCycle.Length <= cycle.Length)
                    {
                        // we've already found a shorter cycle
                        continue;
                    }

                    shortestCycleBySyntax[syntax] = cycle;
                }
            }
        }

        private CyclicCheckVisitor(IReadOnlyDictionary<string, DeclaredSymbol> declarations, IReadOnlyDictionary<SyntaxBase, Symbol> bindings)
        {
            this.declarations = declarations;
            this.bindings = bindings;
            this.declarationAccessDict = new Dictionary<DeclaredSymbol, IList<SyntaxBase>>();
        }

        public override void VisitVariableDeclarationSyntax(VariableDeclarationSyntax syntax)
        {
            currentDeclaration = declarations[syntax.Name.IdentifierName];
            declarationAccessDict[currentDeclaration] = new List<SyntaxBase>();
            base.VisitVariableDeclarationSyntax(syntax);
            currentDeclaration = null;
        }

        public override void VisitResourceDeclarationSyntax(ResourceDeclarationSyntax syntax)
        {
            currentDeclaration = declarations[syntax.Name.IdentifierName];
            declarationAccessDict[currentDeclaration] = new List<SyntaxBase>();
            base.VisitResourceDeclarationSyntax(syntax);
            currentDeclaration = null;
        }

        public override void VisitOutputDeclarationSyntax(OutputDeclarationSyntax syntax)
        {
            currentDeclaration = declarations[syntax.Name.IdentifierName];
            declarationAccessDict[currentDeclaration] = new List<SyntaxBase>();
            base.VisitOutputDeclarationSyntax(syntax);
            currentDeclaration = null;
        }

        public override void VisitParameterDeclarationSyntax(ParameterDeclarationSyntax syntax)
        {
            currentDeclaration = declarations[syntax.Name.IdentifierName];
            declarationAccessDict[currentDeclaration] = new List<SyntaxBase>();
            base.VisitParameterDeclarationSyntax(syntax);
            currentDeclaration = null;
        }

        public override void VisitVariableAccessSyntax(VariableAccessSyntax syntax)
        {
            if (currentDeclaration == null)
            {
                throw new ArgumentException($"Variable access outside of declaration");
            }

            declarationAccessDict[currentDeclaration].Add(syntax);
            base.VisitVariableAccessSyntax(syntax);
        }

        public override void VisitFunctionCallSyntax(FunctionCallSyntax syntax)
        {
            if (currentDeclaration == null)
            {
                throw new ArgumentException($"Function access outside of declaration");
            }

            declarationAccessDict[currentDeclaration].Add(syntax);
            base.VisitFunctionCallSyntax(syntax);
        }
    }
}

