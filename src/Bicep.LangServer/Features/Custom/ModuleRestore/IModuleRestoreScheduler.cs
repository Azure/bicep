// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Navigation;
using Bicep.Core.Registry;
using Bicep.Core.SourceGraph;
using Bicep.LanguageServer.Compilation;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace Bicep.LanguageServer.Features.Custom.ModuleRestore
{
    public interface IModuleRestoreScheduler
    {
        void Start();

        void RequestModuleRestore(ICompilationManager compilationManager, DocumentUri documentUri, IEnumerable<ArtifactReference> references);
    }
}
