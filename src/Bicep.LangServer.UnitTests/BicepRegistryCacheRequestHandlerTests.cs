// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.FileSystem;
using Bicep.Core.Modules;
using Bicep.Core.Registry;
using Bicep.Core.UnitTests.Mock;
using Bicep.LanguageServer.Handlers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bicep.LangServer.UnitTests
{
    [TestClass]
    public class BicepRegistryCacheRequestHandlerTests
    {
        [TestMethod]
        public async Task InvalidModuleReferenceShouldThrow()
        {
            const string ModuleRefStr = "hello";

            var dispatcher = StrictMock.Of<IModuleDispatcher>();
            DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder = null;
            dispatcher.Setup(m => m.TryGetModuleReference(ModuleRefStr, out failureBuilder)).Returns<ModuleReference>(null);

            var resolver = StrictMock.Of<IFileResolver>();

            var handler = new BicepRegistryCacheRequestHandler(dispatcher.Object, resolver.Object);
            
            var @params = new BicepRegistryCacheParams(ModuleRefStr);
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
            var localRef = LocalModuleReference.TryParse(ModuleRefStr, out _);
            localRef.Should().NotBeNull();

            dispatcher.Setup(m => m.TryGetModuleReference(ModuleRefStr, out failureBuilder)).Returns(localRef);

            var resolver = StrictMock.Of<IFileResolver>();

            var handler = new BicepRegistryCacheRequestHandler(dispatcher.Object, resolver.Object);

            var @params = new BicepRegistryCacheParams(ModuleRefStr);
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
            
            var moduleReference = OciArtifactModuleReference.TryParse(UnqualifiedModuleRefStr, out _)!;
            moduleReference.Should().NotBeNull();

            dispatcher.Setup(m => m.TryGetModuleReference(ModuleRefStr, out failureBuilder)).Returns(moduleReference);
            dispatcher.Setup(m => m.GetModuleRestoreStatus(moduleReference, out failureBuilder)).Returns(ModuleRestoreStatus.Unknown);

            var resolver = StrictMock.Of<IFileResolver>();

            var handler = new BicepRegistryCacheRequestHandler(dispatcher.Object, resolver.Object);

            var @params = new BicepRegistryCacheParams(ModuleRefStr);
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

            var moduleReference = OciArtifactModuleReference.TryParse(UnqualifiedModuleRefStr, out _)!;
            moduleReference.Should().NotBeNull();

            dispatcher.Setup(m => m.TryGetModuleReference(ModuleRefStr, out failureBuilder)).Returns(moduleReference);
            dispatcher.Setup(m => m.GetModuleRestoreStatus(moduleReference, out failureBuilder)).Returns(ModuleRestoreStatus.Succeeded);
            dispatcher.Setup(m => m.TryGetLocalModuleEntryPointUri(null, moduleReference, out failureBuilder)).Returns<Uri?>(null);

            var resolver = StrictMock.Of<IFileResolver>();

            var handler = new BicepRegistryCacheRequestHandler(dispatcher.Object, resolver.Object);

            var @params = new BicepRegistryCacheParams(ModuleRefStr);
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

            var moduleReference = OciArtifactModuleReference.TryParse(UnqualifiedModuleRefStr, out _)!;
            moduleReference.Should().NotBeNull();

            var fileUri = new Uri("file:///main.bicep");

            dispatcher.Setup(m => m.TryGetModuleReference(ModuleRefStr, out nullBuilder)).Returns(moduleReference);
            dispatcher.Setup(m => m.GetModuleRestoreStatus(moduleReference, out nullBuilder)).Returns(ModuleRestoreStatus.Succeeded);
            dispatcher.Setup(m => m.TryGetLocalModuleEntryPointUri(null, moduleReference, out nullBuilder)).Returns(fileUri);

            var resolver = StrictMock.Of<IFileResolver>();
            resolver.Setup(m => m.TryRead(fileUri, out fileContents, out readFailureBuilder)).Returns(false);

            var handler = new BicepRegistryCacheRequestHandler(dispatcher.Object, resolver.Object);

            var @params = new BicepRegistryCacheParams(ModuleRefStr);
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

            var moduleReference = OciArtifactModuleReference.TryParse(UnqualifiedModuleRefStr, out _)!;
            moduleReference.Should().NotBeNull();

            var fileUri = new Uri("file:///main.bicep");

            dispatcher.Setup(m => m.TryGetModuleReference(ModuleRefStr, out nullBuilder)).Returns(moduleReference);
            dispatcher.Setup(m => m.GetModuleRestoreStatus(moduleReference, out nullBuilder)).Returns(ModuleRestoreStatus.Succeeded);
            dispatcher.Setup(m => m.TryGetLocalModuleEntryPointUri(null, moduleReference, out nullBuilder)).Returns(fileUri);

            var resolver = StrictMock.Of<IFileResolver>();
            resolver.Setup(m => m.TryRead(fileUri, out fileContents, out nullBuilder)).Returns(true);

            var handler = new BicepRegistryCacheRequestHandler(dispatcher.Object, resolver.Object);

            var @params = new BicepRegistryCacheParams(ModuleRefStr);
            var response = await handler.Handle(@params, default);

            response.Should().NotBeNull();
            response.Content.Should().Be(fileContents);
        }
    }
}
