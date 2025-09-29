// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Highlighting;

namespace Bicep.Wasm.LanguageHelpers
{
    public class TokenPosition
    {
        public TokenPosition(int line, int character, int length, SemanticTokenType tokenType)
        {
            Line = line;
            Character = character;
            Length = length;
            TokenType = tokenType;
        }

        public int Line { get; set; }

        public int Character { get; set; }

        public int Length { get; set; }

        public SemanticTokenType TokenType { get; set; }
    }
}

