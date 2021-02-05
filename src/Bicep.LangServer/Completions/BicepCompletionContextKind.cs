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
        /// The current location needs target scope value.
        /// </summary>
        TargetScope = 1 << 12,

        /// <summary>
        /// The current location needs an array index.
        /// </summary>
        ArrayIndex = 1 << 13
    }
}