// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Collections.Immutable;

namespace Bicep.Core.PrettyPrintV2.Documents
{
    public sealed class GlueDocument : ContainerDocument
    {
        public GlueDocument(IEnumerable<Document> documents)
            : base(documents.ToImmutableArray())
        {
        }
    }
}
