// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.LanguageServer.Client;
using Microsoft.VisualStudio.LanguageServer.Protocol;
using Newtonsoft.Json.Linq;

namespace Bicep.VSLanguageServerClient.MiddleLayerProviders
{
    public class InterceptSnippetCompletionMiddleLayer : ILanguageClientMiddleLayer
    {
        public bool CanHandle(string methodName)
        {
            if (string.IsNullOrEmpty(methodName))
            {
                return false;
            }

            return methodName.Equals(Methods.TextDocumentCompletionName, StringComparison.Ordinal);
        }

        public async Task<JToken?> HandleRequestAsync(string methodName, JToken methodParam, Func<JToken, Task<JToken?>> sendRequest)
        {
            if (CanHandle(methodName))
            {
                JToken? jToken = await sendRequest(methodParam);

                if (jToken is not null)
                {
                    //CompletionList updatedCompletionList = new CompletionList();
                    //List<CompletionItem> updatedCompletions = new List<CompletionItem>();

                    //foreach(var child in jToken.Children())
                    //{
                    //    JToken command = child["command"];

                    //    if (command is not null)
                    //    {
                    //        command["title"] = string.Empty;
                    //       // command["command"].Rename("CommandIdentifier");
                    //    }
                    //    CompletionItem? completionItem = child.ToObject<CompletionItem>();

                    //    if (completionItem is not null)
                    //    {
                    //        updatedCompletions.Add(completionItem);
                    //    }
                    //}

                    //updatedCompletionList.Items = updatedCompletions.ToArray();
                    //return JToken.FromObject(updatedCompletionList);
                }
            }

            return await sendRequest(methodParam);
        }

        public async Task HandleNotificationAsync(string methodName, JToken methodParam, Func<JToken, Task> sendNotification)
        {
            await sendNotification(methodParam);
        }
    }

}
