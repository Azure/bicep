// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Bicep.Core.Emit
{
    public class PositionTrackingJsonTextWriter : JsonTextWriter
    {
        private class PositionTrackingTextWriter : TextWriter
        {
            private readonly TextWriter _internalWriter;

            public int CurrentPos;

            public List<int> CommaPositions = new();

            public PositionTrackingTextWriter(TextWriter textWriter)
            {
                _internalWriter = textWriter;
            }

            public override Encoding Encoding => _internalWriter.Encoding;

            public override void Write(char value)
            {
                if (value == ',')
                {
                    CommaPositions.Add(CurrentPos);
                }

                _internalWriter.Write(value);

                CurrentPos++;
            }
        }

        public int CurrentPos => _trackingWriter.CurrentPos;
        public IReadOnlyList<int> CommaPositions => _trackingWriter.CommaPositions.AsReadOnly();

        private readonly PositionTrackingTextWriter _trackingWriter;

        public static PositionTrackingJsonTextWriter Create(TextWriter textWriter)
        {
            var trackingWriter = new PositionTrackingTextWriter(textWriter);
            return new PositionTrackingJsonTextWriter(trackingWriter);
        }

        private PositionTrackingJsonTextWriter(PositionTrackingTextWriter textWriter) : base(textWriter)
        {
            _trackingWriter = textWriter;
        }
    }
}
