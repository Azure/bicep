// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Syntax;
using Bicep.Core.Workspaces;
using Bicep.LanguageServer.CompilationManager;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace Bicep.LanguageServer.Providers
{
    public interface ICompilationProvider
    {
        SyntaxTree BuildSyntaxTree(DocumentUri documentUri, string fileContents);

        CompilationContext Create(IReadOnlyWorkspace workspace, DocumentUri documentUri);
    }
}