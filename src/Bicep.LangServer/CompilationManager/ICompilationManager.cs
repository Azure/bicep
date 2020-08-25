// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using OmniSharp.Extensions.LanguageServer.Protocol;

namespace Bicep.LanguageServer.CompilationManager
{
    public interface ICompilationManager
    {
        CompilationContext? UpsertCompilation(DocumentUri uri, long version, string text);

        void CloseCompilation(DocumentUri uri);

        CompilationContext? GetCompilation(DocumentUri uri);
    }
}

