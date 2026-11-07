// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Artifacts.References;
using Bicep.Core.Configuration;
using Bicep.Core.Features;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Artifacts.Resolution;

public sealed class ExternalArtifactContextFactory : IExternalArtifactContextFactory
{
    private readonly IBicepConfigurationManager configurationManager;
    private readonly IFeatureProviderFactory featureProviderFactory;

    public ExternalArtifactContextFactory(IBicepConfigurationManager configurationManager, IFeatureProviderFactory featureProviderFactory)
    {
        this.configurationManager = configurationManager;
        this.featureProviderFactory = featureProviderFactory;
    }

    public OciArtifactContext GetOciArtifactContext(IOUri referencingUri) => OciArtifactContext.Create(
        this.configurationManager.GetEffectiveConfiguration(referencingUri),
        this.featureProviderFactory.GetFeatureProvider(referencingUri));

    public TemplateSpecArtifactContext GetTemplateSpecArtifactContext(IOUri referencingUri) => TemplateSpecArtifactContext.Create(
        this.configurationManager.GetEffectiveConfiguration(referencingUri),
        this.featureProviderFactory.GetFeatureProvider(referencingUri));
}
