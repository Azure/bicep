// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;

namespace Bicep.Core.PrettyPrintV2.Documents
{
    public class GlueDocument : ContainerDocument
    {
        public GlueDocument(IEnumerable<Document> documents)
            : base([.. documents])
        {
        }
    }
}
