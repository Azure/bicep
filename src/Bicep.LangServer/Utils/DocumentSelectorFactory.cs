// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Utils
{
    public class DocumentSelectorFactory
    {
        public static DocumentSelector CreateForBicepAndParams() => DocumentSelector.ForLanguage(
            LanguageConstants.LanguageId,
            LanguageConstants.ParamsLanguageId);

        public static DocumentSelector CreateForTextDocumentSync() => DocumentSelector.ForLanguage(
            LanguageConstants.LanguageId,
            LanguageConstants.ParamsLanguageId,
            LanguageConstants.JsoncLanguageId,
            LanguageConstants.JsonLanguageId,
            LanguageConstants.ArmTemplateLanguageId);
    }
}

