// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Testing;

public static class TestFileSetExtensions
{
    public static T AddEmbeddedFiles<T>(this T fileSet, TestEmbeddedFile entryFile)
        where T : TestFileSet
    {
        fileSet.AddFiles(entryFile.GetDirectoryFiles()
            .Select(file => (file.GetPathRelativeToDirectory(entryFile.StreamDirectoryPath), (TestFileData)file.BinaryData))
            .ToArray());

        return fileSet;
    }
}
