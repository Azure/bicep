// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Registry.Catalog.Implementation;

namespace Bicep.Core.Registry.Catalog;

/// <summary>
/// An IRegistryModuleMetadataProvider that will be registered as a singleton to handle
/// the public bicep registry only.
/// </summary>
public interface IPublicModuleMetadataProvider : IRegistryModuleMetadataProvider { }
