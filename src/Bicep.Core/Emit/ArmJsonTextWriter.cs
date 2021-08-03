// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO;
using Newtonsoft.Json;

namespace Bicep.Core.Emit
{
    public class ArmJsonTextWriter : JsonTextWriter
    {
        public int CurrentLine;

        public ArmJsonTextWriter(TextWriter textWriter) : base(textWriter)
        {
            this.CurrentLine = 1;
        }

        public override void WriteStartObject()
        {
            base.WriteStartObject();
            CurrentLine++;
        }

        public override void WriteEndObject()
        {
            base.WriteEndObject();
            CurrentLine++;
        }

        public override void WriteStartArray()
        {
            base.WriteStartArray();
            CurrentLine++;
        }

        public override void WriteEndArray()
        {
            base.WriteEndArray();
            CurrentLine++;
        }

        public override void WriteValue(bool v)
        {
            base.WriteValue(v);
            CurrentLine++;
        }

        public override void WriteValue(long v)
        {
            base.WriteValue(v);
            CurrentLine++;
        }
        
        public override void WriteValue(string? v)
        {
            base.WriteValue(v);
            CurrentLine++;
        }
        
    }
}
