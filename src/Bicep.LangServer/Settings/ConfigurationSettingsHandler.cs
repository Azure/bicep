// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;

namespace Bicep.LanguageServer.Settings
{
    [Method("workspace/didChangeConfiguration", Direction.ClientToServer)]
    public record DidChangeConfigurationObjectParams : IRequest
    {
        [JsonProperty("settings")]
        public JToken? Settings;
    }

    /// <summary>
    /// Handles settings change notification from client.
    /// </summary>
    public class ConfigurationSettingsHandler : IJsonRpcNotificationHandler<DidChangeConfigurationObjectParams>
    {
        private readonly ISettingsProvider settingsProvider;

        public ConfigurationSettingsHandler(ISettingsProvider settingsProvider)
        {
            this.settingsProvider = settingsProvider;
        }

        Task<Unit> IRequestHandler<DidChangeConfigurationObjectParams, Unit>.Handle(DidChangeConfigurationObjectParams request, CancellationToken cancellationToken)
        {
            var jObject = JObject.FromObject(request);

            if (jObject["settings"] is JObject settingsObject &&
                settingsObject["bicep"] is JObject bicepObject &&
                bicepObject["completions"] is JObject completionsObject)
            {
                if (completionsObject[LangServerConstants.GetAllAzureContainerRegistriesForCompletionsSetting] is JToken getAllAccessibleAzureContainerRegistriesToken)
                {
                    settingsProvider.AddOrUpdateSetting(LangServerConstants.GetAllAzureContainerRegistriesForCompletionsSetting, getAllAccessibleAzureContainerRegistriesToken.Value<bool>());
                }
            }

            return Unit.Task;
        }
    }
}
