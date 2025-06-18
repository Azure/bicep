// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Registry;
using Bicep.Core.SourceGraph;
using Bicep.Core.SourceGraph.ArtifactReferences;
using Bicep.Core.SourceGraph.Artifacts;
using Bicep.Core.Utils;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Modules
{
    /// <summary>
    /// Represents a reference to a local module (by relative path).
    /// </summary>
    public class LocalModuleReference : ArtifactReference, IExtensionArtifactReference
    {
        // TODO(file-io-abstraction): Create a dedicated type for local extension reference.

        private static readonly StringComparer PathComparer = StringComparer.Ordinal;

        private readonly Lazy<ResultWithDiagnosticBuilder<IFileHandle>> lazyTargetFileResult;
        private readonly Lazy<ResultWithDiagnosticBuilder<BinaryData>> lazyExtensionBinaryDataResult;
        private readonly Lazy<LocalExtensionArtifact> lazyLocalExtensionArtifact;

        private LocalModuleReference(BicepSourceFile referencingFile, ArtifactType artifactType, RelativePath path)
            : base(referencingFile, ArtifactReferenceSchemes.Local)
        {
            ArtifactType = artifactType;
            this.Path = path;
            this.lazyTargetFileResult = new(() => this.ReferencingFile.FileHandle.TryGetRelativeFile(this.Path));
            this.lazyExtensionBinaryDataResult = new(() => this.lazyTargetFileResult.Value.Transform(x => x.TryReadBinaryData()));
            this.lazyLocalExtensionArtifact = new(() => new(this.lazyExtensionBinaryDataResult.Value.Unwrap(), referencingFile.Features.CacheRootDirectory));
        }

        public ArtifactType ArtifactType { get; }

        /// <summary>
        /// Gets the relative path to the module.
        /// </summary>
        public RelativePath Path { get; }

        public override bool Equals(object? obj)
        {
            if (obj is not LocalModuleReference other)
            {
                return false;
            }

            return PathComparer.Equals(this.Path, other.Path);
        }

        public override int GetHashCode() => PathComparer.GetHashCode(this.Path);

        public override string UnqualifiedReference => this.Path;

        public override string FullyQualifiedReference => this.Path;

        public override bool IsExternal => false;

        public static ResultWithDiagnosticBuilder<LocalModuleReference> TryParse(BicepSourceFile referencingFile, ArtifactType artifactType, string unqualifiedReference) =>
            RelativePath.TryCreate(unqualifiedReference).Transform(relativePath => new LocalModuleReference(referencingFile, artifactType, relativePath));

        public override ResultWithDiagnosticBuilder<IFileHandle> TryGetEntryPointFileHandle()
        {
            if (this.ArtifactType == ArtifactType.Module)
            {
                return this.lazyTargetFileResult.Value;
            }

            // Handle local extension reference.
            if (!this.lazyExtensionBinaryDataResult.Value.IsSuccess(out _, out var error))
            {
                return new(error);
            }

            // For local extension, the "entry point" is the types.tgz file in the Bicep cache folder.
            // TODO(shenglol): This is counterintuitive. Will refactor the whole "artifact" concept in the future.
            return new(this.lazyLocalExtensionArtifact.Value.TypesTgzFile);
        }

        public IExtensionArtifact ResolveExtensionArtifact() => this.ArtifactType == ArtifactType.Extension
            ? this.lazyLocalExtensionArtifact.Value
            : throw new InvalidOperationException("Cannot resolve extension artifact for local module reference.");
    }
}
