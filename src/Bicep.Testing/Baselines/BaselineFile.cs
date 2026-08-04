// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Testing.Baselines;

public record BaselineFile(TestContext TestContext, TestEmbeddedFile EmbeddedFile, string OutputFilePath)
{
    public string Read() => File.ReadAllText(OutputFilePath);

    public void Write(string contents) => File.WriteAllText(OutputFilePath, contents);
}