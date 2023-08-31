// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.TypeSystem;
using System.Collections.Generic;

namespace Bicep.Core.Semantics;

public class AmbientTypeSymbol : Symbol, ITypeReference
{
    public AmbientTypeSymbol(string name, TypeSymbol type, NamespaceType declaringNamespace, string? description) : base(name)
    {
        Type = type;
        DeclaringNamespace = declaringNamespace;
        Description = description;
    }

    public TypeSymbol Type { get; }

    public NamespaceType DeclaringNamespace { get; }

    public string? Description { get; }

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
