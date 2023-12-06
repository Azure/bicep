// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.Core.PrettyPrintV2.Documents
{
    public sealed class ConditionalTextDocument : TextDocument
    {
        private readonly TextDocument onFlatten;

        public ConditionalTextDocument(string onBreak, string onFlatten)
            : base(onBreak)
        {
            this.onFlatten = onFlatten;
        }

        public override IEnumerable<TextDocument> Flatten()
        {
            yield return this.onFlatten;
        }
    }
}
