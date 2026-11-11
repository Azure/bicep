// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.


namespace Bicep.Core.Artifacts.References
{
    /// <summary>
    /// Strongly typed representation of a artifact reference string.
    /// </summary>
    public abstract class ArtifactReference
    {
        protected ArtifactReference(string scheme, ArtifactType type)
        {
            Scheme = scheme;
            Type = type;
        }

        public string Scheme { get; }

        /// <summary>
        /// Gets the type of the referenced artifact.
        /// </summary>
        public ArtifactType Type { get; }

        /// <summary>
        /// Gets the fully qualified artifact reference, which includes the scheme.
        /// </summary>
        public virtual string FullyQualifiedReference => $"{Scheme}:{UnqualifiedReference}";

        /// <summary>
        /// Gets the unqualified artifact reference, which does not include the scheme.
        /// </summary>
        public abstract string UnqualifiedReference { get; }

        public override string ToString() => FullyQualifiedReference;
    }
}
