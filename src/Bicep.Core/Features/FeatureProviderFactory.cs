// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Configuration;
using Bicep.Core.Extensions;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Features;

public class FeatureProviderFactory : IFeatureProviderFactory
{
    private readonly IConfigurationManager configurationManager;
    private readonly IFileExplorer fileExplorer;

    public FeatureProviderFactory(IConfigurationManager configurationManager, IFileExplorer fileExplorer)
    {
        this.configurationManager = configurationManager;
        this.fileExplorer = fileExplorer;
    }

    public IFeatureProvider GetFeatureProvider(Uri templateUri) => new FeatureProvider(configurationManager.GetConfiguration(templateUri.ToIOUri()), this.fileExplorer);
}
