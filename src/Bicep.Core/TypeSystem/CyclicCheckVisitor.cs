// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Utils;

namespace Bicep.Core.TypeSystem
{
    public sealed class CyclicCheckVisitor : SyntaxVisitor
    {
        private readonly IReadOnlyDictionary<string, DeclaredSymbol> declarations;

        private readonly IReadOnlyDictionary<SyntaxBase, Symbol> bindings;

        private readonly IDictionary<DeclaredSymbol, IList<SyntaxBase>> declarationAccessDict;

        private DeclaredSymbol? currentDeclaration;

        private SyntaxBase? currentDecorator;

        public static ImmutableDictionary<DeclaredSymbol, ImmutableArray<DeclaredSymbol>> FindCycles(ProgramSyntax programSyntax, IReadOnlyDictionary<string, DeclaredSymbol> declarations, IReadOnlyDictionary<SyntaxBase, Symbol> bindings)
        {
            var visitor = new CyclicCheckVisitor(declarations, bindings);
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

        private CyclicCheckVisitor(IReadOnlyDictionary<string, DeclaredSymbol> declarations, IReadOnlyDictionary<SyntaxBase, Symbol> bindings)
        {
            this.declarations = declarations;
            this.bindings = bindings;
            this.declarationAccessDict = new Dictionary<DeclaredSymbol, IList<SyntaxBase>>();
        }

        private void VisitDeclaration<TDeclarationSyntax>(TDeclarationSyntax syntax, Action<TDeclarationSyntax> visitBaseFunc)
            where TDeclarationSyntax : SyntaxBase, ITopLevelNamedDeclarationSyntax
        {
            if (!bindings.ContainsKey(syntax))
            {
                // If we've failed to bind the symbol, we should already have an error, and a cycle should not be possible
                return;
            }

            currentDeclaration = declarations[syntax.Name.IdentifierName];
            declarationAccessDict[currentDeclaration] = new List<SyntaxBase>();
            visitBaseFunc(syntax);
            currentDeclaration = null;
        }

        public override void VisitVariableDeclarationSyntax(VariableDeclarationSyntax syntax)
            => VisitDeclaration(syntax, base.VisitVariableDeclarationSyntax);

        public override void VisitResourceDeclarationSyntax(ResourceDeclarationSyntax syntax)
            => VisitDeclaration(syntax, base.VisitResourceDeclarationSyntax);

        public override void VisitModuleDeclarationSyntax(ModuleDeclarationSyntax syntax)
            => VisitDeclaration(syntax, base.VisitModuleDeclarationSyntax);

        public override void VisitOutputDeclarationSyntax(OutputDeclarationSyntax syntax)
            => VisitDeclaration(syntax, base.VisitOutputDeclarationSyntax);

        public override void VisitParameterDeclarationSyntax(ParameterDeclarationSyntax syntax)
            => VisitDeclaration(syntax, base.VisitParameterDeclarationSyntax);

        public override void VisitVariableAccessSyntax(VariableAccessSyntax syntax)
        {
            if (currentDeclaration == null)
            {
                if (currentDecorator != null)
                {
                    // We are inside a dangling decorator.
                    return;
                }

                throw new ArgumentException($"Variable access outside of declaration");
            }

            declarationAccessDict[currentDeclaration].Add(syntax);
            base.VisitVariableAccessSyntax(syntax);
        }

        public override void VisitDecoratorSyntax(DecoratorSyntax syntax)
        {
            this.currentDecorator = syntax;
            base.VisitDecoratorSyntax(syntax);
            this.currentDecorator = null;
        }

        public override void VisitFunctionCallSyntax(FunctionCallSyntax syntax)
        {
            if (currentDeclaration == null)
            {
                if (currentDecorator != null)
                {
                    // We are inside a dangling decorator.
                    return;
                }

                throw new ArgumentException($"Function access outside of declaration or decorator");
            }

            declarationAccessDict[currentDeclaration].Add(syntax);
            base.VisitFunctionCallSyntax(syntax);
        }
    }
}

