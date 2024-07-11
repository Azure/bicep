// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Utils
{
    public class DocumentSelectorFactory
    {
        // VS doesn't currently support language filters in the document selector, so we must give it a file pattern
        private static string Glob(params string[] extensions)
        {
            return "**/*{" + string.Join(',', extensions) + "}"; 
        }

        public static readonly TextDocumentFilter[] BicepAndParams = [
            TextDocumentFilter.ForLanguage(LanguageConstants.LanguageId),
            TextDocumentFilter.ForLanguage(LanguageConstants.ParamsLanguageId),
            TextDocumentFilter.ForPattern(Glob(LanguageConstants.LanguageFileExtension, LanguageConstants.ParamsFileExtension))
        ];

        public static readonly TextDocumentFilter[] AllSupportedLangIds = [
            TextDocumentFilter.ForLanguage(LanguageConstants.LanguageId),
            TextDocumentFilter.ForLanguage(LanguageConstants.ParamsLanguageId),
            TextDocumentFilter.ForLanguage(LanguageConstants.JsoncLanguageId),
            TextDocumentFilter.ForLanguage(LanguageConstants.JsonLanguageId),
            TextDocumentFilter.ForLanguage(LanguageConstants.ArmTemplateLanguageId),
            TextDocumentFilter.ForPattern(Glob(
                LanguageConstants.LanguageFileExtension,
                LanguageConstants.ParamsFileExtension,
                LanguageConstants.JsoncFileExtension,
                LanguageConstants.JsonFileExtension,
                LanguageConstants.ArmTemplateFileExtension))
        ];

        public static TextDocumentSelector CreateForBicepAndParams() => new(BicepAndParams);
        public static TextDocumentSelector CreateForAllSupportedLangIds() => new(AllSupportedLangIds);
    }
}

