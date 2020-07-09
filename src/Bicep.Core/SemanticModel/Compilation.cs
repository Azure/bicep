using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
            var typeCache = new TypeCache();

            List<Symbol> declarations = new List<Symbol>();
            var declarationVisitor = new DeclarationVisitor(typeCache, declarations);
            declarationVisitor.Visit(this.ProgramSyntax);

            // TODO: Reparent context to semantic model?
            // TODO: Avoid looping 4 times?
            var file = new FileSymbol(
                typeCache,
                "main",
                this.ProgramSyntax,
                declarations.OfType<ParameterSymbol>(),
                declarations.OfType<VariableSymbol>(),
                declarations.OfType<ResourceSymbol>(),
                declarations.OfType<OutputSymbol>());
            return new SemanticModel(file, typeCache);
        }
    }
}