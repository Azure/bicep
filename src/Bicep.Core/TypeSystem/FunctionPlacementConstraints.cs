using System;

namespace Bicep.Core.TypeSystem
{
    /// <summary>
    /// Represents locations where the function is allowed to be used.
    /// </summary>
    [Flags]
    public enum FunctionPlacementConstraints
    {
        /// <summary>
        /// The function can't be used anywhere. Is it really a function if it can't be used at all?
        /// </summary>
        None = 0,

        /// <summary>
        /// The function can be used in parameter defaults.
        /// </summary>
        ParameterDefaults = 1 << 0,

        /// <summary>
        /// The function can be used in variables.
        /// </summary>
        Variables = 1 << 1,

        /// <summary>
        /// The function can be used in resource declarations.
        /// </summary>
        Resources = 1 << 2,

        /// <summary>
        /// The function can be used in output declarations.
        /// </summary>
        Outputs = 1 << 3,

        /// <summary>
        /// The function can be used everywhere.
        /// </summary>
        All = ParameterDefaults | Variables | Resources | Outputs
    }
}