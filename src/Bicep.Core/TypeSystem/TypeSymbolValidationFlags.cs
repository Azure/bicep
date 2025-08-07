// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
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
        /// Indicates that this type will be a String file path to a JSON file and we should offer completions for it where files with .json extension are prioritised
        /// </summary>
        IsStringJsonFilePath = 1 << 5,

        /// <summary>
        /// Indicates that this type will be a String file path to a YAML file and we should offer completions for it where files with .yaml and .yml extension are prioritised
        /// </summary>
        IsStringYamlFilePath = 1 << 6,

        /// <summary>
        /// Indicates that this type will be a string that contains a fully qualified resource type (e.g., 'Microsoft.Resource/deployments@2022-09-01').
        /// </summary>
        IsResourceTypeIdentifier = 1 << 7,

        /// <summary>
        /// Display warning diagnostics instead of errors if an unknown property is accessed or supplied, a required property is not provided, a read-only property is supplied, or a write-only property is accessed.
        /// </summary>
        WarnOnPropertyTypeMismatch = 1 << 8,

        /// <summary>
        /// Indicates that the string represents a folder path and we should offer completions for it.
        /// </summary>
        IsStringDirectoryPath = 1 << 9,
    }
}
