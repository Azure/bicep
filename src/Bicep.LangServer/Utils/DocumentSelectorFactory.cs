// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Bicep.Core;

namespace Bicep.LanguageServer.Utils
{
    public class DocumentSelectorFactory
    {
        public static DocumentSelector Create() => DocumentSelector.ForLanguage(
            LanguageConstants.LanguageId);

        public static DocumentSelector CreateForTextDocumentSync() => DocumentSelector.ForLanguage(
            LanguageConstants.LanguageId,
            LanguageConstants.JsonLanguageId,
            LanguageConstants.JsoncLanguageId,
            LanguageConstants.ArmTemplateLanguageId);
    }
}

