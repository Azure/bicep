using System.Collections.Generic;
using Bicep.Core.Parser;
using Bicep.Core.Syntax;

namespace Bicep.Core.SemanticModel
{
    public class FileSymbol : DeclaredSymbol
    {
        public FileSymbol(SemanticModel containingModel, string name, ProgramSyntax declaringSyntax)
            : base(name, declaringSyntax)
        {
            this.ContainingModel = containingModel;
        }

        public override SemanticModel ContainingModel { get; }

        public IEnumerable<ParameterSymbol> GetParameterMembers() => new ParameterSymbol[0];


        public override SymbolKind Kind => SymbolKind.File;
        public override IEnumerable<Error> GetErrors()
        {
            yield break;
        }
    }
}