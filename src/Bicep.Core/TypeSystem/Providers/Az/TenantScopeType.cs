// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.TypeSystem.Providers.Az
{
    public class TenantScopeType : ObjectType, IScopeReference
    {
        public TenantScopeType(IEnumerable<FunctionArgumentSyntax> arguments, IEnumerable<NamedTypeProperty> properties)
            : base("tenant", TypeSymbolValidationFlags.Default, properties, null)
        {
            Arguments = [.. arguments];
        }

        public ImmutableArray<FunctionArgumentSyntax> Arguments { get; }

        public ResourceScope Scope => ResourceScope.Tenant;
    }
}
