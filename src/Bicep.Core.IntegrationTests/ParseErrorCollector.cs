using System.Collections.Generic;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parser;
using Bicep.Core.Syntax;

namespace Bicep.Core.IntegrationTests
{
    public class ParseErrorCollector : SyntaxVisitor
    {
        private readonly IList<ErrorDiagnostic> errors;
        
        public ParseErrorCollector(IList<ErrorDiagnostic> errors)
        {
            this.errors = errors;
        }

        public override void VisitSkippedTokensTriviaSyntax(SkippedTokensTriviaSyntax syntax)
        {
            this.errors.Add(syntax.ErrorInfo.WithSpan(syntax.Span));
        }
    }
}
