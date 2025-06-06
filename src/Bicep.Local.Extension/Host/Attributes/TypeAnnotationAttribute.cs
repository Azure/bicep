// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Bicep.Types.Concrete;

namespace Bicep.Local.Extension.Host.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class TypeAnnotationAttribute : Attribute
{
    public TypeAnnotationAttribute(
        string? description,
        ObjectTypePropertyFlags flags = ObjectTypePropertyFlags.None,
        bool isSecure = false)
    {
        Description = description;
        Flags = flags;
        IsSecure = isSecure;
    }

    public string? Description { get; }

    public ObjectTypePropertyFlags Flags { get; }

    public bool IsSecure { get; }
}

