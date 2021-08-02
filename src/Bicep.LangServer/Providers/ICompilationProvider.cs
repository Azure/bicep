// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Workspaces;
using Bicep.LanguageServer.CompilationManager;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace Bicep.LanguageServer.Providers
{
    public interface ICompilationProvider
    {
        CompilationContext Create(IReadOnlyWorkspace workspace, DocumentUri documentUri);

        CompilationContext Update(IReadOnlyWorkspace workspace, CompilationContext current);
    }
}
