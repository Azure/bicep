// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Semantics;

namespace Bicep.Core.TypeSystem
{
    public abstract class TypeSymbol : Symbol, ITypeReference
    {
        protected TypeSymbol(string name) : base(name)
        {
        }
        
        public override SymbolKind Kind => SymbolKind.Type;

        public abstract TypeKind TypeKind { get; }

        public TypeSymbol Type => this;

        public virtual TypeSymbolValidationFlags ValidationFlags { get; } = TypeSymbolValidationFlags.Default;

        public override void Accept(SymbolVisitor visitor)
        {
            visitor.VisitTypeSymbol(this);
        }

        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Returns a name for this type that was formatted for inclusion in the name of another compount type.
        /// For most types, this is the same as name.
        /// </summary>
        public virtual string FormatNameForCompoundTypes() => this.Name;

        protected string WrapTypeName() => $"({this.Name})";
    }
}
