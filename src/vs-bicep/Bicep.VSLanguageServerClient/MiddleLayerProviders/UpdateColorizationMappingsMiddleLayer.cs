// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Microsoft.VisualStudio.LanguageServer.Client;
using Microsoft.VisualStudio.LanguageServer.Protocol;
using Microsoft.VisualStudio.Shell;
using Newtonsoft.Json.Linq;

namespace Bicep.VSLanguageServerClient.MiddleLayerProviders
{
    [Export(typeof(HandleSnippetCompletionsMiddleLayer))]
    public class UpdateColorizationMappingsMiddleLayer : ILanguageClientMiddleLayer
    {
        public bool CanHandle(string methodName)
        {
            if (string.IsNullOrEmpty(methodName))
            {
                return false;
            }

            return methodName.Equals(Methods.TextDocumentSemanticTokensFullName, StringComparison.Ordinal) ||
                 methodName.Equals(Methods.TextDocumentSemanticTokensFullDeltaName, StringComparison.Ordinal);
        }

        public async Task HandleNotificationAsync(string methodName, JToken methodParam, Func<JToken, Task> sendNotification)
        {
            await sendNotification(methodParam);
        }

        public async Task<JToken?> HandleRequestAsync(string methodName, JToken methodParam, Func<JToken, Task<JToken?>> sendRequest)
        {
            if (CanHandle(methodName))
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                JToken? jToken = await sendRequest(methodParam);

                if (jToken is not null)
                {
                    var semanticTokens = jToken.ToObject<SemanticTokens>();

                    
                }
            }

            return await sendRequest(methodParam);
        }
    }
}
