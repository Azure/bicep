using Bicep.Core.Syntax;

namespace Bicep.Core.SemanticModel
{
    public abstract class DeclaredSymbol : Symbol
    {
        protected DeclaredSymbol(string name, SyntaxBase declaringSyntax)
            : base(name)
        {
            this.DeclaringSyntax = declaringSyntax;
        }

        /// <summary>
        /// Gets the syntax node that declared this symbol.
        /// </summary>
        public SyntaxBase DeclaringSyntax { get; }

        public abstract SemanticModel ContainingModel { get; }
    }
}