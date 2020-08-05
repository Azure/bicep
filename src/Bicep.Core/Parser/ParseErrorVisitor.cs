using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Syntax;

namespace Bicep.Core.Parser
{
    /// <summary>
    /// Visitor responsible for collecting all the parse errors from the parse tree.
    /// </summary>
    public class ParseErrorVisitor : SyntaxVisitor
    {
        private readonly IList<Error> errors;
        
        public ParseErrorVisitor(IList<Error> errors)
        {
            this.errors = errors;
        }

        public override void VisitProgramSyntax(ProgramSyntax syntax)
        {
            base.VisitProgramSyntax(syntax);

            foreach (Error error in syntax.LexicalErrors)
            {
                this.errors.Add(error);
            }
        }

        public override void VisitSkippedTokensTriviaSyntax(SkippedTokensTriviaSyntax syntax)
        {
            // parse errors live on skipped token nodes

            // for errors caused by newlines, shorten the span to 1 character to avoid spilling the error over multiple lines
            // VS code will put squiggles on the entire word at that location even for a 0-length span (coordinates in the problems view will be accurate though)
            TextSpan span = syntax.ErrorCause.Type == TokenType.NewLine
                ? new TextSpan(syntax.ErrorCause.Span.Position, 0)
                : syntax.ErrorCause.Span;

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
                    this.AddError($"Property '{duplicatedProperty.Identifier.IdentifierName}' is declared multiple times in this object. Remove or rename the duplicate properties.", duplicatedProperty.Identifier);
                }
            }
        }

        protected void AddError(string message, IPositionable positionable)
        {
            this.errors.Add(new Error(message, positionable.Span));
        }
    }
}