// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using Bicep.Core.Workspaces;
using Bicep.LanguageServer.CompilationManager;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace Bicep.LanguageServer.Registry
{
    public interface IModuleRestoreScheduler
    {
        void Start();

        void RequestModuleRestore(ICompilationManager compilationManager, DocumentUri documentUri, IEnumerable<ArtifactResolutionInfo> references);
    }
}
