// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core;
using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.TypeSystem;
using Bicep.Core.Workspaces;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Utils;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Handlers
{
    [Method(BicepDeploymentScopeRequestHandler.BicepDeploymentScopeLspMethod, Direction.ClientToServer)]
    public record BicepDeploymentScopeParams(TextDocumentIdentifier TextDocument) : ITextDocumentIdentifierParams, IRequest<BicepDeploymentScopeResponse>;

    public record BicepDeploymentScopeResponse(string scope, string? template, string? errorMessage);

    /// <summary>
    /// Handles textDocument/deploymentScope LSP request.
    /// The BicepDeploymentScopeRequestHandler returns targetScope, template and error message.
    /// Error message would be null if provided bicep file was error free.
    /// </summary>
    public class BicepDeploymentScopeRequestHandler : IJsonRpcRequestHandler<BicepDeploymentScopeParams, BicepDeploymentScopeResponse>
    {
        public const string BicepDeploymentScopeLspMethod = "textDocument/deploymentScope";

        private readonly EmitterSettings emitterSettings;
        private readonly ICompilationManager compilationManager;
        private readonly IConfigurationManager configurationManager;
        private readonly IFeatureProvider features;
        private readonly IFileResolver fileResolver;
        private readonly IModuleDispatcher moduleDispatcher;
        private readonly INamespaceProvider namespaceProvider;

        public BicepDeploymentScopeRequestHandler(
            EmitterSettings emitterSettings,
            ICompilationManager compilationManager,
            IConfigurationManager configurationManager,
            IFeatureProvider features,
            IFileResolver fileResolver,
            IModuleDispatcher moduleDispatcher,
            INamespaceProvider namespaceProvider)
        {
            this.compilationManager = compilationManager;
            this.configurationManager = configurationManager;
            this.emitterSettings = emitterSettings;
            this.features = features;
            this.fileResolver = fileResolver;
            this.moduleDispatcher = moduleDispatcher;
            this.namespaceProvider = namespaceProvider;
        }

        public Task<BicepDeploymentScopeResponse> Handle(BicepDeploymentScopeParams request, CancellationToken cancellationToken)
        {
            var documentUri = request.TextDocument.Uri;

            Compilation? compilation;

            try
            {
                compilation = GetCompilation(documentUri);
                var deploymentScope = GetDeploymentScope(compilation.GetEntrypointSemanticModel().TargetScope);

                return Task.FromResult(new BicepDeploymentScopeResponse(deploymentScope, GetCompiledFile(compilation, documentUri), null));
            }
            catch (Exception exception)
            {
                return Task.FromResult(new BicepDeploymentScopeResponse(ResourceScope.None.ToString(), null, exception.Message));
            }
        }

        private static string GetDeploymentScope(ResourceScope resourceScope)
        {
            switch (resourceScope)
            {
                case ResourceScope.ResourceGroup:
                    return LanguageConstants.TargetScopeTypeResourceGroup;
                case ResourceScope.Subscription:
                    return LanguageConstants.TargetScopeTypeSubscription;
                case ResourceScope.ManagementGroup:
                    return LanguageConstants.TargetScopeTypeManagementGroup;
                case ResourceScope.Tenant:
                    return LanguageConstants.TargetScopeTypeTenant;
                default:
                    return resourceScope.ToString();
            }
        }

        private string GetCompiledFile(Compilation compilation, DocumentUri documentUri)
        {
            var fileUri = documentUri.ToUri();

            KeyValuePair<BicepFile, IEnumerable<IDiagnostic>> diagnosticsByFile = compilation.GetAllDiagnosticsByBicepFile()
                .FirstOrDefault(x => x.Key.FileUri == fileUri);

            if (diagnosticsByFile.Value.Any(x => x.Level == DiagnosticLevel.Error))
            {
                throw new Exception(DiagnosticsHelper.GetDiagnosticsMessage(diagnosticsByFile));
            }

            var stringBuilder = new StringBuilder();
            var stringWriter = new StringWriter(stringBuilder);

            var emitter = new TemplateEmitter(compilation.GetEntrypointSemanticModel(), emitterSettings);
            emitter.Emit(stringWriter);

            return stringBuilder.ToString();
        }

        private Compilation GetCompilation(DocumentUri documentUri)
        {
            var fileUri = documentUri.ToUri();
            RootConfiguration? configuration;

            try
            {
                configuration = this.configurationManager.GetConfiguration(fileUri);
            }
            catch (ConfigurationException exception)
            {
                throw exception;
            }

            CompilationContext? context = compilationManager.GetCompilation(fileUri);
            if (context is null)
            {
                SourceFileGrouping sourceFileGrouping = SourceFileGroupingBuilder.Build(this.fileResolver, this.moduleDispatcher, new Workspace(), fileUri, configuration);
                return new Compilation(features, namespaceProvider, sourceFileGrouping, configuration, new LinterAnalyzer(configuration));
            }
            else
            {
                return context.Compilation;
            }
        }
    }
}
