// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Bicep.Wasm.LanguageHelpers
{
    public class Range(Position start, Position end)
    {
        public Range()
            : this(new Position(0, 0), new Position(0, 0))
        {
        }

        public Position Start { get; set; } = start;

        public Position End { get; set; } = end;
    }
}

