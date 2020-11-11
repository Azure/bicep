// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.SemanticModel
{
    public sealed class NameBindingVisitor : SyntaxVisitor
    {
        private FunctionFlags allowedFlags;

        private readonly IReadOnlyDictionary<string, DeclaredSymbol> declarations;

        private readonly IDictionary<SyntaxBase, Symbol> bindings;

        private readonly ImmutableDictionary<string, NamespaceSymbol> namespaces;

        public NameBindingVisitor(IReadOnlyDictionary<string, DeclaredSymbol> declarations, IDictionary<SyntaxBase, Symbol> bindings, ImmutableDictionary<string, NamespaceSymbol> namespaces)
        {
            this.declarations = declarations;
            this.bindings = bindings;
            this.namespaces = namespaces;
        }

        public override void VisitProgramSyntax(ProgramSyntax syntax)
        {
            base.VisitProgramSyntax(syntax);

            // create bindings for all of the declarations to their corresponding symbol
            // this is needed to make find all references work correctly
            // (doing this here to avoid side-effects in the constructor)
            foreach (DeclaredSymbol declaredSymbol in this.declarations.Values)
            {
                this.bindings.Add(declaredSymbol.DeclaringSyntax, declaredSymbol);
            }
        }

        public override void VisitVariableAccessSyntax(VariableAccessSyntax syntax)
        {
            base.VisitVariableAccessSyntax(syntax);

            var symbol = this.LookupSymbolByName(syntax.Name, false);

            // bind what we got - the type checker will validate if it fits
            this.bindings.Add(syntax, symbol);
        }

        public override void VisitResourceDeclarationSyntax(ResourceDeclarationSyntax syntax)
        {
            allowedFlags = FunctionFlags.RequiresInlining;
            base.VisitResourceDeclarationSyntax(syntax);
            allowedFlags = FunctionFlags.Default;
        }

        public override void VisitModuleDeclarationSyntax(ModuleDeclarationSyntax syntax)
        {
            allowedFlags = FunctionFlags.RequiresInlining;
            base.VisitModuleDeclarationSyntax(syntax);
            allowedFlags = FunctionFlags.Default;
        }

        public override void VisitVariableDeclarationSyntax(VariableDeclarationSyntax syntax)
        {
            allowedFlags = FunctionFlags.RequiresInlining;
            base.VisitVariableDeclarationSyntax(syntax);
            allowedFlags = FunctionFlags.Default;
        }

        public override void VisitOutputDeclarationSyntax(OutputDeclarationSyntax syntax)
        {
            allowedFlags = FunctionFlags.RequiresInlining;
            base.VisitOutputDeclarationSyntax(syntax);
            allowedFlags = FunctionFlags.Default;
        }

        public override void VisitParameterDeclarationSyntax(ParameterDeclarationSyntax syntax)
        {
            allowedFlags = FunctionFlags.ParamDefaultsOnly;
            base.VisitParameterDeclarationSyntax(syntax);
            allowedFlags = FunctionFlags.Default;
        }

        public override void VisitFunctionCallSyntax(FunctionCallSyntax syntax)
        {
            base.VisitFunctionCallSyntax(syntax);

            var symbol = this.LookupSymbolByName(syntax.Name, true);

            // bind what we got - the type checker will validate if it fits
            this.bindings.Add(syntax, symbol);
        }

        public override void VisitInstanceFunctionCallSyntax(InstanceFunctionCallSyntax syntax)
        {
            base.VisitInstanceFunctionCallSyntax(syntax);

            if (!syntax.Name.IsValid)
            {
                // the parser produced an instance function calls with an invalid name
                // all instance function calls must be bound to a symbol, so let's
                // bind to a symbol without any errors (there's already a parse error)
                this.bindings.Add(syntax, new ErrorSymbol());
                return;
            }

            if (bindings.TryGetValue(syntax.BaseExpression, out var baseSymbol) && baseSymbol is NamespaceSymbol namespaceSymbol)
            {
                var functionSymbol = namespaceSymbol.Type.MethodResolver.TryGetSymbol(syntax.Name);

                var foundSymbol = SymbolValidator.ResolveNamespaceQualifiedFunction(allowedFlags, functionSymbol, syntax.Name, namespaceSymbol);
                
                this.bindings.Add(syntax, foundSymbol);
            }
        }

        private Symbol LookupSymbolByName(IdentifierSyntax identifierSyntax, bool isFunctionCall)
        {
            // attempt to find name in the imported namespaces
            if (this.namespaces.TryGetValue(identifierSyntax.IdentifierName, out var namespaceSymbol))
            {
                // namespace symbol found
                return namespaceSymbol;
            }

            // declarations must not have a namespace value, namespaces are used to fully qualify a function access.
            // There might be instances where a variable declaration for example uses the same name as one of the imported
            // functions, in this case to differentiate a variable declaration vs a function access we check the namespace value,
            // the former case must have an empty namespace value whereas the latter will have a namespace value.
            if (this.declarations.TryGetValue(identifierSyntax.IdentifierName, out var localSymbol))
            {
                // we found the symbol in the local namespace
                return localSymbol;
            }

            // attempt to find function in all imported namespaces
            var foundSymbols = this.namespaces
                .Select(kvp => kvp.Value.Type.MethodResolver.TryGetSymbol(identifierSyntax))
                .Where(symbol => symbol != null)
                .ToList();

            if (foundSymbols.Count > 1)
            {
                // ambiguous symbol
                return new ErrorSymbol(DiagnosticBuilder.ForPosition(identifierSyntax).AmbiguousSymbolReference(identifierSyntax.IdentifierName, this.namespaces.Keys));
            }

            var foundSymbol = foundSymbols.FirstOrDefault();
            return isFunctionCall ?
                SymbolValidator.ResolveUnqualifiedFunction(allowedFlags, foundSymbol, identifierSyntax, namespaces.Values) :
                SymbolValidator.ResolveUnqualifiedSymbol(foundSymbol, identifierSyntax, namespaces.Values, declarations.Keys);
        }
    }
}