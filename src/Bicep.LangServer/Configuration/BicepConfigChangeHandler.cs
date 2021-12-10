// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Linq;
using Bicep.Core.Workspaces;
using Bicep.LanguageServer.CompilationManager;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace Bicep.LanguageServer.Configuration
{
    public class BicepConfigChangeHandler
    {
        public static void RefreshCompilationOfSourceFilesInWorkspace(ICompilationManager compilationManager, IWorkspace workspace)
        {
            var sourceFiles = workspace.GetActiveSourceFilesByUri().Keys;
            for (int i = 0; i < sourceFiles.Count(); i++)
            {
                var sourceFileUri = sourceFiles.ElementAt(i);
                if (i == 0)
                {
                    compilationManager.RefreshCompilation(DocumentUri.From(sourceFileUri), reloadBicepConfig: true, sendTelemetryOnBicepConfigChange: true);
                }
                else
                {
                    compilationManager.RefreshCompilation(DocumentUri.From(sourceFileUri), reloadBicepConfig: true);
                }
            }
        }
    }
}
