// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics
{
    public class NamespaceSymbol : Symbol
    {
        public NamespaceSymbol(string name, IEnumerable<FunctionOverload> functionOverloads, IEnumerable<BannedFunction> bannedFunctions, IEnumerable<Decorator> decorators)
            : base(name)
        {
            Type = new NamespaceType(name, Enumerable.Empty<TypeProperty>(), functionOverloads, bannedFunctions, decorators);
        }

        public NamespaceType Type { get; }

        public override IEnumerable<Symbol> Descendants
        {
            get
            {
                yield return this.Type;
            }
        }

        public override void Accept(SymbolVisitor visitor) => visitor.VisitNamespaceSymbol(this);

        public override SymbolKind Kind => SymbolKind.Namespace;
    }
}