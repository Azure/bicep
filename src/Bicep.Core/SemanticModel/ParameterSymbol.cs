using System.Collections.Generic;
using Bicep.Core.Parser;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.SemanticModel
{
    public class ParameterSymbol : DeclaredSymbol
    {
        public ParameterSymbol(FileSymbol containingFile, string name, ParameterDeclarationSyntax declaringSyntax, TypeSymbol type, SyntaxBase? defaultValue)
            : base(name, declaringSyntax)
        {
            this.ContainingFile = containingFile;
            this.Type = type;
            this.DefaultValue = defaultValue;
        }

        public FileSymbol ContainingFile { get; }

        public override SemanticModel ContainingModel => this.ContainingFile.ContainingModel;

        public TypeSymbol Type { get; }

        public SyntaxBase? DefaultValue { get; }

        public override SymbolKind Kind => SymbolKind.Parameter;

        public override IEnumerable<Error> GetErrors()
        {
            if(this.DefaultValue != null)
            {
                // check value type matches type
                TypeSymbol? defaultValueType = this.ContainingModel.GetTypeInfo(this.DefaultValue);
                
                if (this.Type.TypeKind != TypeKind.Error && TypeSymbol.Equals(this.Type, defaultValueType) == false)
                {
                    yield return this.CreateError($"The parameter expects a default value of type '{this.Type.Name}' but provided value is of type '{defaultValueType?.Name}'.", this.DefaultValue);
                }
            }
        }
    }
}