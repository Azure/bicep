// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;

namespace Bicep.Core.Registry.Indexing;

/// <summary>
/// An IRegistryModuleMetadataProvider that will be registered as a singleton to handle
/// the public bicep registry only.   asdfg create in indexer?
/// </summary>
public interface IPublicModuleMetadataProvider : IRegistryModuleMetadataProvider { }
