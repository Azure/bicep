// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.IO.Abstraction;
using Bicep.IO.InMemory;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests;

[TestClass]
public class BicepCompilerTests : TestBase
{
    [TestMethod]
    public async Task BicepCompiler_can_be_constructed()
    {
        var fileExplorer = new InMemoryFileExplorer();
        var bicepUri = IOUri.FromFilePath("/main.bicep");
        fileExplorer.GetFile(bicepUri).WriteAllText("""
        param foo string
        output foo string = foo
        """);

        var compiler = BicepCompiler.Create(s => s.AddSingleton<IFileExplorer>(fileExplorer));
        var result = await compiler.CompileBicepFile(bicepUri);

        result.Template.Should().Contain("[parameters('foo')]");
    }
}
