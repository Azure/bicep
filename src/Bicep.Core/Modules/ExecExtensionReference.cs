// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Registry;
using Bicep.Core.SourceGraph;
using Bicep.Core.SourceGraph.ArtifactReferences;
using Bicep.Core.SourceGraph.Artifacts;
using Bicep.Core.Utils;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Modules
{
    /// <summary>
    /// Represents an extension reference that points to a local binary executable,
    /// either via an absolute file system path or by name (resolved from the system PATH).
    /// Syntax: extension 'exec:/path/to/binary' or extension 'exec:my-binary'
    /// </summary>
    public class ExecExtensionReference : ArtifactReference, IExtensionArtifactReference
    {
        private readonly IFileExplorer fileExplorer;
        private readonly Lazy<ExecExtensionArtifact> lazyArtifact;

        private ExecExtensionReference(BicepSourceFile referencingFile, string rawValue, IFileExplorer fileExplorer)
            : base(referencingFile, ArtifactReferenceSchemes.Exec)
        {
            RawValue = rawValue;
            this.fileExplorer = fileExplorer;
            this.lazyArtifact = new(() => new ExecExtensionArtifact(rawValue, referencingFile.Features.CacheRootDirectory, fileExplorer));
        }

        /// <summary>
        /// The raw value after the 'exec:' scheme prefix (e.g. '/path/to/binary' or 'my-binary').
        /// </summary>
        public string RawValue { get; }

        public override bool Equals(object? obj) =>
            obj is ExecExtensionReference other &&
            StringComparer.Ordinal.Equals(RawValue, other.RawValue);

        public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(RawValue);

        public override string UnqualifiedReference => RawValue;

        public override string FullyQualifiedReference => $"{ArtifactReferenceSchemes.Exec}:{RawValue}";

        public override bool IsExternal => false;

        public static ResultWithDiagnosticBuilder<ExecExtensionReference> TryParse(
            BicepSourceFile referencingFile,
            string rawValue,
            IFileExplorer fileExplorer)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
            {
                return new(x => x.ExpectedExtensionSpecification());
            }

            return new(new ExecExtensionReference(referencingFile, rawValue, fileExplorer));
        }

        public override ResultWithDiagnosticBuilder<IFileHandle> TryGetEntryPointFileHandle()
        {
            // The entry point is the cached types.tgz file.
            var typesTgzFile = this.lazyArtifact.Value.TypesTgzFile;

            if (!typesTgzFile.Exists())
            {
                return new(x => x.ArtifactRequiresRestore(FullyQualifiedReference));
            }

            return new(typesTgzFile);
        }

        public IExtensionArtifact ResolveExtensionArtifact() => this.lazyArtifact.Value;
    }
}
