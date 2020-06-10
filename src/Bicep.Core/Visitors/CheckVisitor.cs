using System;
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

            string parameterType = syntax.Type.IdentifierName;
            bool parameterTypeValid = LanguageConstants.PropertyTypes.Contains(parameterType);
            if (!parameterTypeValid)
            {
                this.AddError($"The parameter type is not valid. Please specify one of the following types: {LanguageConstants.PropertyTypesString}", syntax.Type);
            }
            
            if(syntax.Value != null)
            {
                // check value type matches type
                // TODO: Type equality should be done by the semantic model
                if (parameterTypeValid && string.Equals(parameterType, GetTypeInfo(syntax.Value), StringComparison.Ordinal) == false)
                {
                    this.AddError("The parameter type does not match the type of the default value.", syntax.Value);
                }
            }
        }

        public override void VisitIdentifierSyntax(IdentifierSyntax syntax)
        {
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

        private static string? GetTypeInfo(SyntaxBase syntax)
        {
            // TODO: This needs to be handled by the semantic model and return a better type
            switch (syntax)
            {
                case BooleanLiteralSyntax _:
                    return LanguageConstants.BooleanType;

                case NumericLiteralSyntax _:
                    return LanguageConstants.IntegerType;

                case StringSyntax _:
                    return LanguageConstants.StringType;

                default:
                    return null;
            }
        }
    }
}