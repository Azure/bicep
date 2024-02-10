// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Bicep.LangServer.IntegrationTests.Assertions;
using Bicep.LanguageServer.Registry;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace Bicep.LangServer.IntegrationTests;

[TestClass]
public class LangServerScenarioTests
{
    [NotNull]
    public TestContext? TestContext { get; set; }

    [TestMethod]
    public async Task Test_Issue1931()
    {
        // repro for https://github.com/Azure/bicep/issues/1931#issuecomment-1088061552
        var bicepFileUri = new Uri("file:///Users/ant/Desktop/asdfasdf/main.bicep");
        var textFileUri = new Uri("file:///Users/ant/Desktop/asdfasdf/a.txt");

        var helper = await MultiFileLanguageServerHelper.StartLanguageServer(TestContext);

        await helper.OpenFileOnceAsync(TestContext, "param foo string", bicepFileUri);

        // Deleting an unrelated .txt file in the same directory should have no impact on the .bicep file.
        helper.Client.Workspace.DidChangeWatchedFiles(new()
        {
            Changes = new Container<FileEvent>(
                new FileEvent
                {
                    Type = FileChangeType.Deleted,
                    Uri = textFileUri,
                }),
        });

        // Validate that the .bicep file compilation still works by requesting hovers and asserting we get a response.
        var hover = await helper.Client.RequestHover(new HoverParams
        {
            TextDocument = new(bicepFileUri),
            Position = new(0, 8),
        });

        hover!.Contents.MarkupContent!.Value.Should().Contain(@"```bicep
param foo: string
```");
    }

    [TestMethod]
    public async Task Test_Issue13254()
    {
        var clientFactory = RegistryHelper.CreateMockRegistryClient("mockregistry.io", "test/foo");
        async Task publish(string source) 
            => await RegistryHelper.PublishModuleToRegistry(
                clientFactory,
                "modulename",
                "br:mockregistry.io/test/foo:1.1",
                source,
                publishSource: false);

        var cacheRootPath = FileHelper.GetUniqueTestOutputPath(TestContext);
        var helper = await MultiFileLanguageServerHelper.StartLanguageServer(
            TestContext,
            services => services
                .WithFeatureOverrides(new(CacheRootDirectory: cacheRootPath))
                .WithContainerRegistryClientFactory(clientFactory)
                .AddSingleton<IModuleRestoreScheduler, ModuleRestoreScheduler>());

        await publish("param foo bool");

        var paramsFileUri = new Uri("file:///main.bicepparam");

        var diags = await helper.OpenFileOnceAsync(TestContext, """
using 'br:mockregistry.io/test/foo:1.1'

param foo = 'abc'
""", paramsFileUri);
        diags = await helper.WaitForDiagnostics(paramsFileUri);
        diags.Diagnostics.Should().ContainSingle(x => x.Message.Contains("Expected a value of type \"bool\" but the provided value is of type \"'abc'\"."));

        await publish("param foo string");

        await helper.Client.Workspace.ExecuteCommand(new Command
        {
            Name = "forceModulesRestore",
            Arguments = [
                paramsFileUri.LocalPath,
            ]
        });

        diags = await helper.WaitForDiagnostics(paramsFileUri);
        diags.Diagnostics.Should().BeEmpty();
    }
}
