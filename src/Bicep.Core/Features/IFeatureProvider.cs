// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Features
{
    public interface IFeatureProvider
    {
        string AssemblyVersion { get; }

        string CacheRootDirectory { get; }

        bool SymbolicNameCodegenEnabled { get; }

        bool ExtensibilityEnabled { get; }

        bool ResourceTypedParamsAndOutputsEnabled { get; }

        bool SourceMappingEnabled { get; }

        bool UserDefinedTypesEnabled { get; }

        bool UserDefinedFunctionsEnabled { get; }

        bool DynamicTypeLoading { get; }
        bool PrettyPrintingEnabled { get; }
    }
}
