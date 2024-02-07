// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Bicep.Wasm.LanguageHelpers
{
    public class SemanticToken(int line, int character, int length, SemanticTokenType tokenType)
    {
        public int Line { get; set; } = line;

        public int Character { get; set; } = character;

        public int Length { get; set; } = length;

        public SemanticTokenType TokenType { get; set; } = tokenType;
    }
}

