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

    bool LocalDeployEnabled { get; }

    bool ResourceDerivedTypesEnabled { get; }

    bool ExtendableParamFilesEnabled { get; }

    bool SecureOutputsEnabled { get; }

    bool ResourceInfoCodegenEnabled { get; }
    
    bool TypedVariablesEnabled { get; }

    bool ExtensibilityV2EmittingEnabled { get; }

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
                (WaitAndRetryEnabled, CoreResources.ExperimentalFeatureNames_WaitAndRetry, true, false),
                (LocalDeployEnabled, "Enable local deploy", false, false),
                (ResourceDerivedTypesEnabled, CoreResources.ExperimentalFeatureNames_ResourceDerivedTypes, true, false),
                (SecureOutputsEnabled, CoreResources.ExperimentalFeatureNames_SecureOutputs, true, false),
                (TypedVariablesEnabled, "Typed variables", true, false),
                (ExtendableParamFilesEnabled, "Enable extendable parameters", true, false),
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
