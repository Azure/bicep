using Bicep.Core.Syntax;

namespace Bicep.Core.SemanticModel
{
    public class DeclarationVisitor: SyntaxVisitor
    {
        public override void VisitParameterDeclarationSyntax(ParameterDeclarationSyntax syntax)
        {
            base.VisitParameterDeclarationSyntax(syntax);


        }
    }
}
