// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.Core.PrettyPrintV2.Documents
{
    public sealed class LineDocument : Document
    {
        private readonly TextDocument? flattened;

        public LineDocument(TextDocument? flattened)
        {
            this.flattened = flattened;
        }

        public override IEnumerable<TextDocument> Flatten()
        {
            yield return this.flattened ?? throw new InvalidOperationException("The current line should not be flattened.");
        }
    }
}
