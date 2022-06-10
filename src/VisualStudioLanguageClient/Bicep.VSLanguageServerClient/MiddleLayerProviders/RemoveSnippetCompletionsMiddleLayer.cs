// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.LanguageServer.Client;
using Microsoft.VisualStudio.LanguageServer.Protocol;
using Newtonsoft.Json.Linq;

namespace Bicep.VSLanguageServerClient.MiddleLayerProviders
{
    public class RemoveSnippetCompletionsMiddleLayer : ILanguageClientMiddleLayer
    {
        public bool CanHandle(string methodName)
        {
            if (string.IsNullOrEmpty(methodName))
            {
                return false;
            }

            return methodName.Equals(Methods.TextDocumentCompletionName, StringComparison.Ordinal);
        }

        public async Task HandleNotificationAsync(string methodName, JToken methodParam, Func<JToken, Task> sendNotification)
        {
            await sendNotification(methodParam);
        }

        public async Task<JToken?> HandleRequestAsync(string methodName, JToken methodParam, Func<JToken, Task<JToken?>> sendRequest)
        {
            if (CanHandle(methodName))
            {
                JToken? jToken = await sendRequest(methodParam);
                List<CompletionItem> updatedCompletions = new List<CompletionItem>();

                if (jToken is not null)
                {
                    var propSnippet = new CompletionItem()
                    {
                        Label = "prop",
                        InsertText = "public ${1:int} ${2:MyProperty} { get; set; }$0",
                        InsertTextFormat = InsertTextFormat.Snippet,
                        Kind = CompletionItemKind.Snippet,
                    };
                    updatedCompletions.Add(propSnippet);
                    //foreach (var child in jToken.Children())
                    //{
                    //    var completionItem = child.ToObject<CompletionItem>();
                    //    if (completionItem is not null &&
                    //        completionItem.InsertTextFormat == InsertTextFormat.Snippet)
                    //    {
                    //        completionItem.InsertText = completionItem.TextEdit?.NewText;
                    //        completionItem.Kind = CompletionItemKind.Snippet;
                    //        updatedCompletions.Add(completionItem);
                    //    }
                    //}

                    return JToken.FromObject(updatedCompletions);
                }
            }

            return await sendRequest(methodParam);
        }
    }
}
