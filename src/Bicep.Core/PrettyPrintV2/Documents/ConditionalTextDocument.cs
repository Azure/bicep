// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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
