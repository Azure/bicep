// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;

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
            return this.Name;
        }
    }
}
