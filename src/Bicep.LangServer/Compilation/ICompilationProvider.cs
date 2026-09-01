// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace Bicep.LanguageServer.Compilation
{
    public interface ICompilationProvider
    {
        CompilationContext Create(IActiveSourceFileLookup workspace, DocumentUri documentUri, ImmutableDictionary<ISourceFile, ISemanticModel> modelLookup);

        CompilationContext Update(IActiveSourceFileLookup workspace, CompilationContext current, ImmutableDictionary<ISourceFile, ISemanticModel> modelLookup);
    }
}
