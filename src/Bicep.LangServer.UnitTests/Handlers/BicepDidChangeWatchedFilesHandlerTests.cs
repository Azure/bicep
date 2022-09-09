// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Bicep.Core.Workspaces;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Configuration;
using Bicep.LanguageServer.Handlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LangServer.UnitTests.Handlers
{
    [TestClass]
    public class BicepDidChangeWatchedFilesHandlerTests
    {
        [TestMethod]
        public async Task Handle_WithUpdateOfOneOrMoreBicepConfigFiles_ShouldNotifyHandlerAndRefreshCompilationOfAllActiveFiles()
        {
            var workspaceFiles = (new []
            {
                "/projectDir/subDir/main.bicep",
                "/projectDir/subDir/moduleDir/module.bicep",
                "/projectDir/subDir/moduleDir/submodule.bicep",
                "/projectDir/other.bicep",
            }).Select(DocumentUri.FromFileSystemPath).ToImmutableArray();

            var configurationFiles = (new []
            {
                "/projectDir/subDir/bicepconfig.json",
                "/projectDir/subDir/moduleDir/bicepconfig.json",
            }).Select(DocumentUri.FromFileSystemPath).ToImmutableArray();

            var fileEvents = Container.From(configurationFiles.Concat(workspaceFiles).Select(uri => new FileEvent { Uri = uri }));

            var workspace = new Mock<IWorkspace>(MockBehavior.Strict);
            workspace.Setup(w => w.GetActiveSourceFilesByUri())
                .Returns(workspaceFiles.ToImmutableDictionary(uri => uri.ToUri(), uri => new Mock<ISourceFile>(MockBehavior.Strict).Object));

            var compilationManager = new Mock<ICompilationManager>(MockBehavior.Strict);
            foreach (var workspaceFileUri in workspaceFiles)
            {
                compilationManager.Setup(c => c.RefreshCompilation(workspaceFileUri));
            }
            compilationManager.Setup(c => c.HandleFileChanges(fileEvents));

            var changeHandler = new Mock<IBicepConfigChangeHandler>(MockBehavior.Strict);
            foreach (var configFileUri in configurationFiles)
            {
                changeHandler.Setup(c => c.HandleBicepConfigChangeEvent(configFileUri));
            }

            var sut = new BicepDidChangeWatchedFilesHandler(compilationManager.Object, changeHandler.Object, workspace.Object);

            await sut.Handle(new DidChangeWatchedFilesParams { Changes = fileEvents }, new System.Threading.CancellationToken());

            foreach (var configFileUri in configurationFiles)
            {
                changeHandler.Verify(c => c.HandleBicepConfigChangeEvent(configFileUri), Times.Once);
            }

            foreach (var workspaceFileUri in workspaceFiles)
            {
                compilationManager.Verify(c => c.RefreshCompilation(workspaceFileUri), Times.Once);
            }

            compilationManager.Verify(c => c.HandleFileChanges(fileEvents), Times.Once);
        }

        [TestMethod]
        public async Task Handle_WithUpdateNotContainingConfigFiles_ShouldNotNotifyHandlerOrRefreshCompilationOfAnyFiles()
        {
            var workspaceFiles = (new []
            {
                "/projectDir/subDir/main.bicep",
                "/projectDir/subDir/moduleDir/module.bicep",
                "/projectDir/subDir/moduleDir/submodule.bicep",
                "/projectDir/other.bicep",
            }).Select(DocumentUri.FromFileSystemPath).ToImmutableArray();

            var fileEvents = Container.From(workspaceFiles.Select(uri => new FileEvent { Uri = uri }));

            var workspace = new Mock<IWorkspace>(MockBehavior.Strict);
            workspace.Setup(w => w.GetActiveSourceFilesByUri())
                .Returns(workspaceFiles.ToImmutableDictionary(uri => uri.ToUri(), uri => new Mock<ISourceFile>(MockBehavior.Strict).Object));

            var compilationManager = new Mock<ICompilationManager>(MockBehavior.Strict);
            compilationManager.Setup(c => c.HandleFileChanges(fileEvents));

            var changeHandler = new Mock<IBicepConfigChangeHandler>(MockBehavior.Strict);

            var sut = new BicepDidChangeWatchedFilesHandler(compilationManager.Object, changeHandler.Object, workspace.Object);

            await sut.Handle(new DidChangeWatchedFilesParams { Changes = fileEvents }, new System.Threading.CancellationToken());

            foreach (var workspaceFileUri in workspaceFiles)
            {
                compilationManager.Verify(c => c.RefreshCompilation(workspaceFileUri), Times.Never);
            }

            compilationManager.Verify(c => c.HandleFileChanges(fileEvents), Times.Once);
        }
    }
}
