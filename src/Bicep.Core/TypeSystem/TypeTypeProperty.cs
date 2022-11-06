// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.TypeSystem;

/// <summary>
/// A TypeProperty whose type must be a <see cref="TypeType" />. Intended for use on the types of objects that themselves have no runtime value (such as namespaces).
/// </summary>
public class TypeTypeProperty : TypeProperty
{
    public TypeTypeProperty(string name, TypeType type, TypePropertyFlags flags = TypePropertyFlags.None, string? description = null) : base(name, type, flags, description)
    {
        TypeReference = type;
    }

    public new TypeType TypeReference { get; }
}
