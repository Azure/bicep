// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.LanguageServer.CompilationManager;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace Bicep.LanguageServer.Providers
{
    public interface ICompilationProvider
    {
        CompilationContext Create(DocumentUri documentUri, string text);
    }
}
