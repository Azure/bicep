// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.RegistryModuleTool.Commands;
using Bicep.RegistryModuleTool.ModuleFiles;
using Bicep.RegistryModuleTool.TestFixtures.Assertions;
using Bicep.RegistryModuleTool.TestFixtures.Extensions;
using Bicep.RegistryModuleTool.TestFixtures.MockFactories;
using Bicep.RegistryModuleTool.TestFixtures.Mocks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.CommandLine;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Threading.Tasks;

namespace Bicep.RegistryModuleTool.IntegrationTests.Commands
{
    [TestClass]
    public class GenerateCommandTests
    {
        [DataTestMethod]
        [DynamicData(nameof(GetSuccessData), DynamicDataSourceType.Method)]
        public async Task InvokeAsync_OnSuccess_ReturnsZero(MockFileSystem fileSystemBeforeGeneration, MockFileSystem _)
        {
            var sut = CreateGenerateCommand(fileSystemBeforeGeneration);

            var exitCode = await sut.InvokeAsync("");

            exitCode.Should().Be(0);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetSuccessData), DynamicDataSourceType.Method)]
        public async Task InvokeAsync_OnSuccess_ProducesExpectedFiles(MockFileSystem fileSystemBeforeGeneration, MockFileSystem fileSystemAfterGeneration)
        {
            var sut = CreateGenerateCommand(fileSystemBeforeGeneration);

            await sut.InvokeAsync("");

            fileSystemBeforeGeneration.Should().HaveSameFilesAs(fileSystemAfterGeneration);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetSuccessData), DynamicDataSourceType.Method)]
        public async Task InvokeAsync_RepeatOnSuccess_ProducesSameFiles(MockFileSystem fileSystemBeforeGeneration, MockFileSystem fileSystemAfterGeneration)
        {
            var sut = CreateGenerateCommand(fileSystemBeforeGeneration);


            for (int i = 0; i < 2; i++)
            {
                await sut.InvokeAsync("");

                fileSystemBeforeGeneration.Should().HaveSameFilesAs(fileSystemAfterGeneration);
            }
        }

        [TestMethod]
        public async Task InvokeAsync_BicepBuildError_ReturnsOne()
        {
            var fileSystem = MockFileSystemFactory.CreateForSample(Sample.Valid);
            var sut = CreateGenerateCommand(fileSystem);

            fileSystem.File.WriteAllText(MainBicepFile.FileName, "something");

            var exitCode = await sut.InvokeAsync("");

            exitCode.Should().Be(1);
        }

        [TestMethod]
        public async Task InvokeAsync_BicepBuildError_PrintDiagnostics()
        {
            var fileSystem = MockFileSystemFactory.CreateForSample(Sample.Valid);
            var sut = CreateGenerateCommand(fileSystem);
            var mainBicepFilePath = fileSystem.Path.GetFullPath(MainBicepFile.FileName);
            var console = new MockConsole().ExpectErrorLines(
                @$"{mainBicepFilePath}(1,1) : Error BCP007: This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration.",
                @$"Failed to build ""{mainBicepFilePath}"".");

            fileSystem.File.WriteAllText(MainBicepFile.FileName, "something");

            await sut.InvokeAsync("", console);

            console.Verify();
        }

        private static IEnumerable<object[]> GetSuccessData()
        {
            //yield return CreateTestCase(Sample.Empty, Sample.NewlyGenerated);
            yield return CreateTestCase(Sample.Modified, Sample.Valid);
            //yield return CreateTestCase(Sample.Modified_Experimental, Sample.Valid_Experimental);

            static object[] CreateTestCase(Sample before, Sample after) => new object[]
            {
                MockFileSystemFactory.CreateForSample(before),
                MockFileSystemFactory.CreateForSample(after),
            };
        }

        private static GenerateCommand CreateGenerateCommand(IFileSystem fileSystem)
        {
            var serviceCollection = new ServiceCollection()
                .AddBicepCompilerWithFileSystem(fileSystem)
                .AddSingleton(MockLoggerFactory.CreateGenericLogger<GenerateCommand>())
                .AddSingleton<GenerateCommand.CommandHandler>();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var handler = serviceProvider.GetRequiredService<GenerateCommand.CommandHandler>();

            return new GenerateCommand()
            {
                Handler = handler,
            };
        }
    }
}
