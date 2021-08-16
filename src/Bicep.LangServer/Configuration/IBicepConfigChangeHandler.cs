// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Workspaces;
using Bicep.LanguageServer.CompilationManager;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Configuration
{
    public interface IBicepConfigChangeHandler
    {
        void RetriggerCompilationOfSourceFilesInWorkspace(ICompilationManager compilationManager, FileEvent bicepConfigFileEvent, IWorkspace workspace);
    }
}
