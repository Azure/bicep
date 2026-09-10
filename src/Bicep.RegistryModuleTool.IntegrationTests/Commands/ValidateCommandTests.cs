// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.CommandLine;
using System.IO.Abstractions;
using Bicep.Core.Parsing;
using Bicep.RegistryModuleTool.Commands;
using Bicep.RegistryModuleTool.Extensions;
using Bicep.RegistryModuleTool.ModuleFiles;
using Bicep.RegistryModuleTool.TestFixtures.Extensions;
using Bicep.RegistryModuleTool.TestFixtures.MockFactories;
using Bicep.RegistryModuleTool.TestFixtures.Mocks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.RegistryModuleTool.IntegrationTests.Commands
{
    [TestClass]
    public class ValidateCommandTests
    {
        [TestMethod]
        public async Task InvokeAsync_ValidFiles_ReturnsZero()
        {
            var fileSystem = MockFileSystemFactory.CreateForSample(Sample.Valid);
            var sut = CreateValidateCommand(fileSystem);

            var exitCode = await sut.Parse("").InvokeAsync();

            exitCode.Should().Be(0);
        }

        [TestMethod]
        public async Task InvokeAsync_InvalidFiles_ReturnsOne()
        {
            var fileSystem = MockFileSystemFactory.CreateForSample(Sample.Invalid);
            var sut = CreateValidateCommand(fileSystem);

            var exitCode = await sut.Parse("").InvokeAsync();

            exitCode.Should().Be(1);
        }

        [TestMethod]
        public async Task InvokeAsync_InvalidFiles_WritesErrorsToConsole()
        {
            var fileSystem = MockFileSystemFactory.CreateForSample(Sample.Invalid);
            var testFile = MainBicepTestFile.Open(fileSystem);
            var console = new MockConsole().ExpectErrorLines(StringUtils.SplitOnNewLine(
                $"""
                The file "{fileSystem.Path.GetFullPath(MainBicepFile.FileName)}" is invalid:
                  - A description must be specified for parameter "dnsPrefix".
                  - A description must be specified for parameter "servicePrincipalClientSecret".
                  - A description must be specified for output "controlPlaneFQDN".
                  - Metadata "description" must contain at least 10 characters.

                The file "{testFile.Path}" is invalid:
                  - Could not find tests in the file. Please make sure to add at least one module referencing the main Bicep file.

                The file "{fileSystem.Path.GetFullPath(MainArmTemplateFile.FileName)}" is invalid:
                  - The file is modified or outdated. Please run "brm generate" to regenerate it.

                The file "{fileSystem.Path.GetFullPath(ReadmeFile.FileName)}" is invalid:
                  - The file is modified or outdated. Please run "brm generate" to regenerate it.

                The file "{fileSystem.Path.GetFullPath(VersionFile.FileName)}" is invalid:
                  - #: Required properties ["$schema","version"] are not present.
                  - The file is modified or outdated. Please run "brm generate" to regenerate it.

                """));

            var sut = CreateValidateCommand(fileSystem, console);

            await sut.Parse("").InvokeAsync();

            console.Verify();
        }

        [TestMethod]
        public async Task InvokeAsync_BicepBuildError_ReturnsOne()
        {
            var fileSystem = MockFileSystemFactory.CreateForSample(Sample.Valid);
            fileSystem.File.WriteAllText(MainBicepFile.FileName, "something");
            var sut = CreateValidateCommand(fileSystem);

            var exitCode = await sut.Parse("").InvokeAsync();

            exitCode.Should().Be(1);
        }

        [TestMethod]
        public async Task InvokeAsync_BicepBuildError_PrintDiagnostics()
        {
            var fileSystem = MockFileSystemFactory.CreateForSample(Sample.Valid);
            var mainBicepFilePath = fileSystem.Path.GetFullPath(MainBicepFile.FileName);
            var console = new MockConsole().ExpectErrorLines(
                @$"{mainBicepFilePath}(1,1) : Error BCP007: This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. [https://aka.ms/bicep/core-diagnostics#BCP007]",
                @$"Failed to build ""{mainBicepFilePath}"".");
            fileSystem.File.WriteAllText(MainBicepFile.FileName, "something");
            var sut = CreateValidateCommand(fileSystem, console);

            await sut.Parse("").InvokeAsync();

            console.Verify();
        }

        [TestMethod]
        public async Task InvokeAsync_BicepTestBuildError_ReturnsOne()
        {
            var fileSystem = MockFileSystemFactory.CreateForSample(Sample.Valid);
            var bicepTestFile = MainBicepTestFile.Open(fileSystem);
            fileSystem.File.WriteAllText(bicepTestFile.Path, "something");
            var sut = CreateValidateCommand(fileSystem);

            var exitCode = await sut.Parse("").InvokeAsync();

            exitCode.Should().Be(1);
        }

        [TestMethod]
        public async Task InvokeAsync_BicepTestBuildError_PrintDiagnostics()
        {
            var fileSystem = MockFileSystemFactory.CreateForSample(Sample.Valid);
            var bicepTestFile = MainBicepTestFile.Open(fileSystem);
            fileSystem.File.WriteAllText(bicepTestFile.Path, "something");
            var console = new MockConsole().ExpectErrorLines(
                @$"{bicepTestFile.Path}(1,1) : Error BCP007: This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. [https://aka.ms/bicep/core-diagnostics#BCP007]",
                @$"Failed to build ""{bicepTestFile.Path}"".");
            var sut = CreateValidateCommand(fileSystem, console);

            await sut.Parse("").InvokeAsync();

            console.Verify();
        }

        private static ValidateCommand CreateValidateCommand(IFileSystem fileSystem, IConsole? console = null)
        {
            var serviceCollection = new ServiceCollection()
                .AddBicepCompilerWithFileSystem(fileSystem)
                .AddSingleton(MockLoggerFactory.CreateGenericLogger<ValidateCommand>())
                .AddSingleton<ValidateCommand.CommandHandler>();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var handler = serviceProvider.GetRequiredService<ValidateCommand.CommandHandler>();
            var effectiveConsole = console ?? new MockConsole();

            var command = new ValidateCommand();
            command.SetAction(async (ParseResult _, CancellationToken ct) =>
                await handler.InvokeAsync(effectiveConsole, ct));

            return command;
        }
    }
}
