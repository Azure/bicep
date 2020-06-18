using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Parser;
using Bicep.Core.Syntax;

namespace Bicep.Core.Visitors
{
    public class CheckVisitor : SyntaxVisitor
    {
        private readonly IList<Error> errors;
        
        public CheckVisitor(IList<Error> errors)
        {
            this.errors = errors;
        }

        public override void VisitSkippedTokensTriviaSyntax(SkippedTokensTriviaSyntax syntax)
        {
            // parse errors live on skipped token nodes
            TextSpan span = TextSpan.Between(syntax.ErrorCause, syntax.Tokens.Last());
            this.errors.Add(new Error(syntax.ErrorMessage, span));
        }

        public override void VisitIdentifierSyntax(IdentifierSyntax syntax)
        {
            if (syntax.IdentifierName.Length > LanguageConstants.MaxIdentifierLength)
            {
                this.AddError($"The identifier exceeds the limit of {LanguageConstants.MaxIdentifierLength}. Reduce the length of the identifier.", syntax.Identifier);
            }

            base.VisitIdentifierSyntax(syntax);
        }

        protected void AddError(string message, IPositionable positionable)
        {
            this.errors.Add(new Error(message, positionable.Span));
        }
    }
}