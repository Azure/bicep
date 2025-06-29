// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Text;

namespace Bicep.Core.CodeAction
{
    public readonly record struct CodeReplacement(TextSpan Span, string Text) : IPositionable
    {
        public static readonly CodeReplacement Nil = new(TextSpan.Nil, "");

        public bool IsNil => this == Nil;
    }
}
