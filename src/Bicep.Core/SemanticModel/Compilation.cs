// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Bicep.Core.Diagnostics;
using Bicep.Core.SemanticModel.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.SemanticModel
{
    public class Compilation
    {
        private readonly IResourceTypeProvider resourceTypeProvider;
        private readonly ImmutableDictionary<SyntaxTree, Lazy<SemanticModel>> lazySemanticModelLookup;

        public SyntaxTreeGrouping SyntaxTreeGrouping { get; }

        public Compilation(IResourceTypeProvider resourceTypeProvider, SyntaxTreeGrouping syntaxTreeGrouping)
        {
            this.SyntaxTreeGrouping = syntaxTreeGrouping;
            this.resourceTypeProvider = resourceTypeProvider;
            this.lazySemanticModelLookup = syntaxTreeGrouping.SyntaxTrees.ToImmutableDictionary(
                syntaxTree => syntaxTree,
                syntaxTree => new Lazy<SemanticModel>(() => GetSemanticModelInternal(syntaxTree)));
        }

        public SemanticModel GetEntrypointSemanticModel()
            => GetSemanticModel(SyntaxTreeGrouping.EntryPoint);

        public SemanticModel GetSemanticModel(SyntaxTree syntaxTree)
            => this.lazySemanticModelLookup[syntaxTree].Value;

        private SemanticModel GetSemanticModelInternal(SyntaxTree syntaxTree)
        {
            var builtinNamespaces = 
                new NamespaceSymbol[] { new SystemNamespaceSymbol(), new AzNamespaceSymbol() }
                .ToImmutableDictionary(property => property.Name, property => property, LanguageConstants.IdentifierComparer);

            var bindings = new Dictionary<SyntaxBase, Symbol>();
            var cyclesBySymbol = new Dictionary<DeclaredSymbol, ImmutableArray<DeclaredSymbol>>();
            
            var hierarchy = new SyntaxHierarchy();
            hierarchy.AddRoot(syntaxTree.ProgramSyntax);
            
            // create this in locked mode by default
            // this blocks accidental type or binding queries until binding is done
            // (if a type check is done too early, unbound symbol references would cause incorrect type check results)
            var symbolContext = new SymbolContext(new TypeManager(resourceTypeProvider, bindings, cyclesBySymbol, hierarchy), bindings, this);

            // collect declarations
            var declarations = new List<DeclaredSymbol>();
            var declarationVisitor = new DeclarationVisitor(symbolContext, declarations);
            declarationVisitor.Visit(syntaxTree.ProgramSyntax);

            // in cases of duplicate declarations we will see multiple declaration symbols in the result list
            // for simplicitly we will bind to the first one
            // it may cause follow-on type errors, but there will also be errors about duplicate identifiers as well
            var uniqueDeclarations = declarations
                .ToLookup(x => x.Name, LanguageConstants.IdentifierComparer)
                .ToImmutableDictionary(x => x.Key, x => x.First(), LanguageConstants.IdentifierComparer);

            // bind identifiers to declarations
            var binder = new NameBindingVisitor(uniqueDeclarations, bindings, builtinNamespaces);
            binder.Visit(syntaxTree.ProgramSyntax);

            var shortestCycleBySymbol = CyclicCheckVisitor.FindCycles(syntaxTree.ProgramSyntax, uniqueDeclarations, bindings);
            foreach (var kvp in shortestCycleBySymbol)
            {
                cyclesBySymbol.Add(kvp.Key, kvp.Value);
            }

            // TODO: Avoid looping 5 times?
            var file = new FileSymbol(
                syntaxTree.FilePath,
                syntaxTree.ProgramSyntax,
                builtinNamespaces,
                declarations.OfType<ParameterSymbol>(),
                declarations.OfType<VariableSymbol>(),
                declarations.OfType<ResourceSymbol>(),
                declarations.OfType<ModuleSymbol>(),
                declarations.OfType<OutputSymbol>());

            // name binding is done
            // allow type queries now
            symbolContext.Unlock();

            return new SemanticModel(file, symbolContext.TypeManager, bindings);
        }

        public bool EmitDiagnosticsAndCheckSuccess(Action<SyntaxTree, Diagnostic> onDiagnostic)
        {
            var success = true;
            foreach (var syntaxTree in SyntaxTreeGrouping.SyntaxTrees)
            {
                var semanticModel = GetSemanticModel(syntaxTree);

                foreach (var diagnostic in semanticModel.GetAllDiagnostics())
                {
                    success &= diagnostic.Level != DiagnosticLevel.Error;
                    onDiagnostic(syntaxTree, diagnostic);
                }
            }

            return success;
        }
    }
}
