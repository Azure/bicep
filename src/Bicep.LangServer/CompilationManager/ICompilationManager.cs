// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;


namespace Bicep.LanguageServer.CompilationManager
{    public interface ICompilationManager<T>
    {
        void HandleFileChanges(IEnumerable<FileEvent> fileEvents);

        void RefreshCompilation(DocumentUri uri, bool reloadBicepConfig = false);

        void UpsertCompilation(DocumentUri uri, int? version, string text, string? languageId = null);

        void CloseCompilation(DocumentUri uri);

        T? GetCompilation(DocumentUri uri); //potentailly have different version for bicep params
    }
    public interface ICompilationManager : ICompilationManager<CompilationContext>
    {
    }

    public interface IParamsCompilationManager : ICompilationManager<ParamsCompilationContext>
    {
    }

   
}

