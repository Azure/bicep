// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.Semantics;

public class AmbientTypeSymbol : Symbol, ITypeReference
{
    public AmbientTypeSymbol(string name, TypeSymbol type, NamespaceType declaringNamespace, TypePropertyFlags flags, string? description) : base(name)
    {
        Type = type;
        DeclaringNamespace = declaringNamespace;
        Flags = flags;
        Description = description;
    }

    public TypeSymbol Type { get; }

    public NamespaceType DeclaringNamespace { get; }

    public string? Description { get; }

    public TypePropertyFlags Flags { get; }

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
