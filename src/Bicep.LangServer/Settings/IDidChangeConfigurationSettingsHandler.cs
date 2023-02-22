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
    ///  Handles "workspace/didChangeConfiguration" messages.
    /// </summary>
    [Method("workspace/didChangeConfiguration")]
    public interface IDidChangeConfigurationSettingsHandler
        : IJsonRpcNotificationHandler<DidChangeConfigurationObjectParams>, IJsonRpcHandler, IRegistration<object>, ICapability<DidChangeConfigurationCapability>
    {
    }

    public class DidChangeConfigurationObjectParams : IRequest
    {
        /// <summary>
        ///  The current settings.
        /// </summary>
        [JsonProperty("settings")]
        public JToken? Settings;
    }
}
