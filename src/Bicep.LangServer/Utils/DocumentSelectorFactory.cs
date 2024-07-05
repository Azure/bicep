// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Utils
{
    public class DocumentSelectorFactory
    {
        public static readonly TextDocumentFilter[] BicepAndParams = [
            TextDocumentFilter.ForLanguage(LanguageConstants.LanguageId),
            TextDocumentFilter.ForLanguage(LanguageConstants.ParamsLanguageId)
        ];

        public static readonly TextDocumentFilter[] AllSupportedLangIds = [
                TextDocumentFilter.ForLanguage(LanguageConstants.LanguageId),
            TextDocumentFilter.ForLanguage(LanguageConstants.ParamsLanguageId),
            TextDocumentFilter.ForLanguage(LanguageConstants.JsoncLanguageId),
            TextDocumentFilter.ForLanguage(LanguageConstants.JsonLanguageId),
            TextDocumentFilter.ForLanguage(LanguageConstants.ArmTemplateLanguageId)
        ];

        public static TextDocumentSelector CreateForBicepAndParams() => new(BicepAndParams);
        public static TextDocumentSelector CreateForAllSupportedLangIds() => new(AllSupportedLangIds);
    }
}

