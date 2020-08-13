using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    public static class SyntaxExtensions
    {
        public static IList<ErrorDiagnostic> GetParseDiagnostics(this SyntaxBase syntax)
        {
            var diagnostics = new List<ErrorDiagnostic>();
            var visitor = new ParseErrorVisitor(diagnostics);
            visitor.Visit(syntax);

            return diagnostics;
        }

        // TODO: Needs to account for warnings when we have warnings.
        public static bool HasParseErrors(this SyntaxBase syntax) => syntax.GetParseDiagnostics().Any();
    }
}
