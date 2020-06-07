using Bicep.Core.Syntax;

namespace Bicep.Core.Visitors
{
    public class ResolverVisitor : ErrorVisitor
    {
        public ResolverVisitor(Scope globalScope)
            : base(globalScope)
        {
        }

        public override void VisitParameterDeclarationSyntax(ParameterDeclarationSyntax syntax)
        {
            var identifierName = syntax.Name.IdentifierName;
            if (CurrentScope.Declarations.TryGetValue(identifierName, out var declaringSyntax))
            {
                AddError(syntax, $"Identifier '{identifierName}' has already been declared at {declaringSyntax.Span}");
                return;
            }

            CurrentScope.Declarations[identifierName] = syntax;
        }
    }
}