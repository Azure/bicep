using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Parser;
using Bicep.Core.Syntax;

namespace Bicep.Core.Visitors
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