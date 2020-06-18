using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Bicep.Core.Parser;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.Visitors;

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

        /// <summary>
        /// Gets all the parser and lexer diagnostics. Does not include diagnostics from the semantic model.
        /// </summary>
        public IEnumerable<Error> GetParseDiagnostics()
        {
            var errors = new List<Error>();
            var visitor = new CheckVisitor(errors);
            visitor.Visit(this.ProgramSyntax);

            return errors;
        }

        /// <summary>
        /// Gets all the semantic diagnostics.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Error> GetSemanticDiagnostics()
        {
            // TODO: Implement a semantic visitor or move into SemanticModel?
            var model = this.GetSemanticModel();
            return model.Root
                .GetErrors()
                .Concat(model.Root.ParameterDeclarations.SelectMany(paramDecl => paramDecl.GetErrors()));
        }

        /// <summary>
        /// Gets all the diagnostics. This includes lexer, parser, and semantic diagnostics.
        /// </summary>
        public IEnumerable<Error> GetAllDiagnostics() => GetParseDiagnostics().Concat(GetSemanticDiagnostics());

        public SemanticModel GetSemanticModel() => this.lazySemanticModel.Value;

        private SemanticModel GetSemanticModelInternal()
        {
            var typeCache = new TypeCache();

            List<Symbol> declarations = new List<Symbol>();
            var declarationVisitor = new DeclarationVisitor(typeCache, declarations);
            declarationVisitor.Visit(this.ProgramSyntax);

            // TODO: Reparent context to semantic model?
            var file = new FileSymbol(typeCache, "main", this.ProgramSyntax, declarations.OfType<ParameterSymbol>());
            return new SemanticModel(file);
        }
    }
}