// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Bicep.Core.Workspaces;
using Bicep.LanguageServer.CompilationManager;

namespace Bicep.LanguageServer.Configuration
{
    public interface IBicepConfigChangeHandler
    {
        void RetriggerCompilationOfSourceFilesInWorkspace(ICompilationManager compilationManager, Uri bicepConfigUri, IWorkspace workspace, string bicepConfigContents);
    }
}
