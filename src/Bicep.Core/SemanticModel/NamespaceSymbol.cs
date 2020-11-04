// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.SemanticModel
{
    public class NamespaceSymbol : Symbol
    {
        public NamespaceSymbol(string name, IEnumerable<FunctionOverload> functionOverloads, IEnumerable<BannedFunction> bannedFunctions)
            : base(name)
        {
            var methodResolver = new FunctionResolver(functionOverloads, bannedFunctions);
            Type = new NamespaceType(name, Enumerable.Empty<TypeProperty>(), methodResolver);
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