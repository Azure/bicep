// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Bicep.Core.Emit
{
    {


        private class PositionTrackingTextWriter : TextWriter
        {
            private readonly TextWriter internalWriter;

        public int CurrentPos;
        public int CurrentLine;


        public PositionTrackingTextWriter(TextWriter textWriter)
        {
        }


        public override void Write(char value)
        {
            {
            }


            CurrentPos++;

            if (value == '\n') // check for carriage return char?
            {
                _endOfLine = true;
            }
        }
    }

        public int CurrentPos => _trackingWriter.CurrentPos;
        

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
