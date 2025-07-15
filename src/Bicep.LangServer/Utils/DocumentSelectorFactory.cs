// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Deployments.Core.Extensions;
using Bicep.Core;
using Bicep.LanguageServer.Options;
using Bicep.LanguageServer.Settings;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Utils
{
    public class DocumentSelectorFactory(BicepLangServerOptions langServerOptions)
    {
        private static string Glob(params string[] extensions)
        {
            return "**/*{" + string.Join(',', extensions) + "}";
        }

        public TextDocumentSelector CreateForBicepAndParams() => new(
            langServerOptions?.VsCompatibilityMode == true
              // VS doesn't currently support language filters in the document selector, so we must give it a file pattern
              ? [TextDocumentFilter.ForPattern(Glob(LanguageConstants.LanguageFileExtension, LanguageConstants.ParamsFileExtension))]
              : [TextDocumentFilter.ForLanguage(LanguageConstants.LanguageId), TextDocumentFilter.ForLanguage(LanguageConstants.ParamsLanguageId)]
            );

        public TextDocumentSelector CreateForAllSupportedLangIds() => new(
            langServerOptions?.VsCompatibilityMode == true
              ? [
                  // VS doesn't currently support language filters in the document selector, so we must give it a file pattern
                  TextDocumentFilter.ForPattern(Glob(
                    LanguageConstants.LanguageFileExtension,
                    LanguageConstants.ParamsFileExtension,
                    LanguageConstants.JsoncFileExtension,
                    LanguageConstants.JsonFileExtension,
                    LanguageConstants.ArmTemplateFileExtension))
              ] : [
                    TextDocumentFilter.ForLanguage(LanguageConstants.LanguageId),
                  TextDocumentFilter.ForLanguage(LanguageConstants.ParamsLanguageId),
                  TextDocumentFilter.ForLanguage(LanguageConstants.JsoncLanguageId),
                  TextDocumentFilter.ForLanguage(LanguageConstants.JsonLanguageId),
                  TextDocumentFilter.ForLanguage(LanguageConstants.ArmTemplateLanguageId)
              ]);
    }
}
