// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Bicep.Core.TypeSystem
{
    public enum TypeKind
    {
        /// <summary>
        /// The error type.
        /// </summary>
        Error,

        /// <summary>
        /// Empty union of types. 
        /// </summary>
        Never,

        /// <summary>
        /// Any type
        /// </summary>
        Any,

        /// <summary>
        /// Primitive value type like string, int, bool, object, or array.
        /// </summary>
        Primitive,

        /// <summary>
        /// Named object type. (Examples: ResourceGroup, ParameterModifier, etc.)
        /// </summary>
        NamedObject,

        /// <summary>
        /// Resource type
        /// </summary>
        Resource,

        /// <summary>
        /// Union of multiple types.
        /// </summary>
        Union,

        /// <summary>
        /// String literal type.
        /// </summary>
        StringLiteral,

        /// <summary>
        /// Discriminated object type.
        /// </summary>
        DiscriminatedObject,
    }
}