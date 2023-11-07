// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Utils
{
    public class DocumentSelectorFactory
    {
        public static TextDocumentSelector CreateForBicepAndParams() => TextDocumentSelector.ForLanguage(
            LanguageConstants.LanguageId,
            LanguageConstants.ParamsLanguageId);

        public static TextDocumentSelector CreateForTextDocumentSync() => TextDocumentSelector.ForLanguage(
            LanguageConstants.LanguageId,
            LanguageConstants.ParamsLanguageId,
            LanguageConstants.JsoncLanguageId,
            LanguageConstants.JsonLanguageId,
            LanguageConstants.ArmTemplateLanguageId);
    }
}

