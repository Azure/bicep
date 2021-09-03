// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Features
{
    public interface IFeatureProvider
    {
        public string CacheRootDirectory { get; }

        public bool RegistryEnabled { get; }

        bool SymbolicNameCodegenEnabled { get; }
    }
}
