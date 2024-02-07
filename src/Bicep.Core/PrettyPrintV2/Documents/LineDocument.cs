// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.PrettyPrintV2.Documents
{
    public sealed class LineDocument(TextDocument? flattened) : Document
    {
        private readonly TextDocument? flattened = flattened;

        public override IEnumerable<TextDocument> Flatten()
        {
            yield return this.flattened ?? throw new InvalidOperationException("The current line should not be flattened.");
        }
    }
}
