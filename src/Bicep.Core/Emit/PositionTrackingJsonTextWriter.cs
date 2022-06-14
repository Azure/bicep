// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Bicep.Core.Emit
{
    public class PositionTrackingJsonTextWriter : JsonTextWriter
    {
        private class PositionTrackingTextWriter : TextWriter
        {
            private readonly TextWriter internalWriter;

            public int CurrentPos;

            public List<int> CommaPositions = new();

            public PositionTrackingTextWriter(TextWriter textWriter)
            {
                this.internalWriter = textWriter;
            }

            public override Encoding Encoding => this.internalWriter.Encoding;

            public override void Write(char value)
            {
                if (value == ',')
                {
                    CommaPositions.Add(CurrentPos);
                }

                this.internalWriter.Write(value);

                CurrentPos++;
            }
        }

        private readonly PositionTrackingTextWriter _trackingWriter;

        public int CurrentPos => _trackingWriter.CurrentPos;
        public ImmutableArray<int> CommaPositions => _trackingWriter.CommaPositions.ToImmutableArray();

        private PositionTrackingJsonTextWriter(PositionTrackingTextWriter textWriter) : base(textWriter)
        {
            _trackingWriter = textWriter;
        }

        public static PositionTrackingJsonTextWriter Create(TextWriter textWriter)
        {
            var trackingWriter = new PositionTrackingTextWriter(textWriter);
            return new PositionTrackingJsonTextWriter(trackingWriter);
        }
    }
}
