// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using Bicep.Core.Resources;
using Bicep.Core.Semantics;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Emit
{
    public static class EmitHelpers
    {
        /// <summary>
        /// Gets the resource type reference from a resource symbol, assuming it has already been type-checked.
        /// Works for single resources and resource collections.
        /// </summary>
        /// <param name="resourceSymbol">The resource symbol</param>
        /// <exception cref="ArgumentException">If the resource symbol is not for a valid resource type.</exception>
        public static ResourceTypeReference GetTypeReference(ResourceSymbol resourceSymbol)
        {
            // TODO: come up with safety mechanism to ensure type checking has already occurred
            return resourceSymbol.Type switch
            {
                ResourceType resourceType => resourceType.TypeReference,
                ArrayType { Item: ResourceType resourceType } => resourceType.TypeReference,

                // throw here because the semantic model should be completely valid at this point
                // (it's a code defect if it some errors were not emitted)
                _ => throw new ArgumentException($"Resource symbol does not have a valid type (found {resourceSymbol.Type.Name})")
            };
        }

        public static ResourceType? TryGetResourceType(ResourceSymbol resourceSymbol) => resourceSymbol.Type switch
        {
            ResourceType resourceType => resourceType,
            ArrayType { Item: ResourceType resourceType } => resourceType,
            _ => null,
        };

        public static ModuleType? TryGetModuleType(ModuleSymbol moduleSymbol) => moduleSymbol.Type switch
        {
            ModuleType moduleType => moduleType,
            ArrayType { Item: ModuleType moduleType } => moduleType,
            _ => null,
        };

        public static ObjectType? TryGetResourceBodyObjectType(ResourceSymbol resourceSymbol) => EmitHelpers.TryGetResourceType(resourceSymbol) switch
        {
            ResourceType { Body: { Type: ObjectType bodyObjectType } } => bodyObjectType,
            _ => null,
        };

        public static ObjectType? TryGetModuleBodyObjectType(ModuleSymbol moduleSymbol) => EmitHelpers.TryGetModuleType(moduleSymbol) switch
        {
            ModuleType { Body: { Type: ObjectType bodyObjectType } } => bodyObjectType,
            _ => null,
        };
    }
}
