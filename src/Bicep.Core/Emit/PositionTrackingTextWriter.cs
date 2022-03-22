// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Bicep.Core.Emit
{
    public class PositionTrackingTextWriter : TextWriter
    {

        public int CurrentPos;
        public int CurrentLine;

        private readonly TextWriter _internalWriter;
        private bool _endOfLine;

        public PositionTrackingTextWriter(TextWriter textWriter)
        {
            _internalWriter = textWriter;
        }

        public override Encoding Encoding => _internalWriter.Encoding;

        public override void Write(char value)
        {
            if (_endOfLine)
            {
                CurrentPos = 0;
                CurrentLine++;
            }

            _internalWriter.Write(value);

            CurrentPos++;

            if (value == '\n') // check for carriage return char?
            {
                _endOfLine = true;
            }
        }
    }

    public class PositionTrackingJsonTextWriter : JsonTextWriter
    {
        public int CurrentPos => _trackingWriter.CurrentPos;
        public int CurrentLine => _trackingWriter.CurrentLine;
        
        private PositionTrackingTextWriter _trackingWriter;

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
