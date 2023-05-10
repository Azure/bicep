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

        bool ExtensibilityEnabled { get; }

        bool ResourceTypedParamsAndOutputsEnabled { get; }

        bool SourceMappingEnabled { get; }

        bool ParamsFilesEnabled { get; }

        bool UserDefinedTypesEnabled { get; }

        bool UserDefinedFunctionsEnabled { get; }

        bool DynamicTypeLoadingEnabled { get; }
    }
}
