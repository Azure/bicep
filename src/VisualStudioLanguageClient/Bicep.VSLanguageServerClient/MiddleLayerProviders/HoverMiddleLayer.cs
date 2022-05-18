// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.LanguageServer.Client;
using Microsoft.VisualStudio.LanguageServer.Protocol;
using Newtonsoft.Json.Linq;
using Hover = OmniSharp.Extensions.LanguageServer.Protocol.Models.Hover;
using MarkupContent = OmniSharp.Extensions.LanguageServer.Protocol.Models.MarkupContent;
using VSHover = Microsoft.VisualStudio.LanguageServer.Protocol.Hover;
using VSMarkupContent = Microsoft.VisualStudio.LanguageServer.Protocol.MarkupContent;

namespace Bicep.VSLanguageServerClient.MiddleLayerProviders
{
    internal class HoverMiddleLayer : ILanguageClientMiddleLayer
    {
        public bool CanHandle(string methodName)
        {
            if (string.IsNullOrEmpty(methodName))
            {
                return false;
            }

            return methodName.Equals(Methods.TextDocumentHoverName, StringComparison.Ordinal);
        }

        public async Task HandleNotificationAsync(string methodName, JToken methodParam, Func<JToken, Task> sendNotification)
        {
            await sendNotification(methodParam);
        }

        public async Task<JToken?> HandleRequestAsync(string methodName, JToken methodParam, Func<JToken, Task<JToken?>> sendRequest)
        {
            JToken? jToken = await sendRequest(methodParam);

            if (jToken != null && CanHandle(methodName))
            {
                return HandleHoverRequest(jToken);
            }

            return jToken;
        }

        internal JToken? HandleHoverRequest(JToken jToken)
        {
            if (jToken.ToObject<Hover>() is not Hover hover)
            {
                return null;
            }
            var range = hover.Range;

            if (range is not null)
            {
                var vsHoverRange = new Range()
                {
                    Start = new Position(range.Start.Line, range.Start.Character),
                    End = new Position(range.End.Line, range.End.Character)
                };
                var contents = hover.Contents;

                if (contents.HasMarkupContent &&
                    contents.MarkupContent is MarkupContent markupContent &&
                    markupContent is not null)
                {
                    var vsHover = new VSHover()
                    {
                        Range = vsHoverRange,
                        Contents = new VSMarkupContent()
                        {
                            Kind = MarkupKind.Markdown,
                            Value = markupContent.Value
                        }
                    };

                    return JToken.FromObject(vsHover);
                }
            }

            return jToken;
        }

        protected static bool IsSumTypeValueNullOrWhiteSpace(SumType<string, MarkedString> sumType)
        {
            switch (sumType.Value)
            {
                case MarkedString markedString:
                    return markedString.Value is string markedStringValue &&
                        string.IsNullOrWhiteSpace(markedStringValue);
                case string descriptionText:
                    return string.IsNullOrWhiteSpace(descriptionText);
                default:
                    return false;
            }
        }
    }
}
