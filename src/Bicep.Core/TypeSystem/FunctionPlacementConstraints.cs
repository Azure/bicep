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
        ParameterDefault = 1,

        /// <summary>
        /// The function can be used in variables.
        /// </summary>
        Variables = 2,

        /// <summary>
        /// The function can be used in resource declarations.
        /// </summary>
        Resources = 4,

        /// <summary>
        /// The function can be used in module references.
        /// </summary>
        Modules = 8,

        /// <summary>
        /// The function can be used in output declarations.
        /// </summary>
        Outputs = 16,

        /// <summary>
        /// The function can be used everywhere.
        /// </summary>
        All = ParameterDefault | Variables | Resources | Modules | Outputs
    }
}