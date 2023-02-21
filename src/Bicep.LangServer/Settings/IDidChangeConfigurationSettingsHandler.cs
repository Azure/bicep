// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;

namespace Bicep.LanguageServer.Settings
{
    /// <summary>
    ///     Custom handler for "workspace/didChangeConfiguration" with the configuration as a <see cref="JObject"/>.
    /// </summary>
    [Method("workspace/didChangeConfiguration")]
    public interface IDidChangeConfigurationSettingsHandler
        : IJsonRpcNotificationHandler<DidChangeConfigurationObjectParams>, IJsonRpcHandler, IRegistration<object>, ICapability<DidChangeConfigurationCapability>
    {
    }

    /// <summary>
    ///     Notification parameters for "workspace/didChangeConfiguration".
    /// </summary>
    public class DidChangeConfigurationObjectParams : IRequest
    {
        /// <summary>
        ///     The current settings.
        /// </summary>
        [JsonProperty("settings")]
        public JToken? Settings;
    }
}
