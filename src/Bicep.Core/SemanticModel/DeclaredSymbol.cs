using Bicep.Core.Syntax;

namespace Bicep.Core.SemanticModel
{
    public abstract class DeclaredSymbol : Symbol
    {
        protected DeclaredSymbol(ISemanticContext context, string name, SyntaxBase declaringSyntax)
            : base(name)
        {
            this.Context = context;
            this.DeclaringSyntax = declaringSyntax;
        }

        public ISemanticContext Context { get; }

        /// <summary>
        /// Gets the syntax node that declared this symbol.
        /// </summary>
        public SyntaxBase DeclaringSyntax { get; }
    }
}