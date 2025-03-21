// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.SourceGraph;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Emit
{
    public record SourceMap(string Entrypoint, ImmutableArray<SourceMapFileEntry> Entries);

    public record SourceMapFileEntry(string FilePath, ImmutableArray<SourceMapEntry> SourceMap);

    public readonly record struct SourceMapEntry(int SourceLine, int TargetLine);

    public class SourceAwareJsonTextWriter : JsonTextWriter
    {
        private readonly BicepSourceFile? sourceFile;

        public SourceMap? SourceMap { get; private set; }
        public readonly PositionTrackingJsonTextWriter TrackingJsonWriter;

        /// <summary>
        /// Creates a JsonTextWriter that is capable of generating a source map for the compiled JSON
        /// </summary>
        /// <param name="textWriter"></param>
        /// <param name="sourceFileToTrack">If set to default, source mapping is disabled</param>
        public SourceAwareJsonTextWriter(TextWriter textWriter, BicepSourceFile? sourceFileToTrack = default) : base(textWriter)
        {
            this.sourceFile = sourceFileToTrack;
            this.TrackingJsonWriter = new PositionTrackingJsonTextWriter(new StringWriter() { NewLine = "\n" }, this.sourceFile);
        }

        public void ProcessSourceMap(JToken templateWithHash)
        {
            this.SourceMap = this.TrackingJsonWriter.ProcessRawSourceMap(templateWithHash);
        }
    }
}
