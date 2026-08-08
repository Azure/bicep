// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Bicep.Core.FileSystem;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Mock;
using Bicep.LanguageServer.Extensions;
using Bicep.LanguageServer.Handlers;
using Bicep.Testing.IO;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace Bicep.LangServer.UnitTests.Handlers
{
    [TestClass]
    public class BicepDecompileParamsCommandHandlerTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        private static BicepDecompileParamsCommandHandler CreateHandler(TestFileSet files)
        {
            var helper = ServiceBuilder.Create(services => services
                .WithFileExplorer(files.FileExplorer)
                .AddSingleton(StrictMock.Of<ISerializer>().Object)
                .AddSingleton<BicepDecompileParamsCommandHandler>());

            return helper.Construct<BicepDecompileParamsCommandHandler>();
        }

        [TestMethod]
        public async Task HandleDecompileParams_WithValidParamsFile_ShouldSucceed()
        {
            var paramFile =
                """
                {
                "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#",
                "contentVersion": "1.0.0.0",
                "parameters": {
                  "foo": {
                    "value": "bar"
                  }
                }
                }
                """;
            var expectedOutput =
                """
                using 'main.bicep'

                param foo = 'bar'

                """;

            var files = new InMemoryTestFileSet().AddFile("param.json", paramFile);
            var paramFileUri = files.GetUri("param.json");
            var bicepFileUri = files.GetUri("main.bicep");

            var requestParams = new BicepDecompileParamsCommandParams(paramFileUri.ToDocumentUri(), bicepFileUri.ToDocumentUri());

            var decompileParamsCommandHandler = CreateHandler(files);

            var result = await decompileParamsCommandHandler.Handle(
                requestParams,
                CancellationToken.None);

            result.decompiledBicepparamFile.Should().NotBeNull();
            result.errorMessage.Should().BeNull();
            result.decompiledBicepparamFile?.contents.Should().Be(expectedOutput);
        }

        [TestMethod]
        public async Task HandleDecompileParams_WithInValidParamsFile_ShouldFailWithError()
        {
            var paramFile =
  @"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""parameters"": {
    ""foo"": {
    }
  }
}";
            var expectedErrorMsg = "Decompilation failed. Please fix the following problems and try again: [5:10]: No value found parameter foo";

            var files = new InMemoryTestFileSet().AddFile("param.json", paramFile);

            var requestParams = new BicepDecompileParamsCommandParams(files.GetUri("param.json").ToDocumentUri(), "/main.bicep");

            var decompileParamsCommandHandler = CreateHandler(files);

            var result = await decompileParamsCommandHandler.Handle(
                requestParams,
                CancellationToken.None);

            result.decompiledBicepparamFile.Should().BeNull();
            result.errorMessage.Should().Be(expectedErrorMsg);
        }
    }
}
