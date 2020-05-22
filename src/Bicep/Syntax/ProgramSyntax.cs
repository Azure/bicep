using System.Collections.Generic;
using System.Linq;
using Bicep.Parser;

namespace Bicep.Syntax
{
    public class ProgramSyntax : SyntaxBase
    {
        public ProgramSyntax(IEnumerable<SyntaxBase> statements, Token endOfFile)
        {
            Statements = statements.ToList();
            EndOfFile = endOfFile;
        }

        public IReadOnlyList<SyntaxBase> Statements { get; }

        public Token EndOfFile { get; }

        public override void Accept(SyntaxVisitor visitor)
            => visitor.VisitProgramSyntax(this);

        public override TextSpan Span
            => Statements.Any() ? 
                TextSpan.Between(Statements.First(), EndOfFile) :
                TextSpan.Between(EndOfFile, EndOfFile);
    }
}