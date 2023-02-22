// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using MediatR;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bicep.LanguageServer.Settings
{
    public sealed class ConfigurationSettingsHandler: IDidChangeConfigurationSettingsHandler
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

        public object GetRegistrationOptions(ClientCapabilities clientCapabilities)
        {
            return null!;
        }

        public void SetCapability(DidChangeConfigurationCapability capability, ClientCapabilities clientCapabilities)
        {

        }
    }
}
