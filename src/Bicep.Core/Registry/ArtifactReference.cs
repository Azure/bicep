// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Features;
using Bicep.Core.SourceGraph;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Registry
{
    /// <summary>
    /// Strongly typed representation of a artifact reference string.
    /// </summary>
    public abstract class ArtifactReference
    {
        protected ArtifactReference(IFeatureProvider featureProvider, RootConfiguration configuration, string scheme)
        {
            Scheme = scheme;
            FeatureProvider = featureProvider;
            Configuration = configuration;
        }

        public string Scheme { get; }

        public IFeatureProvider FeatureProvider { get; }

        public RootConfiguration Configuration { get; }

        /// <summary>
        /// Gets the fully qualified artifact reference, which includes the scheme.
        /// </summary>
        public virtual string FullyQualifiedReference => $"{Scheme}:{UnqualifiedReference}";

        /// <summary>
        /// Gets the unqualified artifact reference, which does not include the scheme.
        /// </summary>
        public abstract string UnqualifiedReference { get; }

        /// <summary>
        /// Gets a value indicating whether this reference points to an external artifact.
        /// </summary>
        public abstract bool IsExternal { get; }

        public abstract ResultWithDiagnosticBuilder<IFileHandle> TryGetEntryPointFileHandle();

        public override string ToString() => FullyQualifiedReference;
    }
}
