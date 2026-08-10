// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;


namespace Bicep.LanguageServer.Compilation
{
    public interface ICompilationManager
    {
        void RefreshChangedFiles(IEnumerable<DocumentUri> files);

        void HandleFileChanges(IEnumerable<FileEvent> fileEvents);

        void RefreshCompilation(DocumentUri uri, bool forceReloadAuxiliaryFiles = false);

        void RefreshAllActiveCompilations(bool forceReloadAuxiliaryFiles = false);

        void OpenCompilation(DocumentUri uri, int? version, string text, string languageId);

        void UpdateCompilation(DocumentUri uri, int? version, string text);

        void CloseCompilation(DocumentUri uri);

        CompilationContext? GetCompilation(DocumentUri uri);
    }
}
