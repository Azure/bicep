// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;

namespace Bicep.Core.PrettyPrintV2.Documents
{
    public sealed class IndentDocument(IEnumerable<Document> documents) : ContainerDocument(documents.ToImmutableArray())
    {
    }
}
