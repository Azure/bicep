// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services.Configuration;

namespace Bicep.Core.PrettyPrintV2.Documents
{
    public sealed class FunctionTailObjectOrArrayDocument : GlueDocument
    {
        public FunctionTailObjectOrArrayDocument(IEnumerable<Document> documents)
            : base(documents)
        {
        }

        public override int Width { get; } = 1; // Width for "{" on the first line.

        public override IEnumerable<Document> Flatten()
        {
            yield return this;
        }
    }
}
