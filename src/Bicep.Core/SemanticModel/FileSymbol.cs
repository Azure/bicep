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

        public override IEnumerable<Symbol> Descendants => this.ParameterDeclarations;

        public override SymbolKind Kind => SymbolKind.File;

        public ImmutableArray<ParameterSymbol> ParameterDeclarations { get; }

        public override void Accept(SymbolVisitor visitor)
        {
            visitor.VisitFileSymbol(this);
        }

        public override IEnumerable<Error> GetDiagnostics()
        {
            var duplicateParameterGroups = this.ParameterDeclarations
                .GroupBy(paramDecl => paramDecl.Name)
                .Where(group => group.Count() > 1);
            
            foreach (IGrouping<string, ParameterSymbol> group in duplicateParameterGroups)
            {
                foreach (ParameterSymbol duplicatedSymbol in group)
                {
                    yield return this.CreateError($"Parameter '{duplicatedSymbol.Name}' is declared several times, which is not allowed.", duplicatedSymbol.DeclaringSyntax);
                }
            }
        }
    }
}