// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Parsing;

namespace Bicep.Core.CodeAction
{
    public static class CodeManipulator
    {
        public static CodeReplacement Replace(TextSpan span, string text)
            => new CodeReplacement(span, text);

        public static CodeReplacement Replace(IPositionable positionable, string text)
            => Replace(positionable.Span, text);
    }
}
