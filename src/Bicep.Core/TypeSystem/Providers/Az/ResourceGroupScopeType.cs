// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.TypeSystem.Providers.Az
{
    public class ResourceGroupScopeType : ObjectType, IScopeReference
    {
        public ResourceGroupScopeType(IEnumerable<FunctionArgumentSyntax> arguments, IEnumerable<NamedTypeProperty> properties)
            : base("resourceGroup", TypeSymbolValidationFlags.Default, properties, null)
        {
            Arguments = [.. arguments];
        }

        public ImmutableArray<FunctionArgumentSyntax> Arguments { get; }

        public ResourceScope Scope => ResourceScope.ResourceGroup;
    }
}
