// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.PrettyPrintV2.Documents
{
    public sealed class ConditionalTextDocument(string onBreak, string onFlatten) : TextDocument(onBreak)
    {
        private readonly TextDocument onFlatten = onFlatten;

        public override IEnumerable<TextDocument> Flatten()
        {
            yield return this.onFlatten;
        }
    }
}
