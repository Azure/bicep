// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Bicep.Wasm.LanguageHelpers
{
    public class Range
    {
        public Range(Position start, Position end)
        {
            Start = start;
            End = end;
        }

        public Range()
            : this(new Position(0, 0), new Position(0, 0))
        {
        }

        public Position Start { get; set; }

        public Position End { get; set; }
    }
}

