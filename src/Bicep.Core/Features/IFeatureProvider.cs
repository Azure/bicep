// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.IO.Abstraction;

namespace Bicep.Core.Features;

public interface IFeatureProvider
{
    string AssemblyVersion { get; }

    IDirectoryHandle CacheRootDirectory { get; }

    bool SymbolicNameCodegenEnabled { get; }

    bool ExtensibilityEnabled { get; }

    bool ResourceTypedParamsAndOutputsEnabled { get; }

    bool SourceMappingEnabled { get; }

    bool LegacyFormatterEnabled { get; }

    bool TestFrameworkEnabled { get; }

    bool AssertsEnabled { get; }

    bool WaitAndRetryEnabled { get; }

    bool OnlyIfNotExistsEnabled { get; }

    bool LocalDeployEnabled { get; }

    bool ExtendableParamFilesEnabled { get; }

    bool ResourceInfoCodegenEnabled { get; }

    bool TypedVariablesEnabled { get; }

    bool ExtensibilityV2EmittingEnabled { get; }

    bool ModuleExtensionConfigsEnabled { get; }

    bool DesiredStateConfigurationEnabled { get; }

    bool ExternalInputFunctionEnabled { get; }

    IEnumerable<(string name, bool impactsCompilation, bool usesExperimentalArmEngineFeature)> EnabledFeatureMetadata
    {
        get
        {
            // `impactsCompilation` means that the CLI will emit a warning if this feature is enabled
            // `usesExperimentalArmEngineFeature` means that the compiled JSON template will use an experimental language version and include a warning in the template metadata
            foreach (var (enabled, name, impactsCompilation, usesExperimentalArmEngineFeature) in new[]
            {
                (SymbolicNameCodegenEnabled, CoreResources.ExperimentalFeatureNames_SymbolicNameCodegen, false, false), // Symbolic name codegen is listed as not impacting compilation because it is GA
                (ResourceInfoCodegenEnabled, CoreResources.ExperimentalFeatureNames_ResourceInfoCodegen, true, true),
                (ExtensibilityEnabled, CoreResources.ExperimentalFeatureNames_Extensibility, true, true),
                (ResourceTypedParamsAndOutputsEnabled, CoreResources.ExperimentalFeatureNames_ResourceTypedParamsAndOutputs, true, false),
                (SourceMappingEnabled, CoreResources.ExperimentalFeatureNames_SourceMapping, true, false),
                (TestFrameworkEnabled, CoreResources.ExperimentalFeatureNames_TestFramework, false, false),
                (AssertsEnabled, CoreResources.ExperimentalFeatureNames_Asserts, true, true),
                (WaitAndRetryEnabled, CoreResources.ExperimentalFeatureNames_WaitAndRetry, true, true),
                (OnlyIfNotExistsEnabled, CoreResources.ExperimentalFeatureNames_OnlyIfNotExists, true, true),
                (LocalDeployEnabled, "Enable local deploy", false, false),
                (TypedVariablesEnabled, "Typed variables", true, false),
                (ExtendableParamFilesEnabled, "Enable extendable parameters", true, false),
                (ModuleExtensionConfigsEnabled, "Enable defining extension configs for modules", true, true),
                (DesiredStateConfigurationEnabled, "Enable defining Desired State Configuration documents", true, false),
                (ExternalInputFunctionEnabled, CoreResources.ExperimentalFeatureNames_ExternalInputFunction, true, false),
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
