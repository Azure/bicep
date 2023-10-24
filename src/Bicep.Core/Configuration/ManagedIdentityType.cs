// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Bicep.Core.Configuration
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ManagedIdentityType
    {
        SystemAssigned,

        UserAssigned,
    }
}
