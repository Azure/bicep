using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Parser;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Visitors
{
    public class CheckVisitor : SyntaxVisitor
    {
        private readonly IList<Error> errors;
        private readonly TypeCache typeCache;

        public CheckVisitor(IList<Error> errors, TypeCache typeCache)
        {
            this.errors = errors;
            this.typeCache = typeCache;
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

            bool parameterTypeValid = LanguageConstants.ParameterTypes.TryGetValue(syntax.Type.TypeName, out TypeSymbol parameterType);
            if (!parameterTypeValid)
            {
                this.AddError($"The parameter type is not valid. Please specify one of the following types: {LanguageConstants.PropertyTypesString}", syntax.Type);
            }
            
            if(syntax.Value != null)
            {
                // check value type matches type
                if (parameterTypeValid && TypeSymbol.Equals(parameterType, typeCache.GetTypeInfo(syntax.Value)) == false)
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
    }
}