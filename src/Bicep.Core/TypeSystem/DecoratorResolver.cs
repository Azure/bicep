// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Decorators;
using Bicep.Core.Syntax;

namespace Bicep.Core.TypeSystem
{
    public class DecoratorResolver
    {
        private readonly FunctionResolver functionResolver;

        private readonly ImmutableDictionary<FunctionSymbol, Decorator> decoratorsBySymbol;

        public DecoratorResolver(IEnumerable<Decorator> decorators)
        {
            this.functionResolver = FunctionResolver.Create(decorators.Select(decorator => decorator.Overload));
            this.decoratorsBySymbol = decorators.ToImmutableDictionary(decorator => functionResolver.GetKnownFunctions()[decorator.Overload.Name], decorator => decorator);
        }

        public Symbol? TryGetSymbol(IdentifierSyntax identifierSyntax) => this.functionResolver.TryGetSymbol(identifierSyntax);

        public ImmutableDictionary<string, FunctionSymbol> GetKnownDecoratorFunctions() => this.functionResolver.GetKnownFunctions();

        public Decorator? TryGetDecorator(FunctionSymbol symbol)
        {
            this.decoratorsBySymbol.TryGetValue(symbol, out Decorator? decorator);
            return decorator;
        }
    }
}
