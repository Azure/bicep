// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;

namespace Bicep.Core.TypeSystem
{
    /// <summary>
    /// Flags that may be placed on type symbols to modify their behavior.
    /// </summary>
    [Flags]
    public enum TypeSymbolValidationFlags
    {
        /// <summary>
        /// The default.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Display warning diagnostics instead of errors if there is a type mismatch.
        /// </summary>
        WarnOnTypeMismatch = 1 << 0,

        /// <summary>
        /// Permits assignment from string/int/bool to string/int/bool literal, respectively.
        /// </summary>
        AllowLooseAssignment = 1 << 1,

        /// <summary>
        /// Prevents all assignment of this type.
        /// </summary>
        PreventAssignment = 1 << 2,

        /// <summary>
        /// Allows assigning a secret reference
        /// </summary>
        IsSecure = 1 << 3,

        /// <summary>
        /// Indicates that this type will be a String file path and we should offer completions for it
        /// </summary>
        IsStringFilePath = 1 << 4,

        /// <summary>
        /// Indicates that this type will be a String file path to a JSON file and we should offer completions for it where files wih .json extension are prioritised
        /// </summary>
        IsStringJsonFilePath = 1 << 5,

        /// <summary>
        /// Indicates that this type will be a String file path to a YAML file and we should offer completions for it where files wih .yaml and .yml extension are prioritised
        /// </summary>
        IsStringYamlFilePath = 1 << 6,
    }
}
