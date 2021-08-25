// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Bicep.Core.Configuration;
using Bicep.Core.FileSystem;
using Bicep.Core.Workspaces;
using Bicep.LanguageServer.CompilationManager;
using OmniSharp.Extensions.LanguageServer.Protocol;
using static Bicep.Core.Diagnostics.DiagnosticBuilder;

namespace Bicep.LanguageServer.Configuration
{
    public class BicepConfigChangeHandler : IBicepConfigChangeHandler
    {
        private readonly IFileResolver fileResolver;

        public BicepConfigChangeHandler(IFileResolver fileResolver)
        {
            this.fileResolver = fileResolver;
        }

        public void RetriggerCompilationOfSourceFilesInWorkspace(ICompilationManager compilationManager, Uri bicepConfigUri, IWorkspace workspace, string bicepConfigFileContents)
        {
            // bicepconfig.json file was deleted
            if (string.IsNullOrWhiteSpace(bicepConfigFileContents))
            {
                workspace.UpsertActiveBicepConfig(null);
            }
            else
            {
                workspace.UpsertActiveBicepConfig(new BicepConfig(bicepConfigUri, bicepConfigFileContents));
            }

            foreach (Uri sourceFileUri in workspace.GetActiveSourceFilesByUri().Keys)
            {
                if (fileResolver.TryRead(sourceFileUri, out string? bicepFileContents, out ErrorBuilderDelegate _) &&
                    !string.IsNullOrWhiteSpace(bicepFileContents))
                {
                    compilationManager.UpsertCompilation(DocumentUri.From(sourceFileUri), null, bicepFileContents, reloadBicepConfig: true);
                }
            }
        }
    }
}
