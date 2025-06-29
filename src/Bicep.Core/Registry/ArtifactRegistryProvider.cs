// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.Core.Registry
{
    public abstract class ArtifactRegistryProvider : IArtifactRegistryProvider
    {
        private readonly ImmutableDictionary<string, IArtifactRegistry> registriesByScheme;

        protected ArtifactRegistryProvider(IEnumerable<IArtifactRegistry> registries)
        {
            this.registriesByScheme = registries.ToImmutableDictionary(x => x.Scheme, x => x);
            this.SupportedSchemes = [.. this.registriesByScheme.Keys.OrderBy(x => x)];
        }

        public ImmutableArray<string> SupportedSchemes { get; }

        public IArtifactRegistry? TryGetRegistry(string scheme) => this.registriesByScheme.TryGetValue(scheme, out var registry) ? registry : null;
    }
}
