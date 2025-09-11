// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.IO.Abstraction;

namespace Bicep.Core.UnitTests.Configuration;

public class PatchingConfigurationManager : IConfigurationManager
{
    private readonly IConfigurationManager configurationManager;
    private readonly Func<RootConfiguration, RootConfiguration> patchFunc;

    public PatchingConfigurationManager(ConfigurationManager configurationManager, Func<RootConfiguration, RootConfiguration> patchFunc)
    {
        this.configurationManager = configurationManager;
        this.patchFunc = patchFunc;
    }

    public RootConfiguration GetConfiguration(IOUri sourceFileUri) => patchFunc(configurationManager.GetConfiguration(sourceFileUri));
}
