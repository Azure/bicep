// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json.Serialization;

namespace Bicep.Core.Configuration
{
    [JsonConverter(typeof(JsonStringEnumConverter<ManagedIdentityType>))]
    public enum ManagedIdentityType
    {
        SystemAssigned,

        UserAssigned,
    }
}
