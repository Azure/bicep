// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Bicep.Core.Syntax;
using Bicep.Core.Workspaces;
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

            public override Encoding Encoding => this.internalWriter.Encoding;

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

        private readonly RawSourceMap? rawSourceMap;
        private readonly BicepFile? sourceFile;
        private readonly PositionTrackingTextWriter trackingWriter;

        public int CurrentPos => this.trackingWriter.CurrentPos;
        public ImmutableArray<int> CommaPositions => this.trackingWriter.CommaPositions.ToImmutableArray();

        private PositionTrackingJsonTextWriter(PositionTrackingTextWriter textWriter,
            RawSourceMap? rawSourceMap, BicepFile? sourceFile) : base(textWriter)
        {
            this.rawSourceMap = rawSourceMap;
            this.sourceFile = sourceFile;
            this.trackingWriter = textWriter;
        }

        public static PositionTrackingJsonTextWriter Create(TextWriter textWriter,
            RawSourceMap? rawSourceMap = null, BicepFile? sourceFile = null)
        {
            var trackingWriter = new PositionTrackingTextWriter(textWriter);
            return new PositionTrackingJsonTextWriter(trackingWriter, rawSourceMap, sourceFile);
        }

        public void WriteProperty(SyntaxBase? keyPosition, string name, Action valueFunc)
        {
            var startPos = this.trackingWriter.CurrentPos;

            base.WritePropertyName(name);
            valueFunc();

            AddSourceMapping(keyPosition, startPos);
        }

        public void AddSourceMapping(SyntaxBase? bicepSyntax, int startPosition)
        {
            if (bicepSyntax != null && this.rawSourceMap != null && this.sourceFile != null)
            {
                SourceMapHelper.AddMapping(
                    this.rawSourceMap,
                    this.sourceFile,
                    bicepSyntax,
                    this,
                    startPosition);
            }
        }
    }
}
