// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.Semantics
{
    public class BuiltInNamespaceSymbol : Symbol, INamespaceSymbol
    {
        public BuiltInNamespaceSymbol(string name, TypeSymbol type)
            : base(name)
        {
            Type = type;
        }

        public TypeSymbol Type { get; }

        public override IEnumerable<Symbol> Descendants
        {
            get
            {
                yield return this.Type;
            }
        }

        public override void Accept(SymbolVisitor visitor) => visitor.VisitBuiltInNamespaceSymbol(this);

        public override SymbolKind Kind => SymbolKind.Namespace;

        public NamespaceType? TryGetNamespaceType() => Type as NamespaceType;
    }
}
