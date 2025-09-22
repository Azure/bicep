// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text;
using Bicep.Core.Configuration;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.Core.Utils;
using Bicep.IO.Abstraction;
using Bicep.LanguageServer.CompilationManager;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace Bicep.LanguageServer.Handlers
{
    // This handler is used to force the modules restore for given a bicep file.
    // It returns Restore (force) succeeded/failed message, which can be displayed appropriately in IDE output window
    public class BicepForceModulesRestoreCommandHandler : ExecuteTypedResponseCommandHandlerBase<DocumentUri, string>
    {
        private readonly IFileExplorer fileExplorer;
        private readonly IModuleDispatcher moduleDispatcher;
        private readonly ICompilationManager compilationManager;
        private readonly IActiveSourceFileSet workspace;
        private readonly ISourceFileFactory sourceFileFactory;

        public BicepForceModulesRestoreCommandHandler(
            ISerializer serializer,
            IFileExplorer fileExplorer,
            IModuleDispatcher moduleDispatcher,
            ICompilationManager compilationManager,
            IActiveSourceFileSet workspace,
            ISourceFileFactory sourceFileFactory)
            : base(LangServerConstants.ForceModulesRestoreCommand, serializer)
        {
            this.fileExplorer = fileExplorer;
            this.moduleDispatcher = moduleDispatcher;
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
                this.moduleDispatcher,
                this.workspace,
                this.sourceFileFactory,
                fileUri);

            // Ignore modules to restore logic, include all modules to be restored
            var artifactsToRestore = sourceFileGrouping.GetArtifactsToRestore(force: true);

            // RestoreModules() does a distinct but we'll do it also to prevent duplicates in outputs and logging
            var artifactReferencesToRestore = ArtifactHelper.GetValidArtifactReferences(artifactsToRestore)
                .OrderBy(key => key.FullyQualifiedReference)
                .ToArray();

            if (artifactReferencesToRestore.Length == 0)
            {
                return $"Restore (force) skipped. No modules references in input file.";
            }

            // restore is supposed to only restore the module references that are syntactically valid
            await this.moduleDispatcher.RestoreArtifacts(artifactReferencesToRestore, forceRestore: true);

            // if all are marked as success
            var sbRestoreSummary = new StringBuilder();
            foreach (var module in artifactReferencesToRestore)
            {
                var restoreStatus = this.moduleDispatcher.GetArtifactRestoreStatus(module, out _);
                sbRestoreSummary.Append($"{System.Environment.NewLine}  * {module.FullyQualifiedReference}: {restoreStatus}");
            }

            // refresh all compilations with a reference to this file or cached artifacts
            var artifactUris = artifactsToRestore.Select(x => x.Result.Transform(x => x.Uri.ToUri()).TryUnwrap()).WhereNotNull();
            compilationManager.RefreshChangedFiles(artifactUris.Concat(documentUri.ToUriEncoded()));
            return $"Restore (force) summary: {sbRestoreSummary}";
        }
    }
}
