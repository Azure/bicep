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
        /// The current location represents the beginning of a declaration.
        /// </summary>
        DeclarationStart = 1 << 0,

        /// <summary>
        /// The current location needs a parameter type.
        /// </summary>
        ParameterType = 1 << 1,

        /// <summary>
        /// The current location needs an output type.
        /// </summary>
        OutputType = 1 << 2
    }
}