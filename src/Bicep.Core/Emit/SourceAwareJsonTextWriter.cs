// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.IO;
using Bicep.Core.FileSystem;
using Bicep.Core.Workspaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Emit
{
    public record SourceMap(
        string Entrypoint,
        ImmutableArray<SourceMapFileEntry> Entries);

    public record SourceMapFileEntry(
        string FilePath,
        ImmutableArray<SourceMapEntry> SourceMap);

    public record SourceMapEntry(
        int SourceLine,
        int TargetLine);

    public class SourceAwareJsonTextWriter : JsonTextWriter
    {
        private readonly BicepFile? sourceFile;

        public SourceMap? SourceMap { get; private set; }
        public readonly PositionTrackingJsonTextWriter TrackingJsonWriter;

        /// <summary>
        /// Creates a JsonTextWriter that is capable of generated a source map for the compiled JSON
        /// </summary>
        /// <param name="fileResolver"></param>
        /// <param name="textWriter"></param>
        /// <param name="sourceFileToTrack">If set to default, source mapping is disabled</param>
        public SourceAwareJsonTextWriter(IFileResolver fileResolver, TextWriter textWriter, BicepFile? sourceFileToTrack = default) : base(textWriter)
        {
            this.sourceFile = sourceFileToTrack;
            this.TrackingJsonWriter = new PositionTrackingJsonTextWriter(fileResolver, new StringWriter(), this.sourceFile);
        }

        public void ProcessSourceMap(JToken templateWithHash)
        {
            this.SourceMap = this.TrackingJsonWriter.ProcessRawSourceMap(templateWithHash);
        }
    }
}
