// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;


namespace Bicep.LanguageServer.CompilationManager
{
    public interface ICompilationManager
    {
        void HandleFileChanges(IEnumerable<FileEvent> fileEvents);

        void RefreshCompilation(DocumentUri uri);

        void RefreshAllActiveCompilations();

        void OpenCompilation(DocumentUri uri, int? version, string text, string languageId);

        void UpdateCompilation(DocumentUri uri, int? version, string text);

        void CloseCompilation(DocumentUri uri);

        CompilationContext? GetCompilation(DocumentUri uri);
    }
}
