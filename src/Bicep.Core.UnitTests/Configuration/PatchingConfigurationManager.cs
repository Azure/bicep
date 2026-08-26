// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.IO.Abstraction;

namespace Bicep.Core.UnitTests.Configuration;

public class PatchingConfigurationManager : IConfigurationManager
{
    private readonly IConfigurationManager configurationManager;
    private readonly Func<IBicepConfiguration, IBicepConfiguration> patchFunc;

    public PatchingConfigurationManager(BicepConfigurationManager configurationManager, Func<IBicepConfiguration, IBicepConfiguration> patchFunc)
    {
        this.configurationManager = configurationManager;
        this.patchFunc = patchFunc;
    }

    public IBicepConfiguration GetConfiguration(IOUri sourceFileUri) => patchFunc(configurationManager.GetConfiguration(sourceFileUri));
}
