// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Bicep.Core;
using Bicep.Core.Configuration;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.Syntax;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Workspaces;
using Bicep.LanguageServer;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Providers;
using Bicep.LanguageServer.Registry;
using Moq;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using IOFileSystem = System.IO.Abstractions.FileSystem;

namespace Bicep.LangServer.UnitTests
{
    public class BicepCompilationManagerHelper
    {
        private static readonly FileResolver FileResolver = new();
        private static readonly MockRepository Repository = new(MockBehavior.Strict);

        public static BicepCompilationManager CreateCompilationManager(DocumentUri documentUri, string fileContents, bool upsertCompilation = false)
        {
            PublishDiagnosticsParams? receivedParams = null;

            var document = CreateMockDocument(p => receivedParams = p);
            var server = CreateMockServer(document);
            BicepCompilationManager bicepCompilationManager = new(server.Object, CreateEmptyCompilationProvider(), new Workspace(), FileResolver, CreateMockScheduler().Object, new ConfigurationManager(new IOFileSystem()));

            if (upsertCompilation)
            {
                bicepCompilationManager.UpsertCompilation(documentUri, version: null, fileContents, LanguageConstants.LanguageId);
            }

            return bicepCompilationManager;
        }

        public static Mock<ITextDocumentLanguageServer> CreateMockDocument(Action<PublishDiagnosticsParams> callback)
        {
            var document = Repository.Create<ITextDocumentLanguageServer>();
            document
                .Setup(m => m.SendNotification(It.IsAny<MediatR.IRequest>()))
                .Callback<MediatR.IRequest>((p) => callback((PublishDiagnosticsParams)p))
                .Verifiable();

            return document;
        }

        public static Mock<ILanguageServerFacade> CreateMockServer(Mock<ITextDocumentLanguageServer> document)
        {
            var server = Repository.Create<ILanguageServerFacade>();
            server
                .Setup(m => m.TextDocument)
                .Returns(document.Object);

            var window = Repository.Create<IWindowLanguageServer>();
            window
                .Setup(m => m.SendNotification(It.IsAny<LogMessageParams>()));

            server
                .Setup(m => m.Window)
                .Returns(window.Object);

            return server;
        }

        public static ICompilationProvider CreateEmptyCompilationProvider()
        {
            return new BicepCompilationProvider(TestTypeHelper.CreateEmptyProvider(), FileResolver, new ModuleDispatcher(BicepTestConstants.RegistryProvider));
        }

        public static Mock<IModuleRestoreScheduler> CreateMockScheduler()
        {
            var scheduler = Repository.Create<IModuleRestoreScheduler>();
            scheduler.Setup(m => m.RequestModuleRestore(It.IsAny<ICompilationManager>(), It.IsAny<DocumentUri>(), It.IsAny<IEnumerable<ModuleDeclarationSyntax>>()));

            return scheduler;
        }
    }
}
