// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Azure.Deployments.Core.Extensions;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics
{
    public sealed class NameBindingVisitor : SyntaxVisitor
    {
        private FunctionFlags allowedFlags;

        private readonly IReadOnlyDictionary<string, DeclaredSymbol> declarations;

        private readonly IDictionary<SyntaxBase, Symbol> bindings;

        private readonly NamespaceResolver namespaceResolver;

        private readonly ImmutableDictionary<SyntaxBase, LocalScope> allLocalScopes;

        private readonly Stack<LocalScope> activeScopes;

        private NameBindingVisitor(
            IReadOnlyDictionary<string, DeclaredSymbol> declarations,
            IDictionary<SyntaxBase, Symbol> bindings,
            NamespaceResolver namespaceResolver,
            ImmutableDictionary<SyntaxBase, LocalScope> allLocalScopes)
        {
            this.declarations = declarations;
            this.bindings = bindings;
            this.namespaceResolver = namespaceResolver;
            this.allLocalScopes = allLocalScopes;
            this.activeScopes = new Stack<LocalScope>();
        }

        public static ImmutableDictionary<SyntaxBase, Symbol> GetBindings(
            ProgramSyntax programSyntax,
            IReadOnlyDictionary<string, DeclaredSymbol> outermostDeclarations,
            NamespaceResolver namespaceResolver,
            ImmutableArray<LocalScope> childScopes)
        {
            // bind identifiers to declarations
            var bindings = new Dictionary<SyntaxBase, Symbol>();
            var allLocalScopes = ScopeCollectorVisitor.Build(childScopes);
            var binder = new NameBindingVisitor(outermostDeclarations, bindings, namespaceResolver, allLocalScopes);
            binder.Visit(programSyntax);

            return bindings.ToImmutableDictionary();
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

            // include all the locals in the symbol table as well
            // since we only allow lookups by object and not by name,
            // a flat symbol table should be sufficient
            foreach (var declaredSymbol in allLocalScopes.Values.SelectMany(scope => scope.Declarations))
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

        public override void VisitResourceAccessSyntax(ResourceAccessSyntax syntax)
        {
            base.VisitResourceAccessSyntax(syntax);

            // we need to resolve which resource delaration the LHS is pointing to - and then 
            // validate that we can resolve the name.
            this.bindings.TryGetValue(syntax.BaseExpression, out var symbol);

            if (symbol is ErrorSymbol)
            {
                this.bindings.Add(syntax, symbol);
                return;
            }
            else if (symbol is null || symbol is not ResourceSymbol)
            {
                // symbol could be null in the case of an incomplete expression during parsing like `a:`
                var error = new ErrorSymbol(DiagnosticBuilder.ForPosition(syntax.ResourceName).ResourceRequiredForResourceAccess(symbol?.Kind.ToString() ?? LanguageConstants.ErrorName));
                this.bindings.Add(syntax, error);
                return;
            }

            // This is the symbol of LHS and it's a valid resource.
            var resourceSymbol = (ResourceSymbol)symbol;
            var resourceBody = resourceSymbol.DeclaringResource.TryGetBody();
            if (resourceBody == null)
            {
                // If we have no body then there will be nothing to reference.
                var error = new ErrorSymbol(DiagnosticBuilder.ForPosition(syntax.ResourceName).NestedResourceNotFound(resourceSymbol.Name, syntax.ResourceName.IdentifierName, nestedResourceNames: new []{ "(none)", }));
                this.bindings.Add(syntax, error);
                return;
            }

            if (!this.allLocalScopes.TryGetValue(resourceBody, out var localScope))
            {
                // code defect in the declaration visitor
                throw new InvalidOperationException($"Local scope is missing for {syntax.GetType().Name} at {syntax.Span}");
            }

            var referencedResource = LookupResourceSymbolByName(localScope, syntax.ResourceName);
            if (referencedResource is null)
            {
                var nestedResourceNames = localScope.Declarations.OfType<ResourceSymbol>().Select(r => r.Name);
                var error = new ErrorSymbol(DiagnosticBuilder.ForPosition(syntax.ResourceName).NestedResourceNotFound(resourceSymbol.Name, syntax.ResourceName.IdentifierName, nestedResourceNames));
                this.bindings.Add(syntax, error);
                return;
            }
            
            // This is valid.
            this.bindings.Add(syntax, referencedResource);
        }

        public override void VisitResourceDeclarationSyntax(ResourceDeclarationSyntax syntax)
        {
            allowedFlags = FunctionFlags.ResourceDecorator;
            this.VisitNodes(syntax.LeadingNodes);
            this.Visit(syntax.Keyword);
            this.Visit(syntax.Name);
            this.Visit(syntax.Type);
            this.Visit(syntax.ExistingKeyword);
            this.Visit(syntax.Assignment);
            allowedFlags = FunctionFlags.RequiresInlining;
            this.Visit(syntax.Value);
            allowedFlags = FunctionFlags.Default;
        }

        public override void VisitModuleDeclarationSyntax(ModuleDeclarationSyntax syntax)
        {
            allowedFlags = FunctionFlags.ModuleDecorator;
            this.VisitNodes(syntax.LeadingNodes);
            this.Visit(syntax.Keyword);
            this.Visit(syntax.Name);
            this.Visit(syntax.Path);
            this.Visit(syntax.Assignment);
            allowedFlags = FunctionFlags.RequiresInlining;
            this.Visit(syntax.Value);
            allowedFlags = FunctionFlags.Default;
        }

        public override void VisitIfConditionSyntax(IfConditionSyntax syntax)
        {
            this.Visit(syntax.Keyword);
            allowedFlags = FunctionFlags.Default;
            this.Visit(syntax.ConditionExpression);
            // if-condition syntax parent is always a resource/module declaration
            // this means that we have to allow the functions that are only allowed
            // in resource bodies by our runtime (like reference() or listKeys())
            // TODO: Update when conditions can be composed together with loops
            allowedFlags = FunctionFlags.RequiresInlining;
            this.Visit(syntax.Body);
            allowedFlags = FunctionFlags.Default;
        }

        public override void VisitVariableDeclarationSyntax(VariableDeclarationSyntax syntax)
        {
            allowedFlags = FunctionFlags.VariableDecorator;
            this.VisitNodes(syntax.LeadingNodes);
            this.Visit(syntax.Keyword);
            this.Visit(syntax.Name);
            this.Visit(syntax.Assignment);
            allowedFlags = FunctionFlags.RequiresInlining;
            this.Visit(syntax.Value);
            allowedFlags = FunctionFlags.Default;
        }

        public override void VisitOutputDeclarationSyntax(OutputDeclarationSyntax syntax)
        {
            allowedFlags = FunctionFlags.OutputDecorator;
            this.VisitNodes(syntax.LeadingNodes);
            this.Visit(syntax.Keyword);
            this.Visit(syntax.Name);
            this.Visit(syntax.Type);
            this.Visit(syntax.Assignment);
            allowedFlags = FunctionFlags.RequiresInlining;
            this.Visit(syntax.Value);
            allowedFlags = FunctionFlags.Default;
        }

        public override void VisitImportDeclarationSyntax(ImportDeclarationSyntax syntax)
        {
            allowedFlags = FunctionFlags.ImportDecorator;
            this.VisitNodes(syntax.LeadingNodes);
            this.Visit(syntax.Keyword);
            this.Visit(syntax.AliasName);
            this.Visit(syntax.FromKeyword);
            this.Visit(syntax.ProviderName);
            this.Visit(syntax.Config);
            allowedFlags = FunctionFlags.Default;
        }

        public override void VisitParameterDeclarationSyntax(ParameterDeclarationSyntax syntax)
        {
            allowedFlags = FunctionFlags.ParameterDecorator;
            this.VisitNodes(syntax.LeadingNodes);
            this.Visit(syntax.Keyword);
            this.Visit(syntax.Name);
            this.Visit(syntax.Type);
            allowedFlags = FunctionFlags.ParamDefaultsOnly;
            this.Visit(syntax.Modifier);
            allowedFlags = FunctionFlags.Default;
        }

        public override void VisitMissingDeclarationSyntax(MissingDeclarationSyntax syntax)
        {
            allowedFlags = FunctionFlags.ParameterDecorator |
                FunctionFlags.VariableDecorator |
                FunctionFlags.ResourceDecorator |
                FunctionFlags.ModuleDecorator |
                FunctionFlags.OutputDecorator |
                FunctionFlags.ImportDecorator;
            base.VisitMissingDeclarationSyntax(syntax);
            allowedFlags = FunctionFlags.Default;
        }

        public override void VisitFunctionCallSyntax(FunctionCallSyntax syntax)
        {
            FunctionFlags currentFlags = allowedFlags;
            this.Visit(syntax.Name);
            this.Visit(syntax.OpenParen);
            allowedFlags = allowedFlags.HasAnyDecoratorFlag() ? FunctionFlags.Default : allowedFlags;
            this.VisitNodes(syntax.Arguments);
            this.Visit(syntax.CloseParen);
            allowedFlags = currentFlags;

            var symbol = this.LookupSymbolByName(syntax.Name, true);

            // bind what we got - the type checker will validate if it fits
            this.bindings.Add(syntax, symbol);
        }

        protected override void VisitInternal(SyntaxBase syntax)
        {
            // any node can be a binding scope
            if (!this.allLocalScopes.TryGetValue(syntax, out var localScope))
            {
                // not a binding scope
                // visit children normally
                base.VisitInternal(syntax);
                return;
            }

            // we are in a binding scope
            // push it to the stack of active scopes
            // as a result this scope will be used to resolve symbols first
            // (then all the previous one and then finally the global scope)
            this.activeScopes.Push(localScope);

            // visit all the children
            base.VisitInternal(syntax);

            // we are leaving the loop scope
            // pop the scope - no symbols will be resolved against it ever again
            var lastPopped = this.activeScopes.Pop();
            Debug.Assert(ReferenceEquals(lastPopped, localScope), "ReferenceEquals(lastPopped, localScope)");
        }

        public override void VisitForSyntax(ForSyntax syntax)
        {
            // we must have a scope in the map for the loop body - otherwise binding won't work
            Debug.Assert(this.allLocalScopes.ContainsKey(syntax.Body), "this.allLocalScopes.ContainsKey(syntax.Body)");
            
            // visit all the children
            base.VisitForSyntax(syntax);
        }

        private Symbol LookupSymbolByName(IdentifierSyntax identifierSyntax, bool isFunctionCall) => 
            this.LookupLocalSymbolByName(identifierSyntax, isFunctionCall) ?? LookupGlobalSymbolByName(identifierSyntax, isFunctionCall);

        private Symbol? LookupLocalSymbolByName(IdentifierSyntax identifierSyntax, bool isFunctionCall)
        {
            if (isFunctionCall)
            {
                // functions can't be local symbols
                return null;
            }

            // iterating over a stack gives you the items in the same
            // order as if you popped each one but without modifying the stack
            foreach (var scope in activeScopes)
            {
                // resolve symbol against current scope
                // this binds to the innermost symbol even if there exists one at the parent scope
                var symbol = LookupLocalSymbolByName(scope, identifierSyntax);
                if (symbol != null)
                {
                    // found a symbol - return it
                    return symbol;
                }
            }

            return null;
        }

        private static Symbol? LookupLocalSymbolByName(LocalScope scope, IdentifierSyntax identifierSyntax) => 
            // bind to first symbol matching the specified identifier
            // (errors about duplicate identifiers are emitted elsewhere)
            // loops currently are the only source of local symbols
            // as a result a local scope can contain between 1 to 2 local symbols
            // linear search should be fine, but this should be revisited if the above is no longer holds true
            scope.Declarations.FirstOrDefault(symbol => string.Equals(identifierSyntax.IdentifierName, symbol.Name, LanguageConstants.IdentifierComparison));

        private static ResourceSymbol? LookupResourceSymbolByName(ILanguageScope scope, IdentifierSyntax identifierSyntax) =>
            scope.Declarations
                .OfType<ResourceSymbol>()
                .FirstOrDefault(symbol => string.Equals(identifierSyntax.IdentifierName, symbol.Name, LanguageConstants.IdentifierComparison));

        private Symbol LookupGlobalSymbolByName(IdentifierSyntax identifierSyntax, bool isFunctionCall)
        {
            // attempt to find name in the built in namespaces. imported namespaces will be present in the declarations list as they create declared symbols.
            if (this.namespaceResolver.BuiltIns.TryGetValue(identifierSyntax.IdentifierName) is { } namespaceSymbol)
            {
                // namespace symbol found
                return namespaceSymbol;
            }

            // declarations must not have a namespace value, namespaces are used to fully qualify a function access.
            // There might be instances where a variable declaration for example uses the same name as one of the imported
            // functions, in this case to differentiate a variable declaration vs a function access we check the namespace value,
            // the former case must have an empty namespace value whereas the latter will have a namespace value.
            if (this.declarations.TryGetValue(identifierSyntax.IdentifierName, out var globalSymbol))
            {
                // we found the symbol in the global namespace
                return globalSymbol;
            }

            // attempt to find function in all imported namespaces
            var foundSymbols = namespaceResolver.ResolveUnqualifiedFunction(identifierSyntax, includeDecorators: allowedFlags.HasAnyDecoratorFlag());
            if (foundSymbols.Count() > 1)
            {
                // ambiguous symbol
                return new ErrorSymbol(DiagnosticBuilder.ForPosition(identifierSyntax).AmbiguousSymbolReference(identifierSyntax.IdentifierName, namespaceResolver.GetNamespaceNames().ToImmutableSortedSet(StringComparer.Ordinal)));
            }

            var foundSymbol = foundSymbols.FirstOrDefault();
            return isFunctionCall ?
                SymbolValidator.ResolveUnqualifiedFunction(allowedFlags, foundSymbol, identifierSyntax, namespaceResolver) :
                SymbolValidator.ResolveUnqualifiedSymbol(foundSymbol, identifierSyntax, namespaceResolver, declarations.Keys);
        }
        
        private class ScopeCollectorVisitor: SymbolVisitor
        {
            private IDictionary<SyntaxBase, LocalScope> ScopeMap { get; } = new Dictionary<SyntaxBase, LocalScope>();


            protected override void VisitInternal(Symbol node)
            {
                // We haven't typed checked yet, so don't visit anything that isn't a scope.
                // 
                // Now that resources can appear in a scope, this causes problems if we visit them and try
                // to get type info.
                if (node is ILanguageScope)
                {
                    base.VisitInternal(node);
                }
            }

            public override void VisitLocalScope(LocalScope symbol)
            {
                this.ScopeMap.Add(symbol.BindingSyntax, symbol);
                base.VisitLocalScope(symbol);
            }

            public static ImmutableDictionary<SyntaxBase, LocalScope> Build(ImmutableArray<LocalScope> outermostScopes)
            {
                var visitor = new ScopeCollectorVisitor();
                foreach (LocalScope outermostScope in outermostScopes)
                {
                    visitor.Visit(outermostScope);
                }

                return visitor.ScopeMap.ToImmutableDictionary();
            }
        }
    }
}
