using System.Collections.Generic;
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
            this.errors.Add(new Error(syntax.ErrorMessage, syntax.Span));
        }
    }
}