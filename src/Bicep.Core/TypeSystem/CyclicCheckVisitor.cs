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
using Bicep.Core.Utils;

namespace Bicep.Core.TypeSystem
{
    public sealed class CyclicCheckVisitor : SyntaxVisitor
    {
        private readonly IReadOnlyDictionary<string, DeclaredSymbol> declarations;

        private readonly IReadOnlyDictionary<SyntaxBase, Symbol> bindings;

        private readonly IDictionary<DeclaredSymbol, IList<SyntaxBase>> declarationAccessDict;

        private DeclaredSymbol? currentDeclaration;

        public static ImmutableDictionary<DeclaredSymbol, ImmutableArray<DeclaredSymbol>> FindCycles(ProgramSyntax programSyntax, IReadOnlyDictionary<string, DeclaredSymbol> declarations, IReadOnlyDictionary<SyntaxBase, Symbol> bindings)
        {
            var visitor = new CyclicCheckVisitor(declarations, bindings);
            visitor.Visit(programSyntax);

            return visitor.FindCycles();
        }

        private ImmutableDictionary<DeclaredSymbol, ImmutableArray<DeclaredSymbol>> FindCycles()
        {
            var symbolGraph = declarationAccessDict
                .SelectMany(kvp => kvp.Value.Select(x => bindings[x]).OfType<DeclaredSymbol>().Select(x => (x, kvp.Key)))
                .ToLookup(x => x.Item1, x => x.Key);

            return CycleDetector<DeclaredSymbol>.FindCycles(symbolGraph);
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

        public override void VisitModuleDeclarationSyntax(ModuleDeclarationSyntax syntax)
        {
            currentDeclaration = declarations[syntax.Name.IdentifierName];
            declarationAccessDict[currentDeclaration] = new List<SyntaxBase>();
            base.VisitModuleDeclarationSyntax(syntax);
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

