// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Bicep.Core;
using Bicep.Core.Configuration;
using Bicep.Core.Registry;
using Bicep.Core.SourceGraph;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.Utils;
using Bicep.LanguageServer;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Providers;
using Bicep.LanguageServer.Registry;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace Bicep.LangServer.UnitTests
{
    public class BicepCompilationManagerHelper
    {
        private static readonly MockRepository Repository = new(MockBehavior.Strict);

        public static BicepCompilationManager CreateCompilationManager(DocumentUri documentUri, string fileContents, bool upsertCompilation = false, IWorkspace? workspace = null, IConfigurationManager? configurationManager = null)
        {
            workspace ??= new Workspace();
            PublishDiagnosticsParams? receivedParams = null;
            var document = CreateMockDocument(p => receivedParams = p);
            var server = CreateMockServer(document);

            var bicepCompilationManager = CreateCompilationManager(server.Object, workspace, configurationManager ?? BicepTestConstants.ConfigurationManager);

            if (upsertCompilation)
            {
                bicepCompilationManager.OpenCompilation(documentUri, version: null, fileContents, LanguageConstants.LanguageId);
            }

            return bicepCompilationManager;
        }

        public static BicepCompilationManager CreateCompilationManager(ILanguageServerFacade server, IWorkspace workspace, IConfigurationManager configurationManager)
        {
            var helper = ServiceBuilder.Create(services => services
                .AddSingleton<ILanguageServerFacade>(server)
                .WithAzResourceTypeLoaderFactory(TestTypeHelper.CreateEmptyResourceTypeLoader())
                .AddSingleton(CreateMockScheduler().Object)
                .AddSingleton(BicepTestConstants.CreateMockTelemetryProvider().Object)
                .AddSingleton<ICompilationProvider, BicepCompilationProvider>()
                .AddSingleton<IWorkspace>(workspace)
                .WithConfigurationManager(configurationManager)
                .WithFeatureOverrides(new FeatureProviderOverrides(
                    // This is necessary to avoid hard-coding a particular version number into a compiled template
                    AssemblyVersion: BicepTestConstants.DevAssemblyFileVersion))
                .AddSingleton<BicepCompilationManager>());

            return helper.Construct<BicepCompilationManager>();
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

        public static ICompilationProvider CreateEmptyCompilationProvider(IConfigurationManager? configurationManager = null)
        {
            var helper = ServiceBuilder.Create(services => services
                .AddSingleton(TestTypeHelper.CreateEmptyResourceTypeLoader())
                .AddSingletonIfNotNull<IConfigurationManager>(configurationManager)
                .AddSingleton<BicepCompilationProvider>());

            return helper.Construct<BicepCompilationProvider>();
        }

        public static Mock<IModuleRestoreScheduler> CreateMockScheduler()
        {
            var scheduler = Repository.Create<IModuleRestoreScheduler>();
            scheduler.Setup(m => m.RequestModuleRestore(It.IsAny<ICompilationManager>(), It.IsAny<DocumentUri>(), It.IsAny<IEnumerable<ArtifactReference>>()));

            return scheduler;
        }
    }
}
