// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Analyzers.Linter.ApiVersions;
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
using Bicep.LanguageServer.Deploy;
using Bicep.LanguageServer.Utils;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace Bicep.LanguageServer.Handlers
{
    public record BicepDeploymentScopeParams(TextDocumentIdentifier TextDocument) : ITextDocumentIdentifierParams, IRequest<BicepDeploymentScopeResponse>;

    public record BicepDeploymentScopeResponse(string scope, string? template, string? errorMessage);

    /// <summary>
    /// Handles getDeploymentScope LSP request.
    /// The BicepDeploymentScopeRequestHandler returns targetScope, template and error message.
    /// Error message would be null if provided bicep file was error free.
    /// </summary>
    public class BicepDeploymentScopeRequestHandler : ExecuteTypedResponseCommandHandlerBase<BicepDeploymentScopeParams, BicepDeploymentScopeResponse>
    {
        private readonly ICompilationManager compilationManager;
        private readonly IConfigurationManager configurationManager;
        private readonly IDeploymentFileCompilationCache deploymentFileCompilationCache;
        private readonly IFeatureProviderFactory featureProviderFactory;
        private readonly IFileResolver fileResolver;
        private readonly IModuleDispatcher moduleDispatcher;
        private readonly INamespaceProvider namespaceProvider;
        private readonly IApiVersionProviderFactory apiVersionProviderFactory;
        private readonly IBicepAnalyzer bicepAnalyzer;

        public BicepDeploymentScopeRequestHandler(
            ICompilationManager compilationManager,
            IConfigurationManager configurationManager,
            IDeploymentFileCompilationCache deploymentFileCompilationCache,
            IFeatureProviderFactory featureProviderFactory,
            IFileResolver fileResolver,
            IModuleDispatcher moduleDispatcher,
            INamespaceProvider namespaceProvider,
            ISerializer serializer,
            IApiVersionProviderFactory apiVersionProviderFactory,
            IBicepAnalyzer bicepAnalyzer)
            : base(LangServerConstants.GetDeploymentScopeCommand, serializer)
        {
            this.compilationManager = compilationManager;
            this.configurationManager = configurationManager;
            this.deploymentFileCompilationCache = deploymentFileCompilationCache;
            this.featureProviderFactory = featureProviderFactory;
            this.fileResolver = fileResolver;
            this.moduleDispatcher = moduleDispatcher;
            this.namespaceProvider = namespaceProvider;
            this.apiVersionProviderFactory = apiVersionProviderFactory;
            this.bicepAnalyzer = bicepAnalyzer;
        }

        public override Task<BicepDeploymentScopeResponse> Handle(BicepDeploymentScopeParams request, CancellationToken cancellationToken)
        {
            var documentUri = request.TextDocument.Uri;

            Compilation? compilation;

            try
            {
                compilation = GetCompilation(documentUri);

                // Cache the compilation so that it can be reused by BicepDeploymentParametersHandler
                deploymentFileCompilationCache.CacheCompilation(documentUri, compilation);

                var deploymentScope = GetDeploymentScope(compilation.GetEntrypointSemanticModel().TargetScope);

                return Task.FromResult(new BicepDeploymentScopeResponse(deploymentScope, GetCompiledFile(compilation, documentUri), null));
            }
            catch (Exception exception)
            {
                return Task.FromResult(new BicepDeploymentScopeResponse(ResourceScope.None.ToString(), null, exception.Message));
            }
        }

        private static string GetDeploymentScope(ResourceScope resourceScope) => resourceScope switch
        {
            ResourceScope.ResourceGroup => LanguageConstants.TargetScopeTypeResourceGroup,
            ResourceScope.Subscription => LanguageConstants.TargetScopeTypeSubscription,
            ResourceScope.ManagementGroup => LanguageConstants.TargetScopeTypeManagementGroup,
            ResourceScope.Tenant => LanguageConstants.TargetScopeTypeTenant,
            _ => resourceScope.ToString()
        };

        private string GetCompiledFile(Compilation compilation, DocumentUri documentUri)
        {
            var fileUri = documentUri.ToUri();

            var diagnosticsByFile = compilation.GetAllDiagnosticsByBicepFile()
                .FirstOrDefault(x => x.Key.FileUri == fileUri);

            if (diagnosticsByFile.Value.Any(x => x.Level == DiagnosticLevel.Error))
            {
                throw new Exception(DiagnosticsHelper.GetDiagnosticsMessage(diagnosticsByFile));
            }

            var stringBuilder = new StringBuilder();
            var stringWriter = new StringWriter(stringBuilder);

            var model = compilation.GetEntrypointSemanticModel();
            var emitter = new TemplateEmitter(model);
            emitter.Emit(stringWriter);

            return stringBuilder.ToString();
        }

        private Compilation GetCompilation(DocumentUri documentUri)
        {
            var fileUri = documentUri.ToUri();

            CompilationContext? context = compilationManager.GetCompilation(documentUri);
            if (context is null)
            {
                SourceFileGrouping sourceFileGrouping = SourceFileGroupingBuilder.Build(this.fileResolver, this.moduleDispatcher, new Workspace(), fileUri);
                return new Compilation(featureProviderFactory, namespaceProvider, sourceFileGrouping, configurationManager, apiVersionProviderFactory, bicepAnalyzer);
            }
            else
            {
                return context.Compilation;
            }
        }
    }
}
