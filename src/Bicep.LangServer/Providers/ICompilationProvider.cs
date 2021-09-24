// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Configuration;
using Bicep.Core.Semantics;
using Bicep.Core.Workspaces;
using Bicep.LanguageServer.CompilationManager;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace Bicep.LanguageServer.Providers
{
    public interface ICompilationProvider
    {
        CompilationContext Create(IReadOnlyWorkspace workspace, DocumentUri documentUri, ImmutableDictionary<ISourceFile, ISemanticModel> modelLookup, RootConfiguration configuration);

        CompilationContext Update(IReadOnlyWorkspace workspace, CompilationContext current, ImmutableDictionary<ISourceFile, ISemanticModel> modelLookup, RootConfiguration configuration);
    }
}
