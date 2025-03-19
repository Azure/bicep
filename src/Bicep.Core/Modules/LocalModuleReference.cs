// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Registry;
using Bicep.Core.SourceGraph;
using Bicep.Core.Utils;
using Bicep.Core.Workspaces;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Modules
{
    /// <summary>
    /// Represents a reference to a local module (by relative path).
    /// </summary>
    public class LocalModuleReference : ArtifactReference
    {
        private static readonly IEqualityComparer<string> PathComparer = StringComparer.Ordinal;

        private LocalModuleReference(BicepSourceFile referencingFile, ArtifactType artifactType, RelativePath path)
            : base(referencingFile, ArtifactReferenceSchemes.Local)
        {
            ArtifactType = artifactType;
            this.Path = path;
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
    }
}
