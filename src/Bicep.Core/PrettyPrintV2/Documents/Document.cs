// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using System.Collections.Generic;

namespace Bicep.Core.PrettyPrintV2.Documents
{
    /// <summary>
    /// A document represents a set of layouts.
    /// </summary>
    public abstract class Document : IEnumerable<Document>
    {
        public static implicit operator Document(string content) => TextDocument.From(content);

        public abstract IEnumerable<TextDocument> Flatten();

        public IEnumerator<Document> GetEnumerator()
        {
            yield return this;
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
