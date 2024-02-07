// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Bicep.Wasm.LanguageHelpers
{
    public class Position(int line, int character)
    {
        public int Line { get; } = line;

        public int Character { get; } = character;
    }
}

