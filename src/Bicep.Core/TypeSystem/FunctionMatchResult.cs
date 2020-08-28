// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Bicep.Core.TypeSystem
{
    public enum FunctionMatchResult
    {
        /// <summary>
        /// The function overload does not match the specified arguments. This may be due to a wrong number of arguments or incorrect types.
        /// </summary>
        Mismatch,

        /// <summary>
        /// The argument counts are valid but all arguments are of type "any"
        /// </summary>
        PotentialMatch,

        /// <summary>
        /// The argument counts are valid and at least one argument matched and is of non-any type.
        /// </summary>
        Match
    }
}
