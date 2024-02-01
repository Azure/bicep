// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core;
using Bicep.Core.FileSystem;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LangServer.IntegrationTests.Helpers
{
    public static class TextDocumentParamHelper
    {
        public static DidOpenTextDocumentParams CreateDidOpenDocumentParams(DocumentUri documentUri, string text, int version) =>
            new()
            {
                TextDocument = new TextDocumentItem
                {
                    LanguageId =
                        PathHelper.HasBicepparamsExtension(documentUri.ToUriEncoded()) ? LanguageConstants.ParamsLanguageId :
                        PathHelper.HasArmTemplateLikeExtension(documentUri.ToUriEncoded()) ? LanguageConstants.ArmTemplateLanguageId :
                        LanguageConstants.LanguageId,
                    Version = version,
                    Uri = documentUri,
                    Text = text,
                },
            };

        public static DidOpenTextDocumentParams CreateDidOpenDocumentParamsFromFile(string filePath, int version) =>
            CreateDidOpenDocumentParams(
                DocumentUri.FromFileSystemPath(filePath),
                File.ReadAllText(filePath),
                version);

        public static DidChangeTextDocumentParams CreateDidChangeTextDocumentParams(DocumentUri documentUri, string text, int version) =>
            new()
            {
                TextDocument = new OptionalVersionedTextDocumentIdentifier
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

        public static DidSaveTextDocumentParams CreateDidSaveTextDocumentParams(DocumentUri documentUri, string text, int version)
        {
            return new DidSaveTextDocumentParams
            {
                TextDocument = new TextDocumentItem
                {
                    LanguageId = LanguageConstants.LanguageId,
                    Version = version,
                    Uri = documentUri,
                    Text = text
                }
            };
        }
    }
}
