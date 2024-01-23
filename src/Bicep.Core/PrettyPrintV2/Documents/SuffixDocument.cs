// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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
