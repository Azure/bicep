// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.TypeSystem.Providers.Az
{
    public class SubscriptionScopeType : ObjectType, IScopeReference
    {
        public SubscriptionScopeType(IEnumerable<FunctionArgumentSyntax> arguments, IEnumerable<NamedTypeProperty> properties)
            : base("subscription", TypeSymbolValidationFlags.Default, properties, null)
        {
            Arguments = [.. arguments];
        }

        public ImmutableArray<FunctionArgumentSyntax> Arguments { get; }

        public ResourceScope Scope => ResourceScope.Subscription;
    }
}
