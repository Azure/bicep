// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;

namespace Bicep.Core.TypeSystem
{
    public class DecoratorResolver
    {
        private readonly ImmutableDictionary<FunctionOverload, Decorator> decoratorsByOverloads;

        private readonly FunctionResolver functionResolver;

        public DecoratorResolver(ObjectType owner, IEnumerable<Decorator> decorators)
        {
            this.decoratorsByOverloads = decorators.ToImmutableDictionary(decorator => decorator.Overload, decorator => decorator);
            this.functionResolver = new FunctionResolver(decoratorsByOverloads.Keys);
            this.Owner = owner;
        }

        public ObjectType Owner { get; }

        public Symbol? TryGetSymbol(IdentifierSyntax identifierSyntax) => this.functionResolver.TryGetSymbol(Owner, identifierSyntax);

        public Decorator? TryGetDecorator(FunctionOverload overload) => this.decoratorsByOverloads.TryGetValue(overload, out Decorator? decorator) ? decorator : null;

        public ImmutableDictionary<string, FunctionSymbol> GetKnownDecoratorFunctions() => this.functionResolver.GetKnownFunctions(Owner);

        public IEnumerable<Decorator> GetMatches(FunctionSymbol symbol, IList<TypeSymbol> argumentTypes)
        {
            foreach (var overload in FunctionResolver.GetMatches(symbol, argumentTypes, out var _, out var _))
            {
                var decorator = this.TryGetDecorator(overload);

                if (decorator != null)
                {
                    yield return decorator;
                }
            }
        }
    }
}
