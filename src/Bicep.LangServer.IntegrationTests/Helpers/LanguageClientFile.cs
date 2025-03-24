// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Text;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Bicep.LangServer.IntegrationTests.Helpers
{
    public class LanguageClientFile
    {
        public LanguageClientFile(DocumentUri uri, string text)
        {
            this.Uri = uri;
            this.Text = text;
            this.LineStarts = TextCoordinateConverter.GetLineStarts(text);
        }

        public DocumentUri Uri { get; }

        public string Text { get; }

        public ImmutableArray<int> LineStarts { get; }

        public Position GetPosition(int offset) => TextCoordinateConverter.GetPosition(this.LineStarts, offset);

        public Range GetRange(TextSpan span) => new()
        {
            Start = GetPosition(span.Position),
            End = GetPosition(span.Position + span.Length),
        };

        public int GetOffset(Position position) => TextCoordinateConverter.GetOffset(this.LineStarts, position.Line, position.Character);

        public TextSpan GetSpan(Range range)
        {
            var start = this.GetOffset(range.Start);
            var end = this.GetOffset(range.End);

            return new TextSpan(start, end - start);
        }
    }
}
