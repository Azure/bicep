// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
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
        /// The function can be used as a variable decorator.
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
        /// The  function can be used as an extension decorator.
        /// </summary>
        ExtensionDecorator = 1 << 7,

        /// <summary>
        /// The function can be used in direct assignment to a module parameter with @secure decorator
        /// </summary>
        ModuleSecureParameterOnly = 1 << 8,

        /// <summary>
        /// The function can be used as a metadata decorator.
        /// </summary>
        MetadataDecorator = 1 << 9,

        /// <summary>
        /// The function can be used as a type decorator.
        /// </summary>
        TypeDecorator = 1 << 10,

        GenerateIntermediateVariableAlways = 1 << 11,

        GenerateIntermediateVariableOnIndirectAssignment = 1 << 12,

        /// <summary>
        /// The function can be used as a function decorator.
        /// </summary>
        FunctionDecorator = 1 << 13,

        /// <summary>
        /// The function can be used in direct assignment only
        /// </summary>
        DirectAssignment = 1 << 14,

        /// <summary>
        /// The function does not depend on argument values - e.g. `nameof(foo)` does not depend on the value of `foo`.
        /// </summary>
        IsArgumentValueIndependent = 1 << 15,

        /// <summary>
        /// The function represents a request for inputs from external tooling.
        /// </summary>
        ExternalInput = 1 << 16,

        /// <summary>
        /// The function can be used as a resource or module decorator.
        /// </summary>
        ResourceOrModuleDecorator = ResourceDecorator | ModuleDecorator,

        /// <summary>
        /// The function can be used as a parameter or type decorator.
        /// </summary>
        ParameterOrTypeDecorator = ParameterDecorator | TypeDecorator,

        /// <summary>
        /// The function can be used as a parameter, output, or type decorator.
        /// </summary>
        ParameterOutputOrTypeDecorator = ParameterDecorator | OutputDecorator | TypeDecorator,

        /// <summary>
        /// The function can be used as a type or variable decorator.
        /// </summary>
        TypeVariableOrFunctionDecorator = TypeDecorator | VariableDecorator | FunctionDecorator,

        /// <summary>
        /// The function can be used as a decorator anywhere.
        /// </summary>
        AnyDecorator = ParameterDecorator | VariableDecorator | FunctionDecorator | ResourceDecorator | ModuleDecorator | OutputDecorator | ExtensionDecorator | MetadataDecorator | TypeDecorator,
    }
}
