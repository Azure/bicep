// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Bicep.Core.Workspaces;
using Bicep.LanguageServer.CompilationManager;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace Bicep.LanguageServer.Configuration
{
    public class BicepConfigChangeHandler
    {
        public static void RefreshCompilationOfSourceFilesInWorkspace(ICompilationManager compilationManager, IWorkspace workspace)
        {
            foreach (Uri sourceFileUri in workspace.GetActiveSourceFilesByUri().Keys)
            {
                compilationManager.RefreshCompilation(DocumentUri.From(sourceFileUri), reloadBicepConfig: true);
            }
        }
    }
}
