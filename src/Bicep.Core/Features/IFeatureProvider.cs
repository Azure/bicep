// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;

namespace Bicep.Core.Features;

public interface IFeatureProvider
{
    string AssemblyVersion { get; }

    string CacheRootDirectory { get; }

    bool SymbolicNameCodegenEnabled { get; }

    bool ExtensibilityEnabled { get; }

    bool ResourceTypedParamsAndOutputsEnabled { get; }

    bool SourceMappingEnabled { get; }

    bool UserDefinedFunctionsEnabled { get; }

    bool DynamicTypeLoadingEnabled { get; }

    bool ProviderRegistryEnabled { get; }

    bool PrettyPrintingEnabled { get; }

    bool TestFrameworkEnabled { get; }

    bool AssertsEnabled { get; }

    bool CompileTimeImportsEnabled { get; }

    bool MicrosoftGraphPreviewEnabled { get; }

    bool PublishSourceEnabled { get; }

    bool OptionalModuleNamesEnabled { get; }

    bool ResourceDerivedTypesEnabled { get; }

    IEnumerable<(string name, bool impactsCompilation, bool usesExperimentalArmEngineFeature)> EnabledFeatureMetadata
    {
        get
        {
            // `impactsCompilation` means that the CLI will emit a warning if this feature is enabled
            // `usesExperimentalArmEngineFeature` means that the compiled JSON template will use an experimental language version and include a warning in the template metadata
            foreach (var (enabled, name, impactsCompilation, usesExperimentalArmEngineFeature) in new[]
            {
                (SymbolicNameCodegenEnabled, CoreResources.ExperimentalFeatureNames_SymbolicNameCodegen, false, false), // Symbolic name codegen is listed as not impacting compilation because it is GA
                (ExtensibilityEnabled, CoreResources.ExperimentalFeatureNames_Extensibility, true, true),
                (ResourceTypedParamsAndOutputsEnabled, CoreResources.ExperimentalFeatureNames_ResourceTypedParamsAndOutputs, true, false),
                (SourceMappingEnabled, CoreResources.ExperimentalFeatureNames_SourceMapping, true, false),
                (UserDefinedFunctionsEnabled, CoreResources.ExperimentalFeatureNames_UserDefinedFunctions, true, false),
                (DynamicTypeLoadingEnabled, CoreResources.ExperimentalFeatureNames_DynamicTypeLoading, true, false),
                (ProviderRegistryEnabled, CoreResources.ExperimentalFeatureNames_ProviderRegistry, true, false),
                (PrettyPrintingEnabled, CoreResources.ExperimentalFeatureNames_PrettyPrinting, false, false),
                (TestFrameworkEnabled, CoreResources.ExperimentalFeatureNames_TestFramework, false, false),
                (AssertsEnabled, CoreResources.ExperimentalFeatureNames_Asserts, true, true),
                (CompileTimeImportsEnabled, CoreResources.ExperimentalFeatureNames_CompileTimeImports, true, false),
                (MicrosoftGraphPreviewEnabled, CoreResources.ExperimentalFeatureNames_MicrosoftGraphPreview, true, true),
                (PublishSourceEnabled, CoreResources.ExperimentalFeatureNames_PublishSource, false, false),
                (OptionalModuleNamesEnabled, CoreResources.ExperimentalFeatureNames_OptionalModuleNames, true, false),
                (ResourceDerivedTypesEnabled, CoreResources.ExperimentalFeatureNames_ResourceDerivedTypes, true, false),
            })
            {
                if (enabled)
                {
                    yield return (name, impactsCompilation, usesExperimentalArmEngineFeature);
                }
            }
        }
    }
}
