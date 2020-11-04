// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Text;
using Bicep.Core.PrettyPrint.Documents;

namespace Bicep.Core.PrettyPrint.DocumentCombinators
{
    public sealed class NilDocument : ILinkedDocument
    {
        public ILinkedDocument Concat(ILinkedDocument other)
        {
            return other;
        }

        public ILinkedDocument Nest(int level)
        {
            return this;
        }

        public void Layout(StringBuilder builder, string indent, string newline)
        {
            // Do nothing.
        }
    }
}
