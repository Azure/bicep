// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text;
using Bicep.Core.Configuration;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.Syntax;
using Bicep.Core.Workspaces;
using Bicep.LanguageServer.CompilationManager;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace Bicep.LanguageServer.Handlers
{
    // This handler is used to force the modules restore for given a bicep file.
    // It returns Restore (force) succeeded/failed message, which can be displayed approriately in IDE output window
    public class BicepForceModulesRestoreCommandHandler : ExecuteTypedResponseCommandHandlerBase<string, string>
    {
        private readonly IFileResolver fileResolver;
        private readonly IModuleDispatcher moduleDispatcher;
        private readonly ICompilationManager compilationManager;
        private readonly IConfigurationManager configurationManager;
        private readonly IWorkspace workspace;
        private readonly IFeatureProviderFactory featureProviderFactory;

        public BicepForceModulesRestoreCommandHandler(
            ISerializer serializer,
            IFileResolver fileResolver,
            IModuleDispatcher moduleDispatcher,
            IConfigurationManager configurationManager,
            ICompilationManager compilationManager,
            IWorkspace workspace,
            IFeatureProviderFactory featureProviderFactory)
            : base(LangServerConstants.ForceModulesRestoreCommand, serializer)
        {
            this.fileResolver = fileResolver;
            this.moduleDispatcher = moduleDispatcher;
            this.configurationManager = configurationManager;
            this.compilationManager = compilationManager;
            this.workspace = workspace;
            this.featureProviderFactory = featureProviderFactory;
        }

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

            var sourceFileGrouping = SourceFileGroupingBuilder.Build(
                this.fileResolver,
                this.moduleDispatcher,
                this.configurationManager,
                workspace,
                fileUri,
                featureProviderFactory);

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
                sbRestoreSummary.Append($"{Environment.NewLine}  * {module.FullyQualifiedReference}: {restoreStatus}");
            }

            // refresh all compilations with a reference to this file or cached artifacts
            var artifactUris = artifactsToRestore.Select(x => x.Result.TryUnwrap()).WhereNotNull();
            compilationManager.RefreshChangedFiles(artifactUris.Concat(documentUri.ToUriEncoded()));
            return $"Restore (force) summary: {sbRestoreSummary}";
        }
    }
}
