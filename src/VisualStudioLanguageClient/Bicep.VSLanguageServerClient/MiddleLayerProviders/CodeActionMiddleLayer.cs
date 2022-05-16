// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.LanguageServer.Client;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Methods = Microsoft.VisualStudio.LanguageServer.Protocol.Methods;

namespace Bicep.VSLanguageServerClient.MiddleLayerProviders
{
    internal class CodeActionMiddleLayer : ILanguageClientMiddleLayer
    {
        public bool CanHandle(string methodName)
        {
            if (string.IsNullOrEmpty(methodName))
            {
                return false;
            }

            return methodName.Equals(Methods.TextDocumentCodeActionName, StringComparison.Ordinal);
        }

        public async Task<JToken?> HandleRequestAsync(string methodName, JToken methodParam, Func<JToken, Task<JToken?>> sendRequest)
        {
            if (CanHandle(methodName))
            {
                JToken? jToken = await sendRequest(methodParam);

                List<CommandOrCodeAction> commandOrCodeActions = new List<CommandOrCodeAction>();

                if (jToken is not null)
                {
                    foreach (var child in jToken.Children())
                    {
                        CodeAction? codeAction = child.ToObject<CodeAction>();
                        if (codeAction is not null && !codeAction.Title.Contains("bicepconfig.json"))
                        {
                            CommandOrCodeAction commandOrCodeAction = new CommandOrCodeAction(codeAction);
                        commandOrCodeActions.Add(commandOrCodeAction);
                        }
                    }

                    CommandOrCodeActionContainer commandOrCodeActionContainer = new CommandOrCodeActionContainer(commandOrCodeActions);
                    return JToken.FromObject(commandOrCodeActionContainer);
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
