// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;

namespace Bicep.Core.Artifacts.Catalog.Brm;

/// <summary>
/// Provides metadata for Bicep Registry Modules (BRM, https://github.com/Azure/bicep-registry-modules), the modules published
/// under mcr.microsoft.com/bicep and referenced with <c>br/public:</c>. Registered as a singleton.
/// </summary>
public interface IBrmMetadataProvider : IRegistryModuleMetadataProvider { }
