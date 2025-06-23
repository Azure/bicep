// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Frozen;
using Bicep.IO.Abstraction;

namespace Bicep.Core.SourceLink
{
    public static class SourceArchiveConstants
    {
        public const string MetadataFileName = "__metadata.json";
        public const string FilesFolderName = "files";

        public static readonly FrozenSet<char> ForbiddenPathCharacters = FilePathFacts.ForbiddenPathCharacters
            .Union(new char[] { '&', '+', '[', ']', '#' })
            .ToFrozenSet();

        public const int MaxLegalPathLength = 260; // Limit for Windows
        public const int MaxArchivePathLength = MaxLegalPathLength - 10; // ... this gives us some extra room to deduplicate paths

        // NOTE: Only change this value if there is a breaking change such that old versions of Bicep should fail on reading new source archives
        public const int CurrentMetadataVersion = 1;
        public static readonly string CurrentBicepVersion = ThisAssembly.AssemblyFileVersion;
    }
}
