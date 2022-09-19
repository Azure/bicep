// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Azure.Bicep.Types.Concrete;
using ConcreteResourceType = Azure.Bicep.Types.Concrete.ResourceType;

namespace Bicep.Core.TypeSystem.ThirdParty
{
    public interface ITypeLoader
    {
        ConcreteResourceType LoadResourceType(TypeLocation location);

        ResourceFunctionType LoadResourceFunctionType(TypeLocation location);

        TypeIndex GetIndexedTypes();
    }
}
