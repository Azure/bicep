// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.IO.Abstraction;

namespace Bicep.Core.Features;

public interface IFeatureProvider
{
    string AssemblyVersion { get; }

    IDirectoryHandle CacheRootDirectory { get; }

    bool SymbolicNameCodegenEnabled { get; }

    bool ResourceTypedParamsAndOutputsEnabled { get; }

    bool SourceMappingEnabled { get; }

    bool LegacyFormatterEnabled { get; }

    bool TestFrameworkEnabled { get; }

    bool AssertsEnabled { get; }

    bool WaitAndRetryEnabled { get; }

    bool LocalDeployEnabled { get; }

    bool ExtendableParamFilesEnabled { get; }

    bool ResourceInfoCodegenEnabled { get; }

    bool ModuleExtensionConfigsEnabled { get; }

    bool DesiredStateConfigurationEnabled { get; }

    bool UserDefinedConstraintsEnabled { get; }

    bool DeployCommandsEnabled { get; }

    bool MultilineStringInterpolationEnabled { get; }

    bool ThisNamespaceEnabled { get; }

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
                (ResourceTypedParamsAndOutputsEnabled, CoreResources.ExperimentalFeatureNames_ResourceTypedParamsAndOutputs, true, false),
                (SourceMappingEnabled, CoreResources.ExperimentalFeatureNames_SourceMapping, true, false),
                (TestFrameworkEnabled, CoreResources.ExperimentalFeatureNames_TestFramework, false, false),
                (AssertsEnabled, CoreResources.ExperimentalFeatureNames_Asserts, true, true),
                (WaitAndRetryEnabled, CoreResources.ExperimentalFeatureNames_WaitAndRetry, true, true),
                (LocalDeployEnabled, "Enable local deploy", true, true),
                (ExtendableParamFilesEnabled, "Enable extendable parameters", true, false),
                (ModuleExtensionConfigsEnabled, "Enable defining extension configs for modules", true, true),
                (DesiredStateConfigurationEnabled, "Enable defining Desired State Configuration documents", true, false),
                (UserDefinedConstraintsEnabled, "Enable @validate() decorator", true, true),
                (DeployCommandsEnabled, "Enable deploy commands", true, true),
                (MultilineStringInterpolationEnabled, "Enable multiline string interpolation", false, false),
                (ThisNamespaceEnabled, "Enable 'this' namespace", true, true),
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
