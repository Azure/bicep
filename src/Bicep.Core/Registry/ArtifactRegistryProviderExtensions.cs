// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.Core.Registry
{
    public static class ArtifactRegistryProviderExtensions
    {
        public static IArtifactRegistry GetRegistry(this IArtifactRegistryProvider provider, string scheme) =>
            provider.TryGetRegistry(scheme) ?? throw new ArgumentException($"No artifact registry is configured for the scheme '{scheme}'.");
    }
}
