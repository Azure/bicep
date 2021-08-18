// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Bicep.Core.FileSystem;
using Bicep.Core.Workspaces;
using Bicep.LanguageServer.CompilationManager;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
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

        public void RetriggerCompilationOfSourceFilesInWorkspace(ICompilationManager compilationManager, FileEvent bicepConfigFileEvent, IWorkspace workspace)
        {
            foreach (ISourceFile sourceFile in workspace.GetSourceFilesForDirectory(bicepConfigFileEvent.Uri.ToUri()))
            {
                Uri uri = sourceFile.FileUri;

                if (fileResolver.TryRead(uri, out string? bicepFileContents, out ErrorBuilderDelegate _) &&
                    !string.IsNullOrWhiteSpace(bicepFileContents))
                {
                    compilationManager.UpsertCompilation(DocumentUri.From(uri), null, bicepFileContents, reloadBicepConfig: true);
                }
            }
        }
    }
}
