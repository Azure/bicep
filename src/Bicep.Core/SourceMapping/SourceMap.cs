// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;

namespace Bicep.Core.SourceMapping
{

    public class LineRange
    {
        public int StartLine { get; }
        public int EndLine { get; }

        public LineRange(int startLine, int endLine)
        {
            this.StartLine = startLine;
            this.EndLine = endLine;
        }

        public bool IsInRange(int line)
        {
            return (line <= this.EndLine && line >= this.StartLine);
        }

    }

    public class SourceMap
    {

        private Dictionary<LineRange, int> mappings;

        public SourceMap()
        {
            this.mappings = new Dictionary<LineRange, int>();
        }
        
        public void AddMapping(int generatedStartLine, int generatedEndLine, int sourceLineNumber)
        {
            mappings.TryAdd(new LineRange(generatedStartLine, generatedEndLine), sourceLineNumber);
        }

        public int GetBicepLineNumber(int jsonLine)
        {
            foreach (var mapping in this.mappings)
            {
                if (mapping.Key.IsInRange(jsonLine))
                {
                    return mapping.Value;
                }
            }
            return 0;
        }

    }    
}
