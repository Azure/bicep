using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Parser;
using Bicep.Core.Syntax;

namespace Bicep.Core.SemanticModel
{
    public class FileSymbol : DeclaredSymbol
    {
        public FileSymbol(ISemanticContext context, string name, ProgramSyntax declaringSyntax, IEnumerable<ParameterSymbol> parameterDeclarations)
            : base(context, name, declaringSyntax)
        {
            this.ParameterDeclarations = parameterDeclarations.ToImmutableArray();
        }

        public ImmutableArray<ParameterSymbol> ParameterDeclarations { get; }
        
        public override SymbolKind Kind => SymbolKind.File;

        public override IEnumerable<Error> GetErrors()
        {
            foreach (IGrouping<string, ParameterSymbol> group in this.ParameterDeclarations.GroupBy(paramDecl => paramDecl.Name))
            {
                foreach (ParameterSymbol duplicatedSymbol in group)
                {
                    yield return this.CreateError($"Parameter '{duplicatedSymbol.Name}' is declared several times, which is not allowed.", duplicatedSymbol.DeclaringSyntax);
                }
            }
        }
    }
}