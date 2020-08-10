using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Parser;
using Bicep.Core.Diagnostics;

namespace Bicep.Core.Syntax
{
    public class ProgramSyntax : SyntaxBase
    {
        public ProgramSyntax(IEnumerable<SyntaxBase> statements, Token endOfFile, IEnumerable<Diagnostic> lexicalErrors)
        {
            this.Statements = statements.ToList().AsReadOnly();
            this.EndOfFile = endOfFile;
            this.LexicalErrors = lexicalErrors.ToImmutableArray();
        }

        public IReadOnlyList<SyntaxBase> Statements { get; }

        public Token EndOfFile { get; }

        public ImmutableArray<Diagnostic> LexicalErrors { get; }

        public override void Accept(SyntaxVisitor visitor)
            => visitor.VisitProgramSyntax(this);

        public override TextSpan Span
            => Statements.Any() ? 
                TextSpan.Between(Statements.First(), EndOfFile) :
                TextSpan.Between(EndOfFile, EndOfFile);
    }
}