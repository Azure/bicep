// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Local.Extension.Types.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class BicepTypeAttribute : Attribute
{
    public BicepTypeAttribute(bool isActive = true)
    {
        IsActive = isActive;
    }

    public bool IsActive { get; set; }
}
