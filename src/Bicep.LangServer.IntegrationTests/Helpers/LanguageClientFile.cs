// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Parsing;
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

        public TextDocumentIdentifier DocumentIdentifier => new(this.Uri);

        public string Text { get; }

        ImmutableArray<int> LineStarts { get; }

        public Position GetPosition(int offset) => TextCoordinateConverter.GetPosition(this.LineStarts, offset);

        public Range GetRange(TextSpan span) => new()
        {
            Start = GetPosition(span.Position),
            End = GetPosition(span.Position + span.Length),
        };
    }
}
