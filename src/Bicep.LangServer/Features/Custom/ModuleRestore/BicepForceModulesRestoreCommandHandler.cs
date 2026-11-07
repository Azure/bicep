// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text;
using Bicep.Core.Artifacts.Redirects;
using Bicep.Core.Artifacts.Restore;
using Bicep.Core.Configuration;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.Core.Utils;
using Bicep.IO.Abstraction;
using Bicep.LanguageServer.Compilation;
using Bicep.LanguageServer.Extensions;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace Bicep.LanguageServer.Features.Custom.ModuleRestore
{
    // This handler is used to force the modules restore for given a bicep file.
    // It returns Restore (force) succeeded/failed message, which can be displayed appropriately in IDE output window
    public class BicepForceModulesRestoreCommandHandler : ExecuteTypedResponseCommandHandlerBase<DocumentUri, string>
    {
        private readonly IFileExplorer fileExplorer;
        private readonly RedirectingArtifactResolver referenceResolver;
        private readonly IRemoteArtifactRestorer restorer;
        private readonly ICompilationManager compilationManager;
        private readonly IActiveSourceFileSet workspace;
        private readonly ISourceFileFactory sourceFileFactory;

        public BicepForceModulesRestoreCommandHandler(
            ISerializer serializer,
            IFileExplorer fileExplorer,
            RedirectingArtifactResolver referenceResolver,
            IRemoteArtifactRestorer restorer,
            ICompilationManager compilationManager,
            IActiveSourceFileSet workspace,
            ISourceFileFactory sourceFileFactory)
            : base(LangServerConstants.ForceModulesRestoreCommand, serializer)
        {
            this.fileExplorer = fileExplorer;
            this.referenceResolver = referenceResolver;
            this.restorer = restorer;
            this.compilationManager = compilationManager;
            this.workspace = workspace;
            this.sourceFileFactory = sourceFileFactory;
        }

        public override Task<string> Handle(DocumentUri documentUri, CancellationToken cancellationToken)
        {
            return ForceModulesRestoreAndGenerateOutputMessage(documentUri);
        }

        private async Task<string> ForceModulesRestoreAndGenerateOutputMessage(DocumentUri documentUri)
        {
            var fileUri = documentUri.ToIOUri();

            var sourceFileGrouping = SourceFileGroupingBuilder.Build(
                this.fileExplorer,
                this.referenceResolver,
                this.restorer,
                this.workspace,
                this.sourceFileFactory,
                fileUri);

            // Ignore modules to restore logic, include all modules to be restored
            var artifactsToRestore = sourceFileGrouping.GetArtifactsToRestore(force: true);

            // RestoreModules() does a distinct but we'll do it also to prevent duplicates in outputs and logging
            var artifactReferencesToRestore = artifactsToRestore.GetRemoteArtifactReferences()
                .OrderBy(key => key.FullyQualifiedReference)
                .ToArray();

            if (artifactReferencesToRestore.Length == 0)
            {
                return $"Restore (force) skipped. No modules references in input file.";
            }

            // restore is supposed to only restore the module references that are syntactically valid
            await this.restorer.RestoreAsync(artifactReferencesToRestore, RestoreMode.Force);

            // if all are marked as success
            var sbRestoreSummary = new StringBuilder();
            foreach (var module in artifactReferencesToRestore)
            {
                var restoreStatus = this.restorer.GetRestoreState(module) switch
                {
                    RemoteArtifactRestoreState.Restored => "Succeeded",
                    RemoteArtifactRestoreState.RestoreFailed => "Failed",
                    _ => "Unknown",
                };
                sbRestoreSummary.Append($"{System.Environment.NewLine}  * {module.FullyQualifiedReference}: {restoreStatus}");
            }

            // refresh all compilations with a reference to this file or cached artifacts
            var artifactUris = artifactsToRestore.Select(x => x.Result.Transform(x => x.Uri.ToDocumentUri()).TryUnwrap()).WhereNotNull();
            compilationManager.RefreshChangedFiles(artifactUris.Concat(documentUri));
            return $"Restore (force) summary: {sbRestoreSummary}";
        }
    }
}
