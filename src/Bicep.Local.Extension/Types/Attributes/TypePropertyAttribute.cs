// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Azure.Bicep.Types.Concrete;

namespace Bicep.Local.Extension.Types.Attributes;

/// <summary>
/// Specifies metadata for a property to be used in Bicep type definitions.
/// </summary>
/// <remarks>
/// Apply <see cref="TypePropertyAttribute"/> to a property in a resource type to provide additional
/// information such as a description, property flags, or to indicate that the property is secure.
/// This metadata is used by the Bicep extension framework when generating type definitions.
/// </remarks>
[AttributeUsage(AttributeTargets.Property)]
public class TypePropertyAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TypePropertyAttribute"/> class.
    /// </summary>
    /// <param name="description">A human-readable description of the property, or <c>null</c> if not specified.</param>
    /// <param name="flags">Flags that describe the property's characteristics (e.g., required, read-only).</param>
    /// <param name="isSecure">Indicates whether the property contains sensitive information and should be treated as secure.</param>
    public TypePropertyAttribute(
        string? description,
        ObjectTypePropertyFlags flags = ObjectTypePropertyFlags.None,
        bool isSecure = false)
    {
        Description = description ?? string.Empty;
        Flags = flags;
        IsSecure = isSecure;
    }

    /// <summary>
    /// Gets the human-readable description of the property.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Gets the flags that describe the property's characteristics.
    /// </summary>
    public ObjectTypePropertyFlags Flags { get; }

    /// <summary>
    /// Gets a value indicating whether the property contains sensitive information.
    /// </summary>
    public bool IsSecure { get; }
}
