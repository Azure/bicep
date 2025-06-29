// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Samples;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Bicep.LangServer.IntegrationTests.Helpers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace Bicep.LangServer.IntegrationTests
{
    [TestClass]
    public class BuildCommandTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public async Task Build_command_should_generate_template()
        {
            var diagnosticsListener = new MultipleMessageListener<PublishDiagnosticsParams>();

            using var helper = await LanguageServerHelper.StartServer(
                this.TestContext,
                options => options.OnPublishDiagnostics(diagnosticsListener.AddMessage),
                services => services.WithNamespaceProvider(BuiltInTestTypes.Create()).WithFeatureOverrides(new(TestContext)));
            var client = helper.Client;

            var outputDirectory = FileHelper.SaveEmbeddedResourcesWithPathPrefix(
                TestContext,
                typeof(DataSet).Assembly,
                DataSets.Resources_CRLF.GetStreamPrefix());

            var bicepFilePath = Path.Combine(outputDirectory, "main.bicep");
            var expectedJson = File.ReadAllText(Path.Combine(outputDirectory, "main.json"));

            client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParamsFromFile(bicepFilePath, 1));
            await diagnosticsListener.WaitNext();

            await client.Workspace.ExecuteCommand(new Command
            {
                Name = "build",
                Arguments = new JArray {
                    DocumentUri.FromFileSystemPath(bicepFilePath).ToString(),
                }
            });

            var buildCommandOutput = File.ReadAllText(Path.ChangeExtension(bicepFilePath, ".json"));
            buildCommandOutput.Should().OnlyContainLFNewline();
            buildCommandOutput.Should().BeEquivalentToIgnoringNewlines(expectedJson);
        }

        [TestMethod]
        public async Task Build_command_should_generate_template_with_symbolic_names_if_enabled()
        {
            var diagnosticsListener = new MultipleMessageListener<PublishDiagnosticsParams>();

            using var helper = await LanguageServerHelper.StartServer(
                this.TestContext,
                options => options.OnPublishDiagnostics(diagnosticsListener.AddMessage),
                services => services.WithNamespaceProvider(BuiltInTestTypes.Create()).WithFeatureOverrides(new(TestContext, SymbolicNameCodegenEnabled: true)));
            var client = helper.Client;

            var outputDirectory = FileHelper.SaveEmbeddedResourcesWithPathPrefix(
                TestContext,
                typeof(DataSet).Assembly,
                DataSets.Resources_CRLF.GetStreamPrefix());

            var bicepFilePath = Path.Combine(outputDirectory, "main.bicep");
            var expectedJson = File.ReadAllText(Path.Combine(outputDirectory, "main.symbolicnames.json"));

            client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParamsFromFile(bicepFilePath, 1));
            await diagnosticsListener.WaitNext();

            await client.Workspace.ExecuteCommand(new Command
            {
                Name = "build",
                Arguments = new JArray {
                    DocumentUri.FromFileSystemPath(bicepFilePath).ToString(),
                }
            });

            var buildCommandOutput = File.ReadAllText(Path.ChangeExtension(bicepFilePath, ".json"));
            buildCommandOutput.Should().OnlyContainLFNewline();
            buildCommandOutput.Should().BeEquivalentToIgnoringNewlines(expectedJson);
        }

        // https://github.com/Azure/bicep/issues/14196
        [TestMethod]
        public async Task Build_command_with_transitive_imports_should_succeed_even_if_multiple_compilations_are_combined_unexpectedly()
        {
            var files = new Dictionary<string, string>
            {
                {
                    "main.bicep",
                    """
                    import * as typesB from 'moduleB.bicep'
                    import * as typesC from 'moduleC.bicep'
                    """
                },
                {
                    "moduleA.bicep",
                    """
                    @export()
                    type typeA = {
                        propA: string
                    }
                    """
                },
                {
                    "moduleB.bicep",
                    """
                    import * as typesA from 'moduleA.bicep'
                    @export()
                    type typeB = {
                        optionsA: typesA.typeA
                        propB: string
                    }
                    """
                },
                {
                    "moduleC.bicep",
                    """
                    import * as typesA from 'moduleA.bicep'
                    @export()
                    type typeC = {
                        optionsA: typesA.typeA
                        propC: string
                    }
                    """
                },
            };

            var outputDirectory = FileHelper.GetUniqueTestOutputPath(TestContext);
            Directory.CreateDirectory(outputDirectory);

            foreach (var kvp in files)
            {
                using var file = File.CreateText(Path.Combine(outputDirectory, kvp.Key));
                await file.WriteAsync(kvp.Value);
            }

            var diagnosticsListener = new MultipleMessageListener<PublishDiagnosticsParams>();

            using var helper = await LanguageServerHelper.StartServer(
                this.TestContext,
                options => options.OnPublishDiagnostics(diagnosticsListener.AddMessage),
                services => services
                    .WithNamespaceProvider(BuiltInTestTypes.Create())
                    .WithFeatureOverrides(new(TestContext)));
            var client = helper.Client;

            var mainPath = Path.Combine(outputDirectory, "main.bicep");
            var moduleCPath = Path.Combine(outputDirectory, "moduleC.bicep");

            // First, load main.bicep (and compile the transitive closure)
            client.TextDocument.DidOpenTextDocument(
                TextDocumentParamHelper.CreateDidOpenDocumentParamsFromFile(mainPath, 1));
            await diagnosticsListener.WaitNext();

            // Second, load moduleC.bicep (and compile its transitive closure)
            // Because main.bicep and moduleC.bicep exist in separate "active contexts" in the language server, this
            // sequence of actions will result in the compilation manager having two separate SemanticModels for
            // moduleA.bicep: one in the transitive closure of main.bicep (still reachable via the import of
            // moduleB.bicep) and another in the transitive closure of moduleC.bicep.
            client.TextDocument.DidOpenTextDocument(
                TextDocumentParamHelper.CreateDidOpenDocumentParamsFromFile(moduleCPath, 1));
            await diagnosticsListener.WaitNext();

            // Make sure we can still build main.bicep event though the `typeA` symbol from moduleA.bicep has two
            // instantiations (and two different memory addresses) within the compilation manager.
            await client.Workspace.ExecuteCommand(new Command
            {
                Name = "build",
                Arguments = [DocumentUri.FromFileSystemPath(mainPath).ToString()],
            });

            var buildCommandOutput = File.ReadAllText(Path.ChangeExtension(mainPath, ".json"));
            buildCommandOutput.Length.Should().BeGreaterThan(0);
        }
    }
}
