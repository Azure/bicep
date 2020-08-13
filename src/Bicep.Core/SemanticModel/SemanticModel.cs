using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parser;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.SemanticModel
{
    public class SemanticModel : ISemanticContext
    {
        private readonly TypeManager typeManager;

        public SemanticModel(FileSymbol root, TypeManager typeManager)
        {
            this.Root = root;
            this.typeManager = typeManager;
        }

        /// <summary>
        /// Gets all the parser and lexer diagnostics unsorted. Does not include diagnostics from the semantic model.
        /// </summary>
        public IEnumerable<Diagnostic> GetParseDiagnostics() => this.Root.DeclaringSyntax.GetParseDiagnostics();

        /// <summary>
        /// Gets all the semantic diagnostics unsorted. Does not include parser and lexer diagnostics.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ErrorDiagnostic> GetSemanticDiagnostics()
        {
            var diagnostics = new List<ErrorDiagnostic>();
            var visitor = new SemanticErrorVisitor(diagnostics);
            visitor.Visit(this.Root);

            return diagnostics;
        }

        /// <summary>
        /// Gets all the diagnostics sorted by span position ascending. This includes lexer, parser, and semantic diagnostics.
        /// </summary>
        public IEnumerable<Diagnostic> GetAllDiagnostics() => GetParseDiagnostics()
            .Concat(GetSemanticDiagnostics())
            .OrderBy(diag => diag.Span.Position);

        /// <summary>
        /// Gets the file that was compiled.
        /// </summary>
        public FileSymbol Root { get; }

        public TypeSymbol GetTypeInfo(SyntaxBase syntax) => this.typeManager.GetTypeInfo(syntax);

        public TypeSymbol? GetTypeByName(string? typeName) => this.typeManager.GetTypeByName(typeName);
    }
}