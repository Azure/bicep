// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using System.Collections.Generic;
using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Bicep.Core.Modules
{
    /// <summary>
    /// Represents a reference to a local module (by relative path).
    /// </summary>
    public class LocalModuleReference : ModuleReference
    {
        private static readonly IEqualityComparer<string> PathComparer = StringComparer.Ordinal;

        private LocalModuleReference(string path)
            : base(ModuleReferenceSchemes.Local)
        {
            this.Path = path;
        }

        /// <summary>
        /// Gets the relative path to the module.
        /// </summary>
        public string Path { get; }

        public override bool Equals(object obj)
        {
            if(obj is not LocalModuleReference other)
            {
                return false;
            }

            return PathComparer.Equals(this.Path, other.Path);
        }

        public override int GetHashCode() => PathComparer.GetHashCode(this.Path);

        public override string UnqualifiedReference => this.Path;

        public override string FullyQualifiedReference => this.Path;

        public override bool IsExternal => false;

        public static LocalModuleReference? TryParse(string unqualifiedReference, out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
        {
            if(!Validate(unqualifiedReference, out failureBuilder))
            {
                return null;
            }

            return new(unqualifiedReference);
        }

        public static bool Validate(string pathName, [NotNullWhen(false)] out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
        {
            if (pathName.Length == 0)
            {
                failureBuilder = x => x.FilePathIsEmpty();
                return false;
            }

            if (pathName.First() == '/')
            {
                failureBuilder = x => x.FilePathBeginsWithForwardSlash();
                return false;
            }

            foreach (var pathChar in pathName)
            {
                if (pathChar == '\\')
                {
                    // enforce '/' rather than '\' for module paths for cross-platform compatibility
                    failureBuilder = x => x.FilePathContainsBackSlash();
                    return false;
                }

                if (forbiddenPathChars.Contains(pathChar))
                {
                    failureBuilder = x => x.FilePathContainsForbiddenCharacters(forbiddenPathChars);
                    return false;
                }

                if (IsInvalidPathControlCharacter(pathChar))
                {
                    failureBuilder = x => x.FilePathContainsControlChars();
                    return false;
                }
            }

            if (forbiddenPathTerminatorChars.Contains(pathName.Last()))
            {
                failureBuilder = x => x.FilePathHasForbiddenTerminator(forbiddenPathTerminatorChars);
                return false;
            }

            failureBuilder = null;
            return true;
        }

        private static readonly ImmutableHashSet<char> forbiddenPathChars = "<>:\"\\|?*".ToImmutableHashSet();
        private static readonly ImmutableHashSet<char> forbiddenPathTerminatorChars = " .".ToImmutableHashSet();

        private static bool IsInvalidPathControlCharacter(char pathChar)
        {
            // TODO: Revisit when we add unicode support to Bicep

            // The following are disallowed as path chars on Windows, so we block them to avoid cross-platform compilation issues.
            // Note that we're checking this range explicitly, as char.IsControl() includes some characters that are valid path characters.
            return pathChar >= 0 && pathChar <= 31;
        }
    }
}
