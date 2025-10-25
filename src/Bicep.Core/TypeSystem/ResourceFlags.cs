// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Bicep.Core.TypeSystem
{
    [Flags]
    public enum ResourceFlags
    {
        /// <summary>
        /// No flags specified.
        /// </summary>
        None = 0,

        /// <summary>
        /// The resource must be used with the 'existing' keyword.
        /// </summary>
        ReadOnly = 1 << 0,

        /// <summary>
        /// The resource cannot be used with the 'existing' keyword.
        /// </summary>
        WriteOnly = 1 << 1,
    }
}
