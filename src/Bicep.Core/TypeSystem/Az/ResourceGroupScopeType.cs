// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Syntax;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Bicep.Core.TypeSystem.Az
{
    public class ResourceGroupScopeType : ObjectType, IScopeReference
    {
        public ResourceGroupScopeType(IEnumerable<FunctionArgumentSyntax> arguments, IEnumerable<TypeProperty> properties)
            : base("resourceGroup", TypeSymbolValidationFlags.Default, properties, null)
        {
            Arguments = arguments.ToImmutableArray();
        }

        public ImmutableArray<FunctionArgumentSyntax> Arguments { get; }

        public ResourceScope Scope => ResourceScope.ResourceGroup;
    }
}
