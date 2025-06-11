// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.SourceGraph;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Registry
{
    /// <summary>
    /// Strongly typed representation of a artifact reference string.
    /// </summary>
    public abstract class ArtifactReference
    {
        protected ArtifactReference(BicepSourceFile referencingFile, string scheme)
        {
            Scheme = scheme;
            ReferencingFile = referencingFile;
        }

        public string Scheme { get; }

        /// <summary>
        /// The URI of the template in which this artifact reference appears.
        /// </summary>
        public BicepSourceFile ReferencingFile { get; }

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
