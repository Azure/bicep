// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Bicep.LanguageServer.Completions
{
    [Flags]
    public enum ParamsCompletionContextKind
    {
        /// <summary>
        /// No specific information about the current completion context is available.
        /// </summary>
        None = 0,

        /// <summary>
        /// The current location needs a bicep file path completion for using declaration
        /// </summary>
        UsingFilePath = 1 << 0,

        /// <summary>
        /// The current location needs a parameter completion from corresponding bicep file
        /// </summary>
        ParamAssignment = 2 << 0,
    }
}
