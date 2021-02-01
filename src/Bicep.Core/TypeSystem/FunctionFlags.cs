// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;

namespace Bicep.Core.TypeSystem
{
    /// <summary>
    /// Flags that may be placed on functions to modify their behavior.
    /// </summary>
    [Flags]
    public enum FunctionFlags
    {
        /// <summary>
        /// The default, no restrictions
        /// </summary>
        Default = 0,

        /// <summary>
        /// The function can only be used in parameter default values.
        /// </summary>
        ParamDefaultsOnly = 1 << 0,

        /// <summary>
        /// The function requires inlining.
        /// </summary>
        RequiresInlining = 1 << 1,

        /// <summary>
        /// The function can be used as a parameter decorator.
        /// </summary>
        ParameterDecorator = 1 << 2,

        /// <summary>
        /// The function can be used as a parameter decorator.
        /// </summary>
        VariableDecorator = 1 << 3,

        /// <summary>
        /// The function can be used as a resource decorator.
        /// </summary>
        ResoureDecorator = 1 << 4,

        /// <summary>
        /// The function can be used as a module decorator.
        /// </summary>
        ModuleDecorator = 1 << 5,

        /// <summary>
        /// The function can be used as an output decorator.
        /// </summary>
        OutputDecorator = 1 << 6,
    }
}
