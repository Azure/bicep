// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using Bicep.LangServer.IntegrationTests;
using Bicep.LanguageServer;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Deploy;
using Bicep.LanguageServer.Handlers;
using Bicep.LanguageServer.Providers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace Bicep.LangServer.UnitTests.Handlers
{
    [TestClass]
    public class BicepDeploymentStartCommandHandlerTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        private BicepDeploymentStartCommandHandler CreateHandler(ICompilationManager compilationManager)
        {
            var helper = ServiceBuilder.Create(services => services
                .AddSingleton(StrictMock.Of<ISerializer>().Object)
                .AddSingleton(StrictMock.Of<IDeploymentCollectionProvider>().Object)
                .AddSingleton(StrictMock.Of<IDeploymentOperationsCache>().Object)
                .AddSingleton(compilationManager)
                .AddSingleton(BicepTestConstants.CreateMockTelemetryProvider().Object)
                .AddSingleton(StrictMock.Of<IArmClientProvider>().Object)
                .WithFeatureOverrides(new(TestContext, ParamsFilesEnabled: true))
                .AddSingleton<BicepDeploymentStartCommandHandler>());

            return helper.Construct<BicepDeploymentStartCommandHandler>();
        }

        [TestMethod]
        public async Task DeploymentFileActionBuildOf_ValidBicepparametersfile_SucceedsWithResult()
        {
            var bicepFileContents = @"
param foo string
param bar int            
            ";

            var bicepparamFileContents = @"
using './main.bicep'

param foo = 'something'
param bar = 1
            ";

            var expectedParamJson = "{\r\n  \"$schema\": \"https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#\",\r\n  \"contentVersion\": \"1.0.0.0\",\r\n  \"parameters\": {\r\n    \"foo\": {\r\n      \"value\": \"something\"\r\n    },\r\n    \"bar\": {\r\n      \"value\": 1\r\n    }\r\n  }\r\n}";

            string bicepFilePath = FileHelper.SaveResultFile(TestContext, "main.bicep", bicepFileContents);
            var dir = Path.GetDirectoryName(bicepFilePath);
            string bicepparamFilePath = FileHelper.SaveResultFile(TestContext, "param.bicepparam", bicepparamFileContents, testOutputPath: dir);

            DocumentUri bicepparamUri = DocumentUri.FromFileSystemPath(bicepparamFilePath);

            BicepCompilationManager bicepCompilationManager = BicepCompilationManagerHelper.CreateCompilationManager(bicepparamUri, bicepFileContents, upsertCompilation: false);

            var bicepDeploymentStartCommandHandler = CreateHandler(bicepCompilationManager);

            var result = await bicepDeploymentStartCommandHandler.TryCompileBicepparamFile(bicepparamFilePath);

            result.isSuccess.Should().BeTrue();
            result.compilationResult.Should().Be(expectedParamJson);
        }


        [TestMethod]
        public async Task DeploymentFileActionBuildOf_InvalidBicepparametersfile_FailsWithError()
        {
            var bicepFileContents = @"
param foo string
param bar int            
            ";

            var bicepparamFileContents = @"
using './main.bicep'

param foo = 'something'
param bar = '1'
            ";

            var expectedError = @"Error BCP260: The parameter ""bar"" expects a value of type ""int"" but the provided value is of type ""'1'""";

            string bicepFilePath = FileHelper.SaveResultFile(TestContext, "main.bicep", bicepFileContents);
            var dir = Path.GetDirectoryName(bicepFilePath);
            string bicepparamFilePath = FileHelper.SaveResultFile(TestContext, "param.bicepparam", bicepparamFileContents, testOutputPath: dir);

            DocumentUri bicepparamUri = DocumentUri.FromFileSystemPath(bicepparamFilePath);

            BicepCompilationManager bicepCompilationManager = BicepCompilationManagerHelper.CreateCompilationManager(bicepparamUri, bicepFileContents, upsertCompilation: false);

            var bicepDeploymentStartCommandHandler = CreateHandler(bicepCompilationManager);

            var result = await bicepDeploymentStartCommandHandler.TryCompileBicepparamFile(bicepparamFilePath);

            result.isSuccess.Should().BeFalse();
            result.compilationResult.Should().Contain(expectedError);
        }
    }
}
