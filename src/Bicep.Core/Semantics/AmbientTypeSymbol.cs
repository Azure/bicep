// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.Semantics;

public class AmbientTypeSymbol(string name, TypeSymbol type, NamespaceType declaringNamespace, string? description) : Symbol(name), ITypeReference
{
    public TypeSymbol Type { get; } = type;

    public NamespaceType DeclaringNamespace { get; } = declaringNamespace;

    public string? Description { get; } = description;

    public override IEnumerable<Symbol> Descendants
    {
        get
        {
            yield return this.Type;
        }
    }

    public override void Accept(SymbolVisitor visitor) => visitor.VisitAmbientTypeSymbol(this);

    public override SymbolKind Kind => SymbolKind.TypeAlias;
}
