// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;

namespace Bicep.Core.TypeSystem
{
    [Flags]
    public enum TypePropertyFlags
    {
        /// <summary>
        /// No flags specified.
        /// </summary>
        None = 0,

        /// <summary>
        /// The property is required.
        /// </summary>
        Required = 1 << 0,

        /// <summary>
        /// The property only accepts compile-time constants.
        /// </summary>
        Constant = 1 << 1,

        /// <summary>
        /// The property is read-only.
        /// </summary>
        ReadOnly = 1 << 2,

        /// <summary>
        /// The property is write-only.
        /// </summary>
        WriteOnly = 1 << 3,

        /// <summary>
        /// The property only accepts deploy-time constants whose values must be known at the start of the deployment, and do not require inlining.
        /// </summary>
        DeployTimeConstant = 1 << 4,

        /// <summary>
        /// Blocks assignment of the "any" type to the property having this flag.
        /// </summary>
        DisallowAny = 1 << 5,

        /// <summary>
        /// The property's value is readable at deploy-time (e.g., id, name, type, and apiVersion).
        /// </summary>
        ReadableAtDeployTime = 1 << 6,

        /// <summary>
        /// The property must be loop-variant. In other words, the value of the property must change
        /// based on the value of the loop item or index variables. This flag has no effect outside of top-level properties.
        /// </summary>
        LoopVariant = 1 << 7,

        /// <summary>
        /// On non-required properties, this allows the property type to be treated as "<x> | null" (where <x> is the current property type)
        /// for the purposes of type checking the value assigned to the property.
        /// </summary>
        AllowImplicitNull = 1 << 8,

        /// <summary>
        /// Property that is not defined in Swagger, but still might be valid. We will show warning instead error.
        /// </summary>
        FallbackProperty = 1 << 9
    }
}
