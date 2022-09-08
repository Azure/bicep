// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.FileSystem;
using Bicep.Core.Modules;
using Bicep.Core.Registry;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests;
using Bicep.LanguageServer.Handlers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Collections.Generic;

namespace Bicep.LangServer.UnitTests
{
    [TestClass]
    public class BicepRegistryCacheRequestHandlerTests
    {
        private static readonly IFileSystem MockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
        {
            ["/foo/bar/bicepconfig.json"] = BicepTestConstants.BuiltInConfiguration.ToUtf8Json(),
        });

        private static readonly IConfigurationManager ConfigurationManager = new ConfigurationManager(MockFileSystem);

        [TestMethod]
        public async Task InvalidModuleReferenceShouldThrow()
        {
            const string ModuleRefStr = "hello";

            var dispatcher = StrictMock.Of<IModuleDispatcher>();
            DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder = null;
            ModuleReference? @ref = null;
            dispatcher.Setup(m => m.TryGetModuleReference(ModuleRefStr, It.IsAny<Uri>(), out @ref, out failureBuilder)).Returns(false);

            var resolver = StrictMock.Of<IFileResolver>();

            var handler = new BicepRegistryCacheRequestHandler(dispatcher.Object, resolver.Object);

            var @params = new BicepRegistryCacheParams("/main.bicep", ModuleRefStr);
            (await FluentActions
                .Awaiting(() => handler.Handle(@params, default))
                .Should()
                .ThrowAsync<InvalidOperationException>())
                .WithMessage($"The client specified an invalid module reference '{ModuleRefStr}'.");
        }

        [TestMethod]
        public async Task LocalModuleReferenceShouldThrow()
        {
            var dispatcher = StrictMock.Of<IModuleDispatcher>();
            DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder = null;

            const string ModuleRefStr = "./hello.bicep";
            LocalModuleReference.TryParse(ModuleRefStr, new Uri("fake:///not/real.bicep"), out var localRef, out _).Should().BeTrue();
            localRef.Should().NotBeNull();

            ModuleReference? outRef = localRef;
            dispatcher.Setup(m => m.TryGetModuleReference(ModuleRefStr, It.IsAny<Uri>(), out outRef, out failureBuilder)).Returns(true);

            var resolver = StrictMock.Of<IFileResolver>();

            var handler = new BicepRegistryCacheRequestHandler(dispatcher.Object, resolver.Object);

            var @params = new BicepRegistryCacheParams("/foo/bar/main.bicep", ModuleRefStr);
            (await FluentActions
                .Awaiting(() => handler.Handle(@params, default))
                .Should()
                .ThrowAsync<InvalidOperationException>())
                .WithMessage($"The specified module reference '{ModuleRefStr}' refers to a local module which is not supported by textDocument/bicepCache requests.");
        }

        [TestMethod]
        public async Task ExternalModuleNotInCacheShouldThrow()
        {
            var dispatcher = StrictMock.Of<IModuleDispatcher>();
            DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder = null;

            const string UnqualifiedModuleRefStr = "example.azurecr.invalid/foo/bar:v3";
            const string ModuleRefStr = "br:" + UnqualifiedModuleRefStr;

            var configuration = IConfigurationManager.GetBuiltInConfiguration();
            var parentModuleLocalPath = "/foo/main.bicep";
            var parentModuleUri = new Uri($"file://{parentModuleLocalPath}");
            OciArtifactModuleReference.TryParse(null, UnqualifiedModuleRefStr, configuration, parentModuleUri, out var moduleReference, out _).Should().BeTrue();
            moduleReference.Should().NotBeNull();

            ModuleReference? outRef = moduleReference;
            dispatcher.Setup(m => m.TryGetModuleReference(ModuleRefStr, parentModuleUri, out outRef, out failureBuilder)).Returns(true);
            dispatcher.Setup(m => m.GetModuleRestoreStatus(moduleReference!, out failureBuilder)).Returns(ModuleRestoreStatus.Unknown);

            var resolver = StrictMock.Of<IFileResolver>();

            var handler = new BicepRegistryCacheRequestHandler(dispatcher.Object, resolver.Object);

            var @params = new BicepRegistryCacheParams(parentModuleLocalPath, ModuleRefStr);
            (await FluentActions
                .Awaiting(() => handler.Handle(@params, default))
                .Should()
                .ThrowAsync<InvalidOperationException>())
                .WithMessage($"The module '{ModuleRefStr}' has not yet been successfully restored.");
        }

