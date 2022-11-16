// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;

namespace Bicep.LanguageServer.Completions
{
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
        ImportProviderFollower = 1 << 23,

        /// <summary>
        /// We're at this place in an import statement: 'import | as foo'
        /// </summary>
        ImportFollower = 1 << 24,

        /// <summary>
        /// We're inside a function parentheses: 'someFunc(|)'
        /// </summary>
        FunctionArgument = 1 << 25,

        /// <summary>
        /// The current location is after # sign.
        /// </summary>
        DisableNextLineDiagnosticsDirectiveStart = 1 << 26,

        /// <summary>
        /// The current location is after '#disable-next-line |'.
        /// </summary>
        DisableNextLineDiagnosticsCodes = 1 << 27,

        /// <summary>
        /// We're at this place in an import statement: 'import foo as bar |'
        /// </summary>
        ImportAliasFollower = 1 << 28,

        /// <summary>
        /// The current location needs a bicep file path completion for using declaration
        /// </summary>
        UsingFilePath = 1 << 29,

        /// <summary>
        /// The current location needs a parameter identifier completion from corresponding bicep file
        /// </summary>
        ParamIdentifier = 1 << 30,

        /// <summary>
        /// The current location needs a parameter value completion from allowed values in corresponding bicep file
        /// </summary>
        ParamValue = ((ulong) 1) << 31,

        /// <summary>
        /// The current location is after the assignment operator in a type declaration: 'type foo = |'
        /// </summary>
        TypeDeclarationValue = ((ulong) 1) << 32,

        /// <summary>
        /// The current location is after the assignment operator in a type declaration: 'type foo = |'
        /// </summary>
        ObjectTypePropertyValue = ((ulong) 1) << 33,

        /// <summary>
        /// The current location is after a pipe separator within a union type: `type foo = 'foo'|'bar'|ǂ`
        /// </summary>
        UnionTypeMember = ((ulong) 1) << 34,

        /// <summary>
        /// The current location needs a repository path.
        /// </summary>
        ModuleReferenceRegistryName = ((ulong)1) << 36,

        /// <summary>
        /// The current location needs a repository path.
        /// </summary>
        ModuleReferenceRepositoryPath = ((ulong)1) << 37,

        /// <summary>
        /// The current location needs a repository path.
        /// </summary>
        ModuleRegistryAliasCompletionStart = ((ulong)1) << 38,

        /// <summary>
        /// The current location needs a repository path.
        /// </summary>
        McrPublicModuleRegistryStart = ((ulong)1) << 39,

        /// <summary>
        /// The current location needs a repository path.
        /// </summary>
        McrPublicModuleRegistryTag = ((ulong)1) << 40,

        /// <summary>
        /// The current location needs a repository path.
        /// </summary>
        AcrModuleRegistryStart = ((ulong)1) << 41,
    }
}
