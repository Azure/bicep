// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Bicep.Core.Diagnostics;
using Bicep.IO.Abstraction;

namespace Bicep.Core.SourceGraph
{
    public readonly partial struct RelativePath
    {
        private readonly string value;

        public RelativePath(string value) => this.value = value;

        public static implicit operator string(RelativePath relativePath) => relativePath.value;

        public ReadOnlySpan<char> AsSpan() => this.value.AsSpan();

        public override string ToString() => this.value;

        [GeneratedRegex(@"^[a-zA-Z]:")]
        private static partial Regex WindowsDriveLetterColonRegex();

        public static ResultWithDiagnosticBuilder<RelativePath> TryCreate(string path)
        {
            if (path.Length == 0)
            {
                return new(x => x.FilePathIsEmpty());
            }

            if (path.First() == '/' || WindowsDriveLetterColonRegex().IsMatch(path))
            {
                // '/' for Unix-style absolute paths, 'C:', etc. for Windows-style absolute paths
                return new(x => x.FilePathIsAbsolute());
            }

            foreach (var pathChar in path)
            {
                if (pathChar == '\\')
                {
                    // enforce '/' rather than '\' for module paths for cross-platform compatibility
                    return new(x => x.FilePathContainsBackSlash());
                }

                if (FilePathFacts.IsForbiddenPathVisibleCharacter(pathChar))
                {
                    return new(x => x.FilePathContainsForbiddenCharacters(FilePathFacts.ForbiddenPathCharacters));
                }

                if (FilePathFacts.IsForbiddenPathControlCharacter(pathChar))
                {
                    return new(x => x.FilePathContainsControlChars());
                }
            }

            if (FilePathFacts.IsForbiddenPathTerminatorCharacter(path.Last()))
            {
                return new(x => x.FilePathHasForbiddenTerminator(FilePathFacts.ForbiddenPathTerminatorCharacters));
            }

            return new(new RelativePath(path));
        }
    }
}
