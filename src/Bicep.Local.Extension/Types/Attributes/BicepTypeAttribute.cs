// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Local.Extension.Types.Attributes;

/// <summary>
/// Marks a class as a Bicep resource type for discovery by Bicep extension frameworks.
/// </summary>
/// <remarks>
/// Apply <see cref="BicepTypeAttribute"/> to a class to indicate that it should be treated as a resource type
/// and included in type discovery by implementations of <see cref="ITypeProvider"/>.
/// Only one instance of this attribute can be applied to a class.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class BicepTypeAttribute : Attribute
{ }
