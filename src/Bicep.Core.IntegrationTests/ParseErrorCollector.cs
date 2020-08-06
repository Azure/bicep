using System.Collections.Generic;
using Bicep.Core.Errors;
using Bicep.Core.Parser;
using Bicep.Core.Syntax;

namespace Bicep.Core.IntegrationTests
{
    public class ParseErrorCollector : SyntaxVisitor
    {
        private readonly IList<Error> errors;
        
        public ParseErrorCollector(IList<Error> errors)
        {
            this.errors = errors;
        }

        public override void VisitSkippedTokensTriviaSyntax(SkippedTokensTriviaSyntax syntax)
        {
            this.errors.Add(syntax.ErrorInfo.WithSpan(syntax.Span));
        }
    }
}
