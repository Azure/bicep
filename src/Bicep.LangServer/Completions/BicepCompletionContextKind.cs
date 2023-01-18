// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;

namespace Bicep.LanguageServer.Completions
{
    // TODO: investigating using https://learn.microsoft.com/en-us/dotnet/api/system.collections.bitarray?redirectedfrom=MSDN&view=net-6.0.
    [Flags]
    public enum BicepCompletionContextKind : ulong
    {
        /// <summary>
        /// No specific information about the current completion context is available.
        /// </summary>
        None = 0,

        /// <summary>
        /// The current location represents the beginning of a declaration.
        /// </summary>
        TopLevelDeclarationStart = 1UL << 0,

        /// <summary>
        /// The current location needs a parameter type.
        /// </summary>
        ParameterType = 1UL << 1,

        /// <summary>
        /// The current location needs an output type.
        /// </summary>
        OutputType = 1UL << 2,

        /// <summary>
        /// The current location needs an expression
        /// </summary>
        Expression = 1UL << 3,

        /// <summary>
        /// The current location needs an object property name.
        /// </summary>
        ObjectPropertyName = 1UL << 4,

        /// <summary>
        /// The current location needs a property value.
        /// </summary>
        PropertyValue = 1UL << 5,

        /// <summary>
        /// The current location needs an array item.
        /// </summary>
        ArrayItem = 1UL << 6,

        /// <summary>
        /// The current location needs a resource type string.
        /// </summary>
        ResourceType = 1UL << 7,

        /// <summary>
        /// The current location needs a module path.
        /// </summary>
        ModulePath = 1UL << 8,

        /// <summary>
        /// The current location needs a resource body.
        /// </summary>
        ResourceBody = 1UL << 9,

        /// <summary>
        /// The current location needs a module body.
        /// </summary>
        ModuleBody = 1UL << 10,

        /// <summary>
        /// The current location is accessing properties or methods.
        /// </summary>
        MemberAccess = 1UL << 11,

        /// <summary>
        /// The current location is accessing a nested resource.
        /// </summary>
        ResourceAccess = 1UL << 12,

        /// <summary>
        /// The current location needs target scope value.
        /// </summary>
        TargetScope = 1UL << 13,

        /// <summary>
        /// The current location needs an array index.
        /// </summary>
        ArrayIndex = 1UL << 14,

        /// <summary>
        /// The current location needs a decorator name.
        /// </summary>
        DecoratorName = 1UL << 15,

        /// <summary>
        /// The current location could be the start of a nested resource declaration.
        /// </summary>
        NestedResourceDeclarationStart = 1UL << 16,

        /// <summary>
        /// The current location needs a variable value.
        /// </summary>
        VariableValue = 1UL << 17,

        /// <summary>
        /// The current location needs an output value.
        /// </summary>
        OutputValue = 1UL << 18,

        /// <summary>
        /// The current location needs a parameter default value.
        /// </summary>
        ParameterDefaultValue = 1UL << 19,

        /// <summary>
        /// The current location is not a valid scope where we can offer completions.
        /// </summary>
        /// <remarks>This is used to prevent fallback to Expression kind</remarks>
        NotValid = 1UL << 20,

        /// <summary>
        /// The current location is after the resource type.
        /// </summary>
        ResourceTypeFollower = 1UL << 21,

        /// <summary>
        /// This is used in conjunction with ObjectPropertyName and indicates that the colon token
        /// is present in the ObjectPropertySyntax and does not need to be included in the completion.
        /// </summary>
        ObjectPropertyColonExists = 1UL << 22,

        /// <summary>
        /// We're at this place in an import statement: 'import foo |'
        /// </summary>
        ExpectingImportWithOrAsKeyword = 1UL << 23,

        /// <summary>
        /// We're at this place in an import statement: 'import | as foo'
        /// </summary>
        ExpectingImportSpecification = 1UL << 24,

        /// <summary>
        /// We're inside a function parentheses: 'someFunc(|)'
        /// </summary>
        FunctionArgument = 1UL << 25,

        /// <summary>
        /// The current location is after # sign.
        /// </summary>
        DisableNextLineDiagnosticsDirectiveStart = 1UL << 26,

        /// <summary>
        /// The current location is after '#disable-next-line |'.
        /// </summary>
        DisableNextLineDiagnosticsCodes = 1UL << 27,

        /// <summary>
        /// We're at this place in an import statement: 'import foo as bar |'
        /// </summary>
        ExpectingImportConfig = 1UL << 28,

        /// <summary>
        /// The current location needs a bicep file path completion for using declaration
        /// </summary>
        UsingFilePath = 1UL << 29,

        /// <summary>
        /// The current location needs a parameter identifier completion from corresponding bicep file
        /// </summary>
        ParamIdentifier = 1UL << 30,

        /// <summary>
        /// The current location needs a parameter value completion from allowed values in corresponding bicep file
        /// </summary>
        ParamValue = 1UL << 31,

        /// <summary>
        /// The current location is after the assignment operator in a type declaration: 'type foo = |'
        /// </summary>
        TypeDeclarationValue = 1UL << 32,

        /// <summary>
        /// The current location is after the assignment operator in a type declaration: 'type foo = |'
        /// </summary>
        ObjectTypePropertyValue = 1UL << 33,

        /// <summary>
        /// The current location is after a pipe separator within a union type: `type foo = 'foo'|'bar'|ǂ`
        /// </summary>
        UnionTypeMember = 1UL << 34,

        /// <summary>
        /// We're at this place in an import statement: 'import 'foo@1.0.0' with { foo: true } as |'
        /// </summary>
        ExpectingImportAsKeyword = 1L << 35,

        /// <summary>
        /// The current location is after the output type.
        /// </summary>
        OutputTypeFollower = 1UL << 36
    }
}
