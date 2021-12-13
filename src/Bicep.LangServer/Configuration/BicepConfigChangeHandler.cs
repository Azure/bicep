// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Bicep.Core.Workspaces;
using Bicep.LanguageServer.CompilationManager;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace Bicep.LanguageServer.Configuration
{
    public class BicepConfigChangeHandler : IBicepConfigChangeHandler
    {
        private readonly IWorkspace workspace;
        private readonly ICompilationManager compilationManager;

        public BicepConfigChangeHandler(ICompilationManager compilationManager, IWorkspace workspace)
        {
            this.compilationManager = compilationManager;
            this.workspace = workspace;
        }

        public void RefreshCompilationOfSourceFilesInWorkspace()
        {
            foreach (Uri sourceFileUri in workspace.GetActiveSourceFilesByUri().Keys)
            {
                compilationManager.RefreshCompilation(DocumentUri.From(sourceFileUri), reloadBicepConfig: true);
            }
        }
    }
}
