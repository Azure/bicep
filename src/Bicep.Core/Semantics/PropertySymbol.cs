// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics
{
    public class PropertySymbol : Symbol
    {
        public PropertySymbol(string name, string? description, TypeSymbol type)
            : base(name)
        {
            Description = description;
            Type = type;
        }

        public override SymbolKind Kind => SymbolKind.Property;

        public string? Description { get; }

        public TypeSymbol Type { get; }

        public override void Accept(SymbolVisitor visitor) => visitor.VisitPropertySymbol(this);
    }
}
