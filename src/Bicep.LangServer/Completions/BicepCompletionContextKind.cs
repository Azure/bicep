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
        TopLevelDeclarationStart = 1 << 0,

        /// <summary>
        /// The current location needs a parameter type.
        /// </summary>
        ParameterType = 1 << 1,

        /// <summary>
        /// The current location needs an output type.
        /// </summary>
        OutputType = 1 << 2,

        /// <summary>
        /// The current location needs an expression
        /// </summary>
        Expression = 1 << 3,

        /// <summary>
        /// The current location needs an object property name. 
        /// </summary>
        ObjectPropertyName = 1 << 4,

        /// <summary>
        /// The current location needs a property value.
        /// </summary>
        PropertyValue = 1 << 5,

        /// <summary>
        /// The current location needs an array item.
        /// </summary>
        ArrayItem = 1 << 6,

        /// <summary>
        /// The current location needs a resource type string.
        /// </summary>
        ResourceType = 1 << 7,

        /// <summary>
        /// The current location needs a module path.
        /// </summary>
        ModulePath = 1 << 8,

        /// <summary>
        /// The current location needs a resource body.
        /// </summary>
        ResourceBody = 1 << 9,

        /// <summary>
        /// The current location needs a module body.
        /// </summary>
        ModuleBody = 1 << 10,

        /// <summary>
        /// The current location is accessing properties or methods.
        /// </summary>
        MemberAccess = 1 << 11,

        /// <summary>
        /// The current location is accessing a nested resource.
        /// </summary>
        ResourceAccess = 1 << 12,
        
        /// <summary>
        /// The current location needs target scope value.
        /// </summary>
        TargetScope = 1 << 13,

        /// <summary>
        /// The current location needs an array index.
        /// </summary>
        ArrayIndex = 1 << 14,

        /// <summary>
        /// The current location needs a decorator name.
        /// </summary>
        DecoratorName = 1 << 15,

        /// <summary>
        /// The current location could be the start of a nested resource declaration.
        /// </summary>
        NestedResourceDeclarationStart = 1 << 16,

        /// <summary>
        /// The current location needs a variable value.
        /// </summary>
        VariableValue = 1 << 17,

        /// <summary>
        /// The current location needs an output value.
        /// </summary>
        OutputValue = 1 << 18,

        /// <summary>
        /// The current location needs a parameter default value.
        /// </summary>
        ParameterDefaultValue = 1 << 19,

        /// <summary>
        /// The current location is not a valid scope where we can offer completions.
        /// </summary>
        /// <remarks>This is used to prevent fallback to Expression kind</remarks>
        NotValid = 1 << 20,

        /// <summary>
        /// The current location is after the resource type.
        /// </summary>
        ResourceTypeFollower = 1 << 21,

        /// <summary>
        /// This is used in conjunction with ObjectPropertyName and indicates that the colon token
        /// is present in the ObjectPropertySyntax and does not need to be included in the completion.
        /// </summary>
        ObjectPropertyColonExists = 1 << 22,

        /// <summary>
        /// We're at this place in an import statement: 'import foo |'
        /// </summary>
        ImportSymbolFollower = 1 << 23,

        /// <summary>
        /// We're at this place in an import statement: 'import foo from |'
        /// </summary>
        ImportFromFollower = 1 << 24,
    }
}
