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

        public override void VisitParameterDeclarationSyntax(ParameterDeclarationSyntax syntax)
        {
            base.VisitParameterDeclarationSyntax(syntax);

            Assert(syntax.ParameterKeyword.Type == TokenType.ParameterKeyword);

            if (LanguageConstants.PropertyTypes.Contains(syntax.Type.IdentifierName) == false)
            {
                this.AddError($"The property type is not valid. Please specify one of the following types: {LanguageConstants.PropertyTypesString}", syntax.Type);
            }

            if (syntax.Assignment == null)
            {
                Assert(syntax.Value == null);
            }
            else
            {
                Assert(syntax.Value != null);

                // check value type matches type
                
            }

            Assert(syntax.NewLine.Type == TokenType.NewLine);
        }

        public override void VisitIdentifierSyntax(IdentifierSyntax syntax)
        {
            Assert(syntax.Identifier.Type == TokenType.Identifier);
            Assert(string.IsNullOrEmpty(syntax.IdentifierName) == false);

            if (syntax.IdentifierName.Length > LanguageConstants.MaxIdentifierLength)
            {
                this.AddError($"The identifier exceeds the limit of {LanguageConstants.MaxIdentifierLength}. Reduce the length of the identifier.", syntax.Identifier);
            }

            base.VisitIdentifierSyntax(syntax);
        }

        protected void AddError(string message, IPositionable positionable)
        {
            this.errors.Add(new Error(message, positionable.Span));
        }

        /// <summary>
        /// Throws an exception if predicate is false. This is indended to guard against parser bugs.
        /// </summary>
        /// <param name="predicate">The predicate</param>
        protected void Assert(bool predicate)
        {
            if (predicate == false)
            {
                // we have a code defect - use the exception stack to debug
                throw new BicepInternalException("Internal parser error.");
            }
        }
    }
}