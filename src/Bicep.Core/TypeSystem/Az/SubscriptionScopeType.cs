// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Syntax;

namespace Bicep.Core.TypeSystem.Az
{
    public class SubscriptionScopeType : ObjectType, IScopeReference
    {
        public SubscriptionScopeType(IEnumerable<FunctionArgumentSyntax> arguments, IEnumerable<TypeProperty> properties)
            : base("subscription", TypeSymbolValidationFlags.Default, properties, null)
        {
            Arguments = arguments.ToImmutableArray();
        }

        public ImmutableArray<FunctionArgumentSyntax> Arguments { get; }

        public ResourceScope Scope => ResourceScope.Subscription;
    }
}