// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;

namespace Bicep.LanguageServer.Completions
{
    [Flags]
    public enum BicepCompletionContextKind
    {
        /// <summary>
        /// No specific information about the current completion context is available.
        /// </summary>
        None = 0,

        /// <summary>
        /// The current location represents 
        /// </summary>
        Declaration = 1
    }
}