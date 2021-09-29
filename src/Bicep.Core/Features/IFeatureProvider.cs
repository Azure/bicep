// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Features
{
    public interface IFeatureProvider
    {
        string AssemblyVersion { get; }

        string CacheRootDirectory { get; }

        bool RegistryEnabled { get; }

        bool SymbolicNameCodegenEnabled { get; }

        bool ImportsEnabled { get; }
    }
}
