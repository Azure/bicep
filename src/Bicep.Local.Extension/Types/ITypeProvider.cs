// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Local.Extension.Types.Attributes;

namespace Bicep.Local.Extension.Types;

/// <summary>
/// Defines a contract for discovering resource types for Bicep extensions.
/// </summary>
/// <remarks>
/// Implementations of <see cref="ITypeProvider"/> are responsible for returning applicable types to
/// be used in Bicep extensions, typically those annotated with <see cref="BicepTypeAttribute"/>.
/// </remarks>
public interface ITypeProvider
{

    Type[] GetResourceTypes();
}
