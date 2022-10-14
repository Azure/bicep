// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;


namespace Bicep.LanguageServer.CompilationManager
{
    // TODO: Get rid of the generic interface
    public interface ICompilationManager<T>
    {
        void HandleFileChanges(IEnumerable<FileEvent> fileEvents);

        void RefreshCompilation(DocumentUri uri);

        void OpenCompilation(DocumentUri uri, int? version, string text, string languageId);

        void UpdateCompilation(DocumentUri uri, int? version, string text);

        void CloseCompilation(DocumentUri uri);

        T? GetCompilation(DocumentUri uri);
    }

    public interface ICompilationManager : ICompilationManager<CompilationContext>
    {

    }
}
