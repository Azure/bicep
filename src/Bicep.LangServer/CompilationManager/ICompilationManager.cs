// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;


namespace Bicep.LanguageServer.CompilationManager
{
    public interface ICompilationManager<T>
    {
        void HandleFileChanges(IEnumerable<FileEvent> fileEvents);

        void RefreshCompilation(DocumentUri uri);

        void UpsertCompilation(DocumentUri uri, int? version, string text, string? languageId = null, bool triggeredByFileOpenEvent = false);

        void CloseCompilation(DocumentUri uri);

        T? GetCompilation(DocumentUri uri);
    }
    public interface ICompilationManager : ICompilationManager<CompilationContext>
    {

    }

    public interface IParamsCompilationManager : ICompilationManager<ParamsCompilationContext>
    {

    }
}
