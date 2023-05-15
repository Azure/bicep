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
    public sealed class GroupDocument : ContainerDocument
    {
        public GroupDocument(IEnumerable<Document> documents)
            : base(documents.ToImmutableArray())
        {
        }
    }
}
