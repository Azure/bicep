// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bicep.Testing.IO;

namespace Bicep.Testing.Baselines;

public record BaselineFile(TestContext TestContext, EmbeddedFile EmbeddedFile, string OutputFilePath)
{
    public string Read() => File.ReadAllText(OutputFilePath);

    public void Write(string contents) => File.WriteAllText(OutputFilePath, contents);
}