namespace Bicep.Core.TypeSystem
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
        /// Simple value type like string, int, bool.
        /// </summary>
        SimpleType,

        /// <summary>
        /// Object type.
        /// </summary>
        Object,

        /// <summary>
        /// Array type
        /// </summary>
        Array
    }
}