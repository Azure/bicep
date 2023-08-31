// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.TypeSystem;
using System.Collections.Generic;

namespace Bicep.Core.Semantics
{
    public class BuiltInNamespaceSymbol : Symbol, INamespaceSymbol
    {
        public BuiltInNamespaceSymbol(string name, NamespaceType type)
            : base(name)
        {
            Type = type;
        }

        public NamespaceType Type { get; }

        public override IEnumerable<Symbol> Descendants
        {
            get
            {
                yield return this.Type;
            }
        }

        public override void Accept(SymbolVisitor visitor) => visitor.VisitBuiltInNamespaceSymbol(this);

        public override SymbolKind Kind => SymbolKind.Namespace;

        public NamespaceType? TryGetNamespaceType() => Type;
    }
}
