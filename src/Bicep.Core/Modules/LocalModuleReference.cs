// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Registry;
using Bicep.Core.Utils;

namespace Bicep.Core.Modules
{
    /// <summary>
    /// Represents a reference to a local module (by relative path).
    /// </summary>
    public class LocalModuleReference : ArtifactReference
    {
        private static readonly IEqualityComparer<string> PathComparer = StringComparer.Ordinal;

        private LocalModuleReference(ArtifactType artifactType, string path, Uri parentModuleUri)
            : base(ArtifactReferenceSchemes.Local, parentModuleUri)
        {
            ArtifactType = artifactType;
            this.Path = path;
        }

        public ArtifactType ArtifactType { get; }

        /// <summary>
        /// Gets the relative path to the module.
        /// </summary>
        public string Path { get; }

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

        public static ResultWithDiagnostic<LocalModuleReference> TryParse(ArtifactType artifactType, string unqualifiedReference, Uri parentModuleUri)
        {
            return Validate(unqualifiedReference)
                .Transform(_ => new LocalModuleReference(artifactType, unqualifiedReference, parentModuleUri));
        }

        public static ResultWithDiagnostic<bool> Validate(string pathName)
        {
            if (pathName.Length == 0)
            {
                return new(x => x.FilePathIsEmpty());
            }

            if (pathName.First() == '/')
            {
                return new(x => x.FilePathBeginsWithForwardSlash());
            }

            foreach (var pathChar in pathName)
            {
                if (pathChar == '\\')
                {
                    // enforce '/' rather than '\' for module paths for cross-platform compatibility
                    return new(x => x.FilePathContainsBackSlash());
                }

                if (forbiddenPathChars.Contains(pathChar))
                {
                    return new(x => x.FilePathContainsForbiddenCharacters(forbiddenPathChars));
                }

                if (IsInvalidPathControlCharacter(pathChar))
                {
                    return new(x => x.FilePathContainsControlChars());
                }
            }

            if (forbiddenPathTerminatorChars.Contains(pathName.Last()))
            {
                return new(x => x.FilePathHasForbiddenTerminator(forbiddenPathTerminatorChars));
            }

            return new(true);
        }

        private static readonly ImmutableHashSet<char> forbiddenPathChars = [.. "<>:\"\\|?*"];
        private static readonly ImmutableHashSet<char> forbiddenPathTerminatorChars = [.. " ."];

        private static bool IsInvalidPathControlCharacter(char pathChar)
        {
            // TODO: Revisit when we add unicode support to Bicep

            // The following are disallowed as path chars on Windows, so we block them to avoid cross-platform compilation issues.
            // Note that we're checking this range explicitly, as char.IsControl() includes some characters that are valid path characters.
            return pathChar >= 0 && pathChar <= 31;
        }
    }
}
