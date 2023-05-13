// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.Core.PrettyPrintV2.Documents
{
    public sealed class SuffixDocument : TextDocument
    {
        public SuffixDocument(string value)
            : base(value)
        {
        }

        public override int Width => 0;
    }
}
