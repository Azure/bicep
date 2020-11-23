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
        /// </summary>
        /// <param name="resourceSymbol">The resource symbol</param>
        /// <exception cref="ArgumentException">If the resource symbol is not for a valid resource type.</exception>
        public static ResourceTypeReference GetTypeReference(ResourceSymbol resourceSymbol)
        {
            // TODO: come up with safety mechanism to ensure type checking has already occurred
            if (!(resourceSymbol.Type is ResourceType resourceType))
            {
                // throw here because the semantic model should be completely valid at this point
                // (it's a code defect if it some errors were not emitted)
                throw new ArgumentException($"Resource symbol does not have a valid type (found {resourceSymbol.Type.Name})");
            }

            return resourceType.TypeReference;
        }
    }
}
