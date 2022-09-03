// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Bicep.VSLanguageServerClient.Settings;
using Microsoft.VisualStudio.LanguageServer.Client;
using Microsoft.VisualStudio.LanguageServer.Protocol;
using Newtonsoft.Json.Linq;

namespace Bicep.VSLanguageServerClient.MiddleLayerProviders
{
    /// <summary>
    /// This middle layer sets the default vscode format settings.
    /// This can be removed once settings and .editorconfig support are added in vs layer - https://github.com/Azure/bicep/issues/7550
    /// </summary>
    public class UpdateFormatSettingsMiddleLayer : ILanguageClientMiddleLayer
    {
        private readonly IBicepSettings bicepSettings;

        public UpdateFormatSettingsMiddleLayer(IBicepSettings bicepSettings)
        {
            this.bicepSettings = bicepSettings;
        }

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
                methodParam = await UpdateFormatOptionsAsync(methodParam);
            }

            return await sendRequest(methodParam);
        }

        public async Task<JToken> UpdateFormatOptionsAsync(JToken methodParam)
        {
            var documentFormattingParams = methodParam.ToObject<DocumentFormattingParams>();

            if (documentFormattingParams is not null)
            {
                var formattingOptions = documentFormattingParams.Options;

                var formatterIndentTypeKey = await bicepSettings.GetIntegerAsync(BicepLanguageServerClientConstants.FormatterIndentTypeKey, (int)IndentType.Spaces);

                if (formatterIndentTypeKey == (int)IndentType.Tabs)
                {
                    formattingOptions.InsertSpaces = false;
                    formattingOptions.TabSize = await bicepSettings.GetIntegerAsync(BicepLanguageServerClientConstants.FormatterTabSizeKey, 2);
                }
                else
                {
                    formattingOptions.InsertSpaces = true;
                    formattingOptions.TabSize = await bicepSettings.GetIntegerAsync(BicepLanguageServerClientConstants.FormatterIndentSizeKey, 2);
                }

                documentFormattingParams.Options = formattingOptions;
                methodParam = JToken.FromObject(documentFormattingParams);
            }

            return methodParam;
        }
    }
}
