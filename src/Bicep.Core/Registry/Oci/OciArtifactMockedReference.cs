// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.SourceGraph;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Registry.Oci
{
    /// <summary>
    /// Represents an OCI module reference that is mocked via a local filesystem path.
    /// When a module alias is added to moduleAliasesMock configuration,
    /// module references are resolved to local .bicep files instead of pulling from a container registry.
    /// </summary>
    public class OciArtifactMockedReference : ArtifactReference
    {
        private readonly IFileHandle fileHandle;
        private readonly string modulePath;
        private readonly string? fullyQualifiedReference;

        public OciArtifactMockedReference(BicepSourceFile referencingFile, string modulePath, IFileHandle fileHandle, string? fullyQualifiedReference = null)
            : base(referencingFile.LoadFeatures(), referencingFile.LoadConfiguration(), OciArtifactReferenceFacts.MockedScheme)
        {
            this.modulePath = modulePath;
            this.fileHandle = fileHandle;
            this.fullyQualifiedReference = fullyQualifiedReference;
        }

        // Override FullyQualifiedReference so user-facing diagnostics shows "br:..."
        public override string FullyQualifiedReference => fullyQualifiedReference ?? $"{OciArtifactReferenceFacts.Scheme}:{UnqualifiedReference}";

        public override string UnqualifiedReference => modulePath;

        public override bool IsExternal => false;

        public override ResultWithDiagnosticBuilder<IFileHandle> TryGetEntryPointFileHandle()
        {
            return new(fileHandle);
        }

        public override bool Equals(object? obj)
        {
            if (obj is not OciArtifactMockedReference other)
            {
                return false;
            }

            return StringComparer.Ordinal.Equals(modulePath, other.modulePath) &&
                   fileHandle.Equals(other.fileHandle);
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(modulePath, StringComparer.Ordinal);
            hash.Add(fileHandle);

            return hash.ToHashCode();
        }
    }
}
