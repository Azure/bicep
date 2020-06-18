﻿using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Parser;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.Visitors;

namespace Bicep.Core.SemanticModel
{
    public class SemanticModel : ISemanticContext
    {
        private readonly TypeCache typeCache = new TypeCache();

        public SemanticModel(FileSymbol root)
        {
            this.Root = root;
        }

        /// <summary>
        /// Gets all the parser and lexer diagnostics unsorted. Does not include diagnostics from the semantic model.
        /// </summary>
        public IEnumerable<Error> GetParseDiagnostics()
        {
            var errors = new List<Error>();
            var visitor = new CheckVisitor(errors);
            visitor.Visit(this.Root.DeclaringSyntax);

            return errors;
        }

        /// <summary>
        /// Gets all the semantic diagnostics unsorted. Does not include parser and lexer diagnostics.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Error> GetSemanticDiagnostics()
        {
            // TODO: Implement a semantic visitor or move into SemanticModel?
            return this.Root
                .GetErrors()
                .Concat(this.Root.ParameterDeclarations.SelectMany(paramDecl => paramDecl.GetErrors()));
        }

        /// <summary>
        /// Gets all the diagnostics sorted by span position ascending. This includes lexer, parser, and semantic diagnostics.
        /// </summary>
        public IEnumerable<Error> GetAllDiagnostics() => GetParseDiagnostics()
            .Concat(GetSemanticDiagnostics())
            .OrderBy(diag => diag.Span.Position);

        /// <summary>
        /// Gets the file that was compiled.
        /// </summary>
        public FileSymbol Root { get; }

        public TypeSymbol? GetTypeInfo(SyntaxBase? syntax) => this.typeCache.GetTypeInfo(syntax);

        public TypeSymbol? GetTypeByName(string typeName) => this.typeCache.GetTypeByName(typeName);
    }
}