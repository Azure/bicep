// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;
using System.Text.Json.Nodes;
using Azure.Bicep.Types.Concrete;
using Bicep.Core.Json;
using Bicep.Core.Syntax;
using Bicep.Local.Extension.Host.Handlers;
using Bicep.Local.Extension.Types.Attributes;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json.Linq;

namespace Bicep.Local.Extension.Mock;

[ResourceType("echo")]
public class EchoResource
{
    [TypeProperty("The payload to echo back", ObjectTypePropertyFlags.Required)]
    public required string Payload { get; set; }
}

public class Configuration
{
    [TypeProperty("The foo configuration", ObjectTypePropertyFlags.Required)]
    public required string Foo { get; set; }
}