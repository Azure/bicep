// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.LanguageServer.Client;
using Microsoft.VisualStudio.LanguageServer.Protocol;
using Newtonsoft.Json.Linq;

namespace Bicep.VSLanguageServerClient.MiddleLayerProviders
{
    public class UpdateFormatSettingsMiddleLayer : ILanguageClientMiddleLayer
    {
        public bool CanHandle(string methodName)
        {
            if (string.IsNullOrEmpty(methodName))
            {
                return false;
            }

            return methodName.Equals(Methods.TextDocumentFormattingName, StringComparison.Ordinal);
        }

        public async Task HandleNotificationAsync(string methodName, JToken methodParam, Func<JToken, Task> sendNotification)
        {
            await sendNotification(methodParam);
        }

        public async Task<JToken?> HandleRequestAsync(string methodName, JToken methodParam, Func<JToken, Task<JToken?>> sendRequest)
        {
            if (CanHandle(methodName))
            {
                var documentFormattingParams = methodParam.ToObject<DocumentFormattingParams>();

                if (documentFormattingParams is not null)
                {
                    var formattingOptions = documentFormattingParams.Options;
                    formattingOptions.InsertSpaces = true;
                    formattingOptions.TabSize = 2;
                    documentFormattingParams.Options = formattingOptions;
                    methodParam = JToken.FromObject(documentFormattingParams);
                }
            }

            return await sendRequest(methodParam);
        }
    }
}
