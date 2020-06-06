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
    }
}