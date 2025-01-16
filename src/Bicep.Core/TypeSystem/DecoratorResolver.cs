// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.TypeSystem
{
    public class DecoratorResolver
    {
        private readonly ImmutableDictionary<FunctionOverload, Decorator> decoratorsByOverloads;

        private readonly FunctionResolver functionResolver;

        public DecoratorResolver(ObjectType declaringObject, IEnumerable<Decorator> decorators)
        {
            this.decoratorsByOverloads = decorators.ToImmutableDictionary(decorator => decorator.Overload, decorator => decorator);
            this.functionResolver = new FunctionResolver(declaringObject, decoratorsByOverloads.Keys);
        }

        public Symbol? TryGetSymbol(IdentifierSyntax identifierSyntax) => this.functionResolver.TryGetSymbol(identifierSyntax);

        public Decorator? TryGetDecorator(FunctionOverload overload) => this.decoratorsByOverloads.TryGetValue(overload, out Decorator? decorator) ? decorator : null;

        public IReadOnlyDictionary<string, FunctionSymbol> GetKnownDecoratorFunctions() => this.functionResolver.GetKnownFunctions();

        public FunctionSymbol? TryGetDecoratorFunctionSymbol(string name)
            => this.functionResolver.TryGetFunctionSymbol(name);

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
