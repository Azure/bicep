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
        /// The property's value is readable at deploy-time (e.g., id, name, type, and apiVersion).
        /// </summary>
        ReadableAtDeployTime = 1 << 5,
    }
}
