// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.Core.PrettyPrintV2.Documents
{
    public abstract class ContainerDocument : Document
    {
        protected ContainerDocument(IEnumerable<Document> documents)
        {
            this.Documents = documents;
        }

        public IEnumerable<Document> Documents { get; }

        public override IEnumerable<TextDocument> Flatten() => this.Documents.Flatten();
    }
}
