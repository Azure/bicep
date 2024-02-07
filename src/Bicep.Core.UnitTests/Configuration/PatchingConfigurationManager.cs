// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;

namespace Bicep.Core.UnitTests.Configuration;

public class PatchingConfigurationManager(ConfigurationManager configurationManager, Func<RootConfiguration, RootConfiguration> patchFunc) : IConfigurationManager
{
    private readonly IConfigurationManager configurationManager = configurationManager;
    private readonly Func<RootConfiguration, RootConfiguration> patchFunc = patchFunc;

    public RootConfiguration GetConfiguration(Uri sourceFileUri) => patchFunc(configurationManager.GetConfiguration(sourceFileUri));
}
