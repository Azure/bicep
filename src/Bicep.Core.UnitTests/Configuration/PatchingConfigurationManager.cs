// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using System;

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

    public RootConfiguration GetConfiguration(Uri sourceFileUri) => patchFunc(configurationManager.GetConfiguration(sourceFileUri));
}