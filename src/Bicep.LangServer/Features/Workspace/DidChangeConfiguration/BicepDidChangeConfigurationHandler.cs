// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.LanguageServer.Settings;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;

namespace Bicep.LanguageServer.Features.Workspace.DidChangeConfiguration
{
    [Method("workspace/didChangeConfiguration", Direction.ClientToServer)]
    public record DidChangeConfigurationParams : IRequest
    {
        [JsonProperty("settings")]
        public JToken? Settings;
    }

    public class BicepDidChangeConfigurationHandler : IJsonRpcNotificationHandler<DidChangeConfigurationParams>
    {
        private readonly ISettingsProvider settingsProvider;

        public BicepDidChangeConfigurationHandler(ISettingsProvider settingsProvider)
        {
            this.settingsProvider = settingsProvider;
        }

        Task<Unit> IRequestHandler<DidChangeConfigurationParams, Unit>.Handle(DidChangeConfigurationParams request, CancellationToken cancellationToken)
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
