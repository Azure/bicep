// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Text;

namespace Bicep.Core.PrettyPrint.Documents
{
    public interface ILinkedDocument
    {
        ILinkedDocument Concat(ILinkedDocument other);

        ILinkedDocument Nest(int level);

        void Layout(StringBuilder sb, string indent, string newline);
    }
}
