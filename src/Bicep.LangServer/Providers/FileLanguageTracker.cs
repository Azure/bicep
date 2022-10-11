// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using OmniSharp.Extensions.LanguageServer.Protocol;
using System.Collections.Concurrent;

namespace Bicep.LanguageServer.Providers
{
    public class FileLanguageTracker : IFileLanguageTracker
    {
        private readonly ConcurrentDictionary<DocumentUri, string> activeDocumentLanguageMap = new();

        public void NotifyFileClose(DocumentUri documentUri) => this.activeDocumentLanguageMap.TryRemove(documentUri, out _);

        public void NotifyFileOpen(DocumentUri documentUri, string languageId) =>
            // I'm not 100% confident that we are always getting file close events
            // overwriting the mapping allows us to self-correct if we ever lose a file close event
            this.activeDocumentLanguageMap.AddOrUpdate(documentUri, languageId, (_, _) => languageId);

        public string? TryGetLanguageId(DocumentUri documentUri)
        {
            if(activeDocumentLanguageMap.TryGetValue(documentUri, out var languageId))
            {
                return languageId;
            }

            return null;
        }
    }
}
