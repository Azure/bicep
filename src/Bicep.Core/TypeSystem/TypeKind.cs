﻿namespace Bicep.Core.TypeSystem
{
    public enum TypeKind
    {
        /// <summary>
        /// The error type.
        /// </summary>
        Error,

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
        Resource
    }
}