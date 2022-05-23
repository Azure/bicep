// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Bicep.Core.Emit
{
    public class PositionTrackingJsonTextWriter : JsonTextWriter
    {
        public int CurrentPos;

            public List<int> CommaPositions = new();

        public string _debugString = string.Empty;
        public List<int> CommaPositions = new List<int>();

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
            _debugString += value;

                CurrentPos++;
            }
        }

        public int CurrentPos => _trackingWriter.CurrentPos;
        public List<int> CommaPositions => _trackingWriter.CommaPositions;

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
