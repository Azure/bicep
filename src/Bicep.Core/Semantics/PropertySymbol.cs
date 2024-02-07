// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics
{
    public class PropertySymbol(string name, string? description, TypeSymbol type) : Symbol(name)
    {
        public override SymbolKind Kind => SymbolKind.Property;

        public string? Description { get; } = description;

        public TypeSymbol Type { get; } = type;

        public override void Accept(SymbolVisitor visitor) => visitor.VisitPropertySymbol(this);
    }
}
