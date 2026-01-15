// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Local.Extension.Types.Attributes;

namespace Bicep.Local.Extension.Types.Models;

/// <summary>
/// Represents a registration for a fallback resource type used in dependency resolution.
/// </summary>
/// <param name="FallbackResourceType">The type of the fallback resource that will be used when needing dynamic type management in Bicep extensions.</param>
public record FallbackTypeRegistration(Type FallbackResourceType);

/// <summary>
/// Represents a definition of a resource type, encapsulating its associated type and attributes.
/// </summary>
/// <param name="Type">The type of the resource being defined. This parameter specifies the .NET type associated with the resource.</param>
/// <param name="Attribute">The attributes associated with the resource type, providing metadata for its usage and behavior.</param>
public record ResourceTypeDefinitionDetails(Type Type, ResourceTypeAttribute Attribute);
