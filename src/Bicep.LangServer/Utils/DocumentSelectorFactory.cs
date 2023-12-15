// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Linq;
using Bicep.Core;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Utils
{
    public class DocumentSelectorFactory
    {
        public static TextDocumentSelector CreateForBicepAndParams() => TextDocumentSelector.ForLanguage(
            LanguageConstants.LanguageId,
            LanguageConstants.ParamsLanguageId);

        public static TextDocumentSelector CreateForTextDocumentSync() =>
            new(
                TextDocumentFilter.ForLanguage(LanguageConstants.LanguageId),
                TextDocumentFilter.ForLanguage(LanguageConstants.ParamsLanguageId),
                TextDocumentFilter.ForLanguage(LanguageConstants.JsoncLanguageId),
                TextDocumentFilter.ForLanguage(LanguageConstants.JsonLanguageId), //asdfg
                TextDocumentFilter.ForLanguage(LanguageConstants.ArmTemplateLanguageId),
                TextDocumentFilter.ForScheme("bicep-extsrc") //asdfg magic
            );
    }
}

