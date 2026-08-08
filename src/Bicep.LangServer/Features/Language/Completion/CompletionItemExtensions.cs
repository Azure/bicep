// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Bicep.LanguageServer.Completions
{
    public static class CompletionItemExtensions
    {
        public static CompletionItem WithDocumentation(this CompletionItem completionItem, string? markdown)
        {
            if (!string.IsNullOrEmpty(markdown))
            {
                return completionItem with
                {
                    Documentation = new StringOrMarkupContent(new MarkupContent
                    {
                        Kind = MarkupKind.Markdown,
                        Value = markdown
                    })
                };
            }

            return completionItem;
        }
    }
}
