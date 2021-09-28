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
        ResourceDecorator = 1 << 4,

        /// <summary>
        /// The function can be used as a module decorator.
        /// </summary>
        ModuleDecorator = 1 << 5,

        /// <summary>
        /// The  function can be used as an output decorator.
        /// </summary>
        OutputDecorator = 1 << 6,

        /// <summary>
        /// The  function can be used as an output decorator.
        /// </summary>
        ImportDecorator = 1 << 7,

        /// <summary>
        /// The function can be used in direct assignment to a module parameter with @secure decorator
        /// </summary>
        ModuleSecureParameterOnly = 1 << 8,

        /// <summary>
        /// The function can be used a resource or module decorator.
        /// </summary>
        ResourceOrModuleDecorator = ResourceDecorator | ModuleDecorator,

        /// <summary>
        /// The function can be used as a decorator anywhere.
        /// </summary>
        AnyDecorator = ParameterDecorator | VariableDecorator | ResourceDecorator | ModuleDecorator | OutputDecorator | ImportDecorator,
    }
}
