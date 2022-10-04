// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Bicep.Core.FileSystem;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.Core.Workspaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Emit
{
    public record RawSourceMap(
        IList<RawSourceMapFileEntry> Entries);

    public record RawSourceMapFileEntry(
        BicepFile SourceFile,
        IList<SourceMapRawEntry> SourceMap);

    public record SourceMapRawEntry(
        TextSpan SourcePosition,
        IList<TextSpan> TargetPositions);

    public class PositionTrackingJsonTextWriter : JsonTextWriter
    {
        private class PositionTrackingTextWriter : TextWriter
        {
            private readonly TextWriter internalWriter = new StringWriter();

            public int CurrentPosition;

            public List<int> CommaPositions = new();

            public PositionTrackingTextWriter(TextWriter textWriter)
            {
                this.internalWriter = textWriter;
            }

            public override Encoding Encoding => this.internalWriter.Encoding;

            public override string? ToString() => internalWriter.ToString();

            public override void Write(char value)
            {
                if (value == ',')
                {
                    CommaPositions.Add(CurrentPosition);
                }

                this.internalWriter.Write(value);

                CurrentPosition++;
            }
        }

        private static readonly Regex JsonWhitespaceStrippingRegex = new(@"(""(?:[^""\\]|\\.)*"")|\s+", RegexOptions.Compiled);

        private readonly IFileResolver fileResolver;
        private readonly RawSourceMap rawSourceMap;
        private readonly BicepFile? sourceFile;
        private readonly PositionTrackingTextWriter trackingWriter;

        public PositionTrackingJsonTextWriter(IFileResolver fileResolver, TextWriter textWriter, BicepFile? sourceFile = null)
            : this(fileResolver, new(textWriter), sourceFile)
        {
        }

        private PositionTrackingJsonTextWriter(IFileResolver fileResolver, PositionTrackingTextWriter trackingWriter, BicepFile? sourceFile)
            : base(trackingWriter)
        {
            this.fileResolver = fileResolver;
            this.sourceFile = sourceFile;
            this.rawSourceMap = new RawSourceMap(new List<RawSourceMapFileEntry>());
            this.trackingWriter = trackingWriter;
        }

        public void WriteExpressionWithPosition(SyntaxBase? sourcePosition, Action expressionFunc)
        {
            var startPos = this.trackingWriter.CurrentPosition;

            expressionFunc();

            AddSourceMapping(sourcePosition, startPos);
        }

        public void WriteObjectWithPosition(SyntaxBase? sourcePosition, Action propertiesFunc)
        {
            var startPos = this.trackingWriter.CurrentPosition;

            base.WriteStartObject();
            propertiesFunc();
            base.WriteEndObject();

            AddSourceMapping(sourcePosition, startPos);
        }

        public void WritePropertyWithPosition(SyntaxBase? keyPosition, string name, Action valueFunc)
        {
            var startPos = this.trackingWriter.CurrentPosition;

            base.WritePropertyName(name);
            valueFunc();

            AddSourceMapping(keyPosition, startPos);
        }

        public void AddNestedSourceMap(PositionTrackingJsonTextWriter nestedTrackingJsonWriter)
        {
            // offset all raw mappings by current position
            var offset = this.trackingWriter.CurrentPosition;
            foreach (var fileEntry in nestedTrackingJsonWriter.rawSourceMap.Entries)
            {
                foreach (var sourceMapEntry in fileEntry.SourceMap)
                {
                    foreach (var position in sourceMapEntry.TargetPositions)
                    {
                        var positionWithOffset = new TextSpan(position.Position + offset, position.Length);

                        AddRawMapping(fileEntry.SourceFile, sourceMapEntry.SourcePosition, positionWithOffset);
                    }
                }
            }
        }

        public override string? ToString() => this.trackingWriter.ToString();

        private void AddSourceMapping(SyntaxBase? bicepSyntax, int jsonStartPosition)
        {
            if (this.sourceFile == null || bicepSyntax == null || bicepSyntax.Span.Length == 0)
            {
                return;
            }

            TextSpan bicepPosition = bicepSyntax.Span;

            // account for leading nodes (decorators)
            if (bicepSyntax is StatementSyntax syntax)
            {
                var lastLeadingNode = syntax.LeadingNodes.LastOrDefault();
                if (lastLeadingNode is not null)
                {
                    bicepPosition = new TextSpan(
                        lastLeadingNode.Span.Position + lastLeadingNode.Span.Length,
                        syntax.Span.Length - lastLeadingNode.Span.Length);
                }
            }

            // increment start position if starting on a comma (happens when outputting successive items in objects and arrays)
            if (this.trackingWriter.CommaPositions.Contains(jsonStartPosition))
            {
                jsonStartPosition++;
            }

            var jsonEndPosition = this.trackingWriter.CurrentPosition - 1;
            var jsonPosition = new TextSpan(jsonStartPosition, jsonEndPosition - jsonStartPosition);

            AddRawMapping(this.sourceFile, bicepPosition, jsonPosition);
        }

        private void AddRawMapping(BicepFile bicepFile, TextSpan bicepPosition, TextSpan jsonPosition)
        {
            var fileEntry = this.rawSourceMap.Entries.FirstOrDefault(entry => entry.SourceFile.Equals(bicepFile));
            if (fileEntry is null)
            {
                fileEntry = new RawSourceMapFileEntry(bicepFile, new List<SourceMapRawEntry>());

                this.rawSourceMap.Entries.Add(fileEntry);
            }

            var sourceMapEntry = fileEntry.SourceMap.FirstOrDefault(i => i.SourcePosition == bicepPosition);
            if (sourceMapEntry is null)
            {
                sourceMapEntry = new SourceMapRawEntry(bicepPosition, new List<TextSpan>());

                fileEntry.SourceMap.Add(sourceMapEntry);
            }

            sourceMapEntry.TargetPositions.Add(jsonPosition);
        }

        public SourceMap? ProcessRawSourceMap(JToken templateWithHash)
        {
            if (this.sourceFile == null || this.rawSourceMap == null)
            {
                return null;
            }

            var formattedTemplateLines = templateWithHash
                .ToString(Formatting.Indented)
                .Split(Environment.NewLine, StringSplitOptions.None);

            // get line starts of unformatted JSON by stripping formatting from each line of formatted JSON
            var unformattedLineStarts = formattedTemplateLines
                .Aggregate(
                    new List<int>() { 0 }, // first line starts at position 0
                    (lineStarts, line) =>
                    {
                        var unformattedLine = JsonWhitespaceStrippingRegex.Replace(line, "$1");
                        lineStarts.Add(lineStarts.Last() + unformattedLine.Length);
                        return lineStarts;
                    });

            // get position and length of template hash (relying on the first occurence)
            (var templateHashStartPosition, var templateHashLength) = formattedTemplateLines
                .Select((value, index) => new { lineNumber = index, lineValue = value })
                .Where(item => item.lineValue.Contains(TemplateWriter.TemplateHashPropertyName))
                .Select(item =>
                {
                    var startPosition = unformattedLineStarts[item.lineNumber];
                    var unformattedLineLength = unformattedLineStarts[item.lineNumber + 1] - startPosition;
                    return (startPosition, unformattedLineLength + 1); // account for comma by adding 1 to length
                })
                .FirstOrDefault();


            var weights = new int[unformattedLineStarts.Count];
            Array.Fill(weights, int.MaxValue);

            var sourceMapFileEntries = new List<SourceMapFileEntry>();
            var entrypointFileName = System.IO.Path.GetFileName(sourceFile.FileUri.AbsolutePath);
            var entrypointAbsolutePath = System.IO.Path.GetDirectoryName(sourceFile.FileUri.AbsolutePath)!;

            foreach (var bicepFileEntry in this.rawSourceMap.Entries)
            {
                var bicepAbsolutePath = bicepFileEntry.SourceFile.FileUri.AbsolutePath;
                var bicepRelativeFilePath = fileResolver.GetRelativePath(entrypointAbsolutePath, bicepAbsolutePath);
                var sourceMapEntries = new List<SourceMapEntry>();

                foreach (var sourceMapEntry in bicepFileEntry.SourceMap)
                {
                    var bicepLine = TextCoordinateConverter.GetPosition(
                        bicepFileEntry.SourceFile.LineStarts, sourceMapEntry.SourcePosition.Position).line;

                    for (int i = 0; i < sourceMapEntry.TargetPositions.Count; i++)
                    {
                        var jsonPosition = sourceMapEntry.TargetPositions[i];

                        // increment positions by templateHashLength that occur after hash start position
                        if (jsonPosition.Position >= templateHashStartPosition)
                        {
                            jsonPosition = new TextSpan(jsonPosition.Position + templateHashLength, jsonPosition.Length);
                            sourceMapEntry.TargetPositions[i] = jsonPosition; // update RawSourceMap in place
                        }

                        var jsonStartPos = jsonPosition.Position;
                        var jsonEndPos = jsonStartPos + jsonPosition.Length;

                        // transform offsets in rawSourceMap to line numbers for formatted JSON using unformattedLineStarts
                        var jsonStartLine = TextCoordinateConverter.GetPosition(unformattedLineStarts, jsonStartPos).line;
                        var jsonEndLine = TextCoordinateConverter.GetPosition(unformattedLineStarts, jsonEndPos).line;

                        // write most specific mapping available for each json line (less lines => stronger weight)
                        int weight = jsonEndLine - jsonStartLine;
                        for (int jsonLine = jsonStartLine; jsonLine <= jsonEndLine; jsonLine++)
                        {
                            // write new mapping if weight is stronger than existing mapping
                            if (weight < weights[jsonLine])
                            {
                                sourceMapEntries.RemoveAll(i => i.TargetLine == jsonLine);
                                sourceMapEntries.Add(new SourceMapEntry(bicepLine, jsonLine));
                                weights[jsonLine] = weight;
                            }
                        }
                    }
                }

                var fileEntry = new SourceMapFileEntry(
                    bicepRelativeFilePath,
                    sourceMapEntries.ToImmutableArray());
                sourceMapFileEntries.Add(fileEntry);
            }

            return new SourceMap(
               entrypointFileName,
               sourceMapFileEntries.ToImmutableArray());
        }
    }
}
