// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Bicep.Core.TypeSystem
{
    public enum DeclaredTypeFlags
    {
        /// <summary>
        /// No flags are specified.
        /// </summary>
        None = 0,

        /// <summary>
        /// The assigned value is supposed to be a compile-time constant.
        /// </summary>
        Constant = 1
    }
}