// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.


namespace Bicep.Core.Artifacts.References
{
    /// <summary>
    /// Strongly typed representation of a artifact reference string.
    /// </summary>
    public abstract class ArtifactReference
    {
        protected ArtifactReference(ArtifactType type)
        {
            Type = type;
        }

        /// <summary>
        /// Gets the type of the referenced artifact.
        /// </summary>
        public ArtifactType Type { get; }

        /// <summary>
        /// Gets the fully qualified artifact reference, which includes the scheme.
        /// </summary>
        public abstract string FullyQualifiedReference { get; }

        /// <summary>
        /// Gets the unqualified artifact reference, which does not include the scheme.
        /// </summary>
        public abstract string UnqualifiedReference { get; }

        public override string ToString() => FullyQualifiedReference;
    }
}
