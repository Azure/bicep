// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.PrettyPrintV2.Documents
{
    public sealed class SuffixDocument(string value) : TextDocument(value)
    {
        public override int Width => 0;
    }
}
