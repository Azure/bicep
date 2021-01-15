// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Semantics.Decorators;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics
{
    public class NamespaceSymbol : Symbol
    {
        public NamespaceSymbol(string name, IEnumerable<FunctionOverload> functionOverloads, IEnumerable<BannedFunction> bannedFunctions, IEnumerable<Decorator> decorators)
            : base(name)
        {
            var methodResolver = new FunctionResolver(functionOverloads, bannedFunctions);
            var decoratorResolver = new DecoratorResolver(decorators);
            Type = new NamespaceType(name, Enumerable.Empty<TypeProperty>(), methodResolver, decoratorResolver);
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