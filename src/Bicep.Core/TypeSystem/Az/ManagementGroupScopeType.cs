// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Syntax;

namespace Bicep.Core.TypeSystem.Az
{
    public class ManagementGroupScopeType : NamedObjectType, IResourceScopeType
    {
        public ManagementGroupScopeType(IEnumerable<FunctionArgumentSyntax> arguments, IEnumerable<TypeProperty> properties)
            : base("managementGroup", TypeSymbolValidationFlags.Default, properties, null)
        {
            Arguments = arguments.ToImmutableArray();
        }

        public ImmutableArray<FunctionArgumentSyntax> Arguments { get; }

        public ResourceScopeType ResourceScopeType => ResourceScopeType.ManagementGroupScope;
    }
}