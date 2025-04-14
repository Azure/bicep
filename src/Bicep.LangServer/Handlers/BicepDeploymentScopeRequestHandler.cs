// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text;
using Bicep.Core;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Semantics;
using Bicep.Core.TypeSystem;
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
        private readonly BicepCompiler bicepCompiler;
        private readonly ICompilationManager compilationManager;
        private readonly IDeploymentFileCompilationCache deploymentFileCompilationCache;

        public BicepDeploymentScopeRequestHandler(
            BicepCompiler bicepCompiler,
            ICompilationManager compilationManager,
            IDeploymentFileCompilationCache deploymentFileCompilationCache,
            ISerializer serializer)
            : base(LangServerConstants.GetDeploymentScopeCommand, serializer)
        {
            this.bicepCompiler = bicepCompiler;
            this.compilationManager = compilationManager;
            this.deploymentFileCompilationCache = deploymentFileCompilationCache;
        }

        public override async Task<BicepDeploymentScopeResponse> Handle(BicepDeploymentScopeParams request, CancellationToken cancellationToken)
        {
            try
            {
                var documentUri = request.TextDocument.Uri;
                var compilation = await new CompilationHelper(bicepCompiler, compilationManager).GetRefreshedCompilation(documentUri);

                // Cache the compilation so that it can be reused by BicepDeploymentParametersHandler
                deploymentFileCompilationCache.CacheCompilation(documentUri, compilation);

                var deploymentScope = GetDeploymentScope(compilation.GetEntrypointSemanticModel().TargetScope);

                return new(deploymentScope, GetCompiledFile(compilation, documentUri), null);
            }
            catch (Exception exception)
            {
                return new(ResourceScope.None.ToString(), null, exception.Message);
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
            var fileUri = documentUri.ToIOUri();

            var diagnosticsByFile = compilation.GetAllDiagnosticsByBicepFile()
                .FirstOrDefault(x => x.Key.FileHandle.Uri == fileUri);

            if (diagnosticsByFile.Value.Any(x => x.IsError()))
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
    }
}
