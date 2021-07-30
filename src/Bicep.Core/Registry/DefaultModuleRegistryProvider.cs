// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.FileSystem;
using System.Collections.Immutable;

namespace Bicep.Core.Registry
{
    public class DefaultModuleRegistryProvider : IModuleRegistryProvider
    {
        private readonly IFileResolver fileResolver;

        public DefaultModuleRegistryProvider(IFileResolver fileResolver)
        {
            this.fileResolver = fileResolver;
        }

        public ImmutableArray<IModuleRegistry> Registries => new IModuleRegistry[]
{
            new LocalModuleRegistry(this.fileResolver)
        }.ToImmutableArray();
    }
}
