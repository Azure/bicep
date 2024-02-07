// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.TypeSystem.Providers.Az
{
    public class SubscriptionScopeType(IEnumerable<FunctionArgumentSyntax> arguments, IEnumerable<TypeProperty> properties) : ObjectType("subscription", TypeSymbolValidationFlags.Default, properties, null), IScopeReference
    {
        public ImmutableArray<FunctionArgumentSyntax> Arguments { get; } = arguments.ToImmutableArray();

        public ResourceScope Scope => ResourceScope.Subscription;
    }
}
