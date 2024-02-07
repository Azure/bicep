// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.Syntax;
using Bicep.Core.Workspaces;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace Bicep.LanguageServer.Handlers
{
    // This handler is used to force the modules restore for given a bicep file.
    // It returns Restore (force) succeeded/failed message, which can be displayed approriately in IDE output window
    public class BicepForceModulesRestoreCommandHandler(
        ISerializer serializer,
        IFileResolver fileResolver,
        IModuleDispatcher moduleDispatcher,
        IWorkspace workspace,
        IFeatureProviderFactory featureProviderFactory) : ExecuteTypedResponseCommandHandlerBase<string, string>(LangServerConstants.ForceModulesRestoreCommand, serializer)
    {
        private readonly IFileResolver fileResolver = fileResolver;
        private readonly IModuleDispatcher moduleDispatcher = moduleDispatcher;
        private readonly IWorkspace workspace = workspace;
        private readonly IFeatureProviderFactory featureProviderFactory = featureProviderFactory;

        public override Task<string> Handle(string bicepFilePath, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(bicepFilePath))
            {
                throw new ArgumentException("Invalid input file path");
            }

            DocumentUri documentUri = DocumentUri.FromFileSystemPath(bicepFilePath);
            Task<string> restoreOutput = ForceModulesRestoreAndGenerateOutputMessage(documentUri);

            return restoreOutput;
        }

        private async Task<string> ForceModulesRestoreAndGenerateOutputMessage(DocumentUri documentUri)
        {
            var fileUri = documentUri.ToUriEncoded();

            SourceFileGrouping sourceFileGrouping = SourceFileGroupingBuilder.Build(
                this.fileResolver,
                this.moduleDispatcher,
                workspace,
                fileUri,
                featureProviderFactory);

            // Ignore modules to restore logic, include all modules to be restored
            var artifactsToRestore = sourceFileGrouping.FileUriResultByArtifactReference
                .SelectMany(kvp => kvp.Value.Keys.Where(x => x is ModuleDeclarationSyntax or UsingDeclarationSyntax).Select(mds => new ArtifactResolutionInfo(mds, kvp.Key)));

            // RestoreModules() does a distinct but we'll do it also to prevent duplicates in outputs and logging
            var artifactReferencesToRestore = this.moduleDispatcher.GetValidModuleReferences(artifactsToRestore)
                .Distinct()
                .OrderBy(key => key.FullyQualifiedReference);

            if (!artifactReferencesToRestore.Any())
            {
                return $"Restore (force) skipped. No modules references in input file.";
            }

            // restore is supposed to only restore the module references that are syntactically valid
            await this.moduleDispatcher.RestoreModules(artifactReferencesToRestore, forceRestore: true);

            // if all are marked as success
            var sbRestoreSummary = new StringBuilder();
            foreach (var module in artifactReferencesToRestore)
            {
                var restoreStatus = this.moduleDispatcher.GetArtifactRestoreStatus(module, out _);
                sbRestoreSummary.Append($"{Environment.NewLine}  * {module.FullyQualifiedReference}: {restoreStatus}");
            }

            // Have to actually update compilations to pick up new modules' contents
            workspace.UpsertSourceFiles(sourceFileGrouping.SourceFiles);

            return $"Restore (force) summary: {sbRestoreSummary}";
        }
    }
}
