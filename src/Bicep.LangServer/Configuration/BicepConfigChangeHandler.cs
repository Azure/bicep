// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Bicep.Core.Configuration;
using Bicep.Core.Workspaces;
using Bicep.LanguageServer.CompilationManager;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace Bicep.LanguageServer.Configuration
{
    public class BicepConfigChangeHandler
    {
        public static void RefreshCompilationOfSourceFilesInWorkspace(ICompilationManager compilationManager, Uri bicepConfigUri, IWorkspace workspace, string bicepConfigFileContents)
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
                compilationManager.RefreshCompilation(DocumentUri.From(sourceFileUri), reloadBicepConfig: true);
            }
        }
    }
}
