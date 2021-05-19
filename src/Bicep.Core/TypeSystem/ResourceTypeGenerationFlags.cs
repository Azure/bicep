// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;

namespace Bicep.Core.TypeSystem
{
    /// <summary>
    /// Flags that influence the generation of resource types.
    /// </summary>
    [Flags]
    public enum ResourceTypeGenerationFlags
    {
        /// <summary>
        /// Default behavior.
        /// </summary>
        None = 0,

        /// <summary>
        /// Generating a definition for a resource using the 'existing' keyword.
        /// </summary>
        ExistingResource = 1 << 0,

        /// <summary>
        /// Generating a definition for a resource which permits literal-valued names (e.g. enums, or discriminated objects with 'name' key).
        /// This is used for root-level resources, and nested/parent-property child resources.
        /// </summary>
        PermitLiteralNameProperty = 1 << 1,

        /// <summary>
        /// Generating a definition for a syntactically nested resource. Do not use this flag for resources that need the "parent" property.
        /// </summary>
        NestedResource = 1 << 2
    }
}
