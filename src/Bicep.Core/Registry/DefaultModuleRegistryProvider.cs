// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using System.Collections.Immutable;

namespace Bicep.Core.Registry
{
    public class DefaultModuleRegistryProvider : IModuleRegistryProvider
    {
        private readonly IFileResolver fileResolver;
        private readonly IContainerRegistryClientFactory clientFactory;
        private readonly ITemplateSpecRepositoryFactory templateSpecRepositoryFactory;
        private readonly IFeatureProvider features;

        public DefaultModuleRegistryProvider(IFileResolver fileResolver, IContainerRegistryClientFactory clientFactory, ITemplateSpecRepositoryFactory templateSpecRepositoryFactory, IFeatureProvider features)
        {
            this.fileResolver = fileResolver;
            this.clientFactory = clientFactory;
            this.templateSpecRepositoryFactory = templateSpecRepositoryFactory;
            this.features = features;
        }

        public ImmutableArray<IModuleRegistry> Registries
        {
            get
            {
                var builder = ImmutableArray.CreateBuilder<IModuleRegistry>();
                builder.Add(new LocalModuleRegistry(this.fileResolver));
                if(features.RegistryEnabled)
                {
                    builder.Add(new OciModuleRegistry(this.fileResolver, this.clientFactory, this.features));
                    builder.Add(new TemplateSpecModuleRegistry(this.fileResolver, this.templateSpecRepositoryFactory, this.features));
                }

                return builder.ToImmutableArray();
            }
        }
    }
}
