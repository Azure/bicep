// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
using Bicep.Core.Configuration;
using Bicep.Core.Workspaces;
using Bicep.LanguageServer.CompilationManager;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace Bicep.LanguageServer.Configuration
{
    public class BicepConfigChangeHandler
    {
        public static void RefreshCompilationOfSourceFilesInWorkspace(ICompilationManager compilationManager, Uri bicepConfigUri, IWorkspace workspace, string? bicepConfigFileContents)
        {
            // BicepDidChangeWatchedFilesHandler sends a notification when bicepconfig.json file is created/deleted/changed.
            // File contents is null in this scenario.
            // If the file doesn't exist on disk, it's a delete event. We'll remove the config if it was cached.
            // For create/change events, we'll upsert the config.
            if (bicepConfigFileContents is null &&
                !File.Exists(bicepConfigUri.LocalPath) &&
                workspace.GetBicepConfig(bicepConfigUri) is not null)
            {
                workspace.RemoveBicepConfig(bicepConfigUri);
            }
            else
            {
                workspace.UpsertBicepConfig(bicepConfigUri, new BicepConfig(bicepConfigUri, bicepConfigFileContents));
            }

            foreach (Uri sourceFileUri in workspace.GetActiveSourceFilesByUri().Keys)
            {
                compilationManager.RefreshCompilation(DocumentUri.From(sourceFileUri), reloadBicepConfig: true);
            }
        }
    }
}
