﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Bicep.Core.SemanticModel.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.SemanticModel
{
    public class Compilation
    {
        private readonly Lazy<SemanticModel> lazySemanticModel;

        public Compilation(ProgramSyntax programSyntax)
        {
            this.ProgramSyntax = programSyntax;

            this.lazySemanticModel = new Lazy<SemanticModel>(this.GetSemanticModelInternal, LazyThreadSafetyMode.PublicationOnly);
        }

        public ProgramSyntax ProgramSyntax { get; }

        public SemanticModel GetSemanticModel() => this.lazySemanticModel.Value;

        private SemanticModel GetSemanticModelInternal()
        {
            var builtinNamespaces = new NamespaceSymbol[] {new SystemNamespaceSymbol(), new AzNamespaceSymbol()}.ToImmutableArray();
            var bindings = new Dictionary<SyntaxBase, Symbol>();
            
            // create this in locked mode by default
            // this blocks accidental type queries until binding is done
            // (if a type check is done too early, unbound symbol references would cause incorrect type check results)
            var typeCache = new TypeManager(bindings);

            // collect declarations
            var declarations = new List<DeclaredSymbol>();
            var declarationVisitor = new DeclarationVisitor(typeCache, declarations);
            declarationVisitor.Visit(this.ProgramSyntax);

            // in cases of duplicate declarations we will see multiple declaration symbols in the result list
            // for simplicitly we will bind to the first one
            // it may cause follow-on type errors, but there will also be errors about duplicate identifiers as well
            var uniqueDeclarations = declarations
                .ToLookup(x => x.Name, LanguageConstants.IdentifierComparer)
                .ToImmutableDictionary(x => x.Key, x => x.First(), LanguageConstants.IdentifierComparer);

            // bind identifiers to declarations
            var binder = new NameBindingVisitor(uniqueDeclarations, bindings, builtinNamespaces);
            binder.Visit(this.ProgramSyntax);

            // name binding is done
            // allow type queries now
            typeCache.Unlock();

            var symbolGraph = SymbolGraphVisitor.Build(this.ProgramSyntax, uniqueDeclarations, bindings);

            // TODO: Avoid looping 4 times?
            var file = new FileSymbol("main",
                this.ProgramSyntax,
                builtinNamespaces,
                declarations.OfType<ParameterSymbol>(),
                declarations.OfType<VariableSymbol>(),
                declarations.OfType<ResourceSymbol>(),
                declarations.OfType<OutputSymbol>());

            return new SemanticModel(file, typeCache, bindings, symbolGraph);
        }
    }
}