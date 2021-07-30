// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Workspaces;
using Bicep.LanguageServer.CompilationManager;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace Bicep.LanguageServer.Providers
{
    public interface ICompilationProvider
    {
        CompilationContext Create(SourceFileGrouping sourceFileGrouping, DocumentUri documentUri, ImmutableDictionary<ISourceFile, ISemanticModel> modelLookup);
    }
}