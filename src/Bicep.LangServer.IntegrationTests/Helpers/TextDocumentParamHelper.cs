﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LangServer.IntegrationTests.Helpers
{
    public static class TextDocumentParamHelper
    {
        public static DidOpenTextDocumentParams CreateDidOpenDocumentParams(DocumentUri documentUri, string text, int version) =>
            new DidOpenTextDocumentParams
            {
                TextDocument = new TextDocumentItem
                {
                    LanguageId = "bicep",
                    Version = version,
                    Uri = documentUri,
                    Text = text,
                },
            };

        public static DidChangeTextDocumentParams CreateDidChangeTextDocumentParams(DocumentUri documentUri, string text, int version) =>
            new DidChangeTextDocumentParams
            {
                TextDocument = new VersionedTextDocumentIdentifier
                {
                    Version = version,
                    Uri = documentUri
                },
                ContentChanges = new Container<TextDocumentContentChangeEvent>(
                    // for now we're sending the full document every time, nothing fancy..
                    new TextDocumentContentChangeEvent
                    {
                        Text = text
                    }
                )
            };

        public static DidCloseTextDocumentParams CreateDidCloseTextDocumentParams(DocumentUri documentUri, int version)
        {
            return new DidCloseTextDocumentParams
            {
                TextDocument = new VersionedTextDocumentIdentifier
                {
                    Version = version,
                    Uri = documentUri
                }
            };
        }
    }
}
