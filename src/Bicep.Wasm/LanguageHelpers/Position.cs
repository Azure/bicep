// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Bicep.Wasm.LanguageHelpers
{
    public class Position
    {
        public Position(int line, int character)
        {
            Line = line;
            Character = character;
        }

        public int Line { get; }

        public int Character { get; }
    }
}

