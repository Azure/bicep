using Bicep.Core.Syntax;

namespace Bicep.Core.Visitors
{
    class ResolverVisitor : ErrorVisitor
    {
        public ResolverVisitor(Scope globalScope)
            : base(globalScope)
        {
        }

        public override void VisitInputDeclSyntax(InputDeclSyntax syntax)
        {
            var identifierName = syntax.Identifier.GetName();
            if (CurrentScope.Declarations.TryGetValue(identifierName, out var declaringSyntax))
            {
                AddError(syntax, $"Identifier '{identifierName}' has already been declared at {declaringSyntax.Span}");
                return;
            }

            CurrentScope.Declarations[identifierName] = syntax;
        }

        public override void VisitProgramSyntax(ProgramSyntax syntax)
        {
            foreach (var statement in syntax.Statements)
            {
                Visit(statement);
            }
        }

        public override void VisitResourceDeclSyntax(ResourceDeclSyntax syntax)
        {
            var identifierName = syntax.Identifier.GetName();
            if (CurrentScope.Declarations.TryGetValue(identifierName, out var declaringSyntax))
            {
                AddError(syntax, $"Identifier '{identifierName}' has already been declared at {declaringSyntax.Span}");
                return;
            }

            CurrentScope.Declarations[identifierName] = syntax;
        }

        public override void VisitVarDeclSyntax(VarDeclSyntax syntax)
        {
            var identifierName = syntax.Identifier.GetName();
            if (CurrentScope.Declarations.TryGetValue(identifierName, out var declaringSyntax))
            {
                AddError(syntax, $"Identifier '{identifierName}' has already been declared at {declaringSyntax.Span}");
                return;
            }

            CurrentScope.Declarations[identifierName] = syntax;
        }
    }
}