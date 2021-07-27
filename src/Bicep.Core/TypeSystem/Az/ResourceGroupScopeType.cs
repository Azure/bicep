// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Extensions;
using Bicep.Core.Syntax;

namespace Bicep.Core.TypeSystem.Az
{
    public class ResourceGroupScopeType : ObjectType, IScopeReference
    {
        public ResourceGroupScopeType(IResourceTypeProvider typeProvider, IEnumerable<FunctionArgumentSyntax> arguments, IEnumerable<TypeProperty> properties)
            : base("resourceGroup", TypeSymbolValidationFlags.Default, properties, null, TypePropertyFlags.None, AzFunctionProvider.GetResourceOverload(typeProvider).AsEnumerable())
        {
            Arguments = arguments.ToImmutableArray();
        }

        public ImmutableArray<FunctionArgumentSyntax> Arguments { get; }

        public ResourceScope Scope => ResourceScope.ResourceGroup;
    }
}