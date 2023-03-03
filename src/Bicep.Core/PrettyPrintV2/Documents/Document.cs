// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.Core.PrettyPrintV2.Documents
{
    /// <summary>
    /// A document represents a set of layouts.
    /// </summary>
    public abstract class Document : IEnumerable<Document>
    {
        public static implicit operator Document(string content) => TextDocument.Create(content);

        public abstract IEnumerable<TextDocument> Flatten();

        public IEnumerator<Document> GetEnumerator()
        {
            yield return this;
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
