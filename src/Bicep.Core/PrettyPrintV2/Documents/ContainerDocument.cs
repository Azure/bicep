// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;

namespace Bicep.Core.PrettyPrintV2.Documents
{
    public abstract class ContainerDocument : Document
    {
        protected ContainerDocument(ImmutableArray<Document> documents)
        {
            if (documents.Length == 0)
            {
                throw new ArgumentException("Expected non-empty documents.", nameof(documents));
            }

            this.Documents = documents;
        }

        public ImmutableArray<Document> Documents { get; }

        public override IEnumerable<Document> Flatten() => this.Documents.SelectMany(x => x.Flatten());

        public bool HasSuffix() => this.Documents[^1] switch
        {
            SuffixDocument => true,
            ContainerDocument container => container.HasSuffix(),
            _ => false,
        };
    }
}
