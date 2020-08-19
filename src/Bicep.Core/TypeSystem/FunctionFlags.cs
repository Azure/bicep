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
    }
}