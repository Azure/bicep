using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Syntax;

namespace Bicep.Core.Parser
{
    /// <summary>
    /// Visitor responsible for collecting all the parse errors from the parse tree.
    /// </summary>
    public class ParseErrorVisitor : SyntaxVisitor
    {
        private readonly IList<Diagnostic> errors;
        
        public ParseErrorVisitor(IList<Diagnostic> errors)
        {
            this.errors = errors;
        }

        public override void VisitProgramSyntax(ProgramSyntax syntax)
        {
            base.VisitProgramSyntax(syntax);

            foreach (var error in syntax.LexicalErrors)
            {
                this.errors.Add(error);
            }
        }

        public override void VisitSkippedTokensTriviaSyntax(SkippedTokensTriviaSyntax syntax)
        {
            // parse errors live on skipped token nodes

            // for errors caused by newlines, shorten the span to 1 character to avoid spilling the error over multiple lines
            // VS code will put squiggles on the entire word at that location even for a 0-length span (coordinates in the problems view will be accurate though)

            var errorInfo = syntax.ErrorCause.Type == TokenType.NewLine
                ? syntax.ErrorInfo.WithSpan(new TextSpan(syntax.ErrorInfo.Span.Position, 0))
                : syntax.ErrorInfo;

            this.errors.Add(errorInfo);
        }

        public override void VisitIdentifierSyntax(IdentifierSyntax syntax)
        {
            if (syntax.IdentifierName.Length > LanguageConstants.MaxIdentifierLength)
            {
                this.errors.Add(DiagnosticBuilder.ForPosition(syntax.Identifier).IdentifierNameExceedsLimit());
            }

            base.VisitIdentifierSyntax(syntax);
        }

        public override void VisitObjectSyntax(ObjectSyntax syntax)
        {
            base.VisitObjectSyntax(syntax);

            var duplicatedProperties = syntax.Properties
                .GroupBy(propertySyntax => propertySyntax.Identifier.IdentifierName)
                .Where(group => group.Count() > 1);

            foreach (IGrouping<string, ObjectPropertySyntax> group in duplicatedProperties)
            {
                foreach (ObjectPropertySyntax duplicatedProperty in group)
                {
                    this.errors.Add(DiagnosticBuilder.ForPosition(duplicatedProperty.Identifier).PropertyMultipleDeclarations(duplicatedProperty.Identifier.IdentifierName));
                }
            }
        }
    }
}