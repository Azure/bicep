// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.LanguageServer.Handlers;
using MediatR;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bicep.LanguageServer.Settings
{
    [Method("workspace/didChangeConfiguration", Direction.ClientToServer)]
    public record DidChangeConfigurationObjectParams : IRequest
    {
        [JsonProperty("settings")]
        public JToken? Settings;
    }

    public class ConfigurationSettingsHandler: IJsonRpcNotificationHandler<DidChangeConfigurationObjectParams>
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
                bicepObject["IncludeAllAccessibleAzureContainerRegistriesForCompletions"] is JToken jToken)
            {
                settingsProvider.AddOrUpdateSetting(LangServerConstants.IncludeAllAccessibleAzureContainerRegistriesForCompletionsSetting, jToken.Value<bool>());
            }

            return Unit.Task;
        }
    }
}
