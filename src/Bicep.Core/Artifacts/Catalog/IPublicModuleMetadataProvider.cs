// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Artifacts.Catalog.Implementation;

namespace Bicep.Core.Artifacts.Catalog;

/// <summary>
/// An IRegistryModuleMetadataProvider that will be registered as a singleton to handle
/// the public bicep registry only.
/// </summary>
public interface IPublicModuleMetadataProvider : IRegistryModuleMetadataProvider { }