        [TestMethod]
        public async Task ExternalModuleFailedEntryPointShouldThrow()
        {
            var dispatcher = StrictMock.Of<IModuleDispatcher>();
            DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder = null;
            const string UnqualifiedModuleRefStr = "example.azurecr.invalid/foo/bar:v3";
            const string ModuleRefStr = "br:" + UnqualifiedModuleRefStr;

            var configuration = IConfigurationManager.GetBuiltInConfiguration();
            var parentModuleLocalPath = "/main.bicep";
            var parentModuleUri = new Uri($"file://{parentModuleLocalPath}");
            OciArtifactModuleReference.TryParse(null, UnqualifiedModuleRefStr, configuration, parentModuleUri, out var moduleReference, out _).Should().BeTrue();
            moduleReference.Should().NotBeNull();

            ModuleReference? outRef = moduleReference;
            dispatcher.Setup(m => m.TryGetModuleReference(ModuleRefStr, parentModuleUri, out outRef, out failureBuilder)).Returns(true);
            dispatcher.Setup(m => m.GetModuleRestoreStatus(moduleReference!, out failureBuilder)).Returns(ModuleRestoreStatus.Succeeded);
            Uri? @null = null;
            dispatcher.Setup(m => m.TryGetLocalModuleEntryPointUri(moduleReference!, out @null, out failureBuilder)).Returns(false);

            var resolver = StrictMock.Of<IFileResolver>();

            var handler = new BicepRegistryCacheRequestHandler(dispatcher.Object, resolver.Object);

            var @params = new BicepRegistryCacheParams(parentModuleLocalPath, ModuleRefStr);
            (await FluentActions
                .Awaiting(() => handler.Handle(@params, default))
                .Should()
                .ThrowAsync<InvalidOperationException>())
                .WithMessage($"Unable to obtain the entry point URI for module '{ModuleRefStr}'.");
        }

        [TestMethod]
        public async Task FailureToReadEntryPointShouldThrow()
        {
            var dispatcher = StrictMock.Of<IModuleDispatcher>();

            // needed for mocking out parameters
            DiagnosticBuilder.ErrorBuilderDelegate? nullBuilder = null;
            DiagnosticBuilder.ErrorBuilderDelegate? readFailureBuilder = x => x.ErrorOccurredReadingFile("Mock file read failure.");
            string? fileContents = null;

            const string UnqualifiedModuleRefStr = "example.azurecr.invalid/foo/bar:v3";
            const string ModuleRefStr = "br:" + UnqualifiedModuleRefStr;

            var fileUri = new Uri("file:///main.bicep");
            var configuration = IConfigurationManager.GetBuiltInConfiguration();
            OciArtifactModuleReference.TryParse(null, UnqualifiedModuleRefStr, configuration, fileUri, out var moduleReference, out _).Should().BeTrue();
            moduleReference.Should().NotBeNull();

            ModuleReference? outRef = moduleReference;
            dispatcher.Setup(m => m.TryGetModuleReference(ModuleRefStr, It.IsAny<Uri>(), out outRef, out nullBuilder)).Returns(true);
            dispatcher.Setup(m => m.GetModuleRestoreStatus(moduleReference!, out nullBuilder)).Returns(ModuleRestoreStatus.Succeeded);
            dispatcher.Setup(m => m.TryGetLocalModuleEntryPointUri(moduleReference!, out fileUri, out nullBuilder)).Returns(true);

            var resolver = StrictMock.Of<IFileResolver>();
            resolver.Setup(m => m.TryRead(fileUri, out fileContents, out readFailureBuilder)).Returns(false);

            var handler = new BicepRegistryCacheRequestHandler(dispatcher.Object, resolver.Object);

            var @params = new BicepRegistryCacheParams(fileUri.AbsolutePath, ModuleRefStr);
            (await FluentActions
                .Awaiting(() => handler.Handle(@params, default))
                .Should()
                .ThrowAsync<InvalidOperationException>())
                .WithMessage($"Unable to read file 'file:///main.bicep'. An error occurred reading file. Mock file read failure.");
        }

        [TestMethod]
        public async Task RestoredValidModuleShouldReturnSuccessfully()
        {
            var dispatcher = StrictMock.Of<IModuleDispatcher>();

            // needed for mocking out parameters
            DiagnosticBuilder.ErrorBuilderDelegate? nullBuilder = null;
            DiagnosticBuilder.ErrorBuilderDelegate? readFailureBuilder = x => x.ErrorOccurredReadingFile("Mock file read failure.");
            string? fileContents = "mock file contents";

            const string UnqualifiedModuleRefStr = "example.azurecr.invalid/foo/bar:v3";
            const string ModuleRefStr = "br:" + UnqualifiedModuleRefStr;

            var fileUri = new Uri("file:///foo/bar/main.bicep");
            var configuration = ConfigurationManager.GetConfiguration(fileUri);

            OciArtifactModuleReference.TryParse(null, UnqualifiedModuleRefStr, configuration, fileUri, out var moduleReference, out _).Should().BeTrue();
            moduleReference.Should().NotBeNull();

            ModuleReference? outRef = moduleReference;
            dispatcher.Setup(m => m.TryGetModuleReference(ModuleRefStr, It.IsAny<Uri>(), out outRef, out nullBuilder)).Returns(true);
            dispatcher.Setup(m => m.GetModuleRestoreStatus(moduleReference!, out nullBuilder)).Returns(ModuleRestoreStatus.Succeeded);
            dispatcher.Setup(m => m.TryGetLocalModuleEntryPointUri(moduleReference!, out fileUri, out nullBuilder)).Returns(true);

            var resolver = StrictMock.Of<IFileResolver>();
            resolver.Setup(m => m.TryRead(fileUri, out fileContents, out nullBuilder)).Returns(true);

            var handler = new BicepRegistryCacheRequestHandler(dispatcher.Object, resolver.Object);

            var @params = new BicepRegistryCacheParams(fileUri.AbsolutePath, ModuleRefStr);
            var response = await handler.Handle(@params, default);

            response.Should().NotBeNull();
            response.Content.Should().Be(fileContents);
        }
    }
}
