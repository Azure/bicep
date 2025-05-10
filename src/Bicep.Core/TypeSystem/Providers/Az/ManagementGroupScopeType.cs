// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.TypeSystem.Providers.Az
{
    public class ManagementGroupScopeType : ObjectType, IScopeReference
    {
        public ManagementGroupScopeType(IEnumerable<FunctionArgumentSyntax> arguments, IEnumerable<NamedTypeProperty> properties)
            : base("managementGroup", TypeSymbolValidationFlags.Default, properties, null)
        {
            Arguments = [.. arguments];
        }

        public ImmutableArray<FunctionArgumentSyntax> Arguments { get; }

        public ResourceScope Scope => ResourceScope.ManagementGroup;
    }
}
