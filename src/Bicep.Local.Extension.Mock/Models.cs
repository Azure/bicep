// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Bicep.Types.Concrete;
using Bicep.Local.Extension.Types.Attributes;

namespace Bicep.Local.Extension.Mock;

public class EchoResourceIdentifiers
{
}

[ResourceType("echo")]
public class EchoResource : EchoResourceIdentifiers
{
    [TypeProperty("The payload to echo back", ObjectTypePropertyFlags.Required)]
    public required string Payload { get; set; }
}

public class Configuration
{
    [TypeProperty("The foo configuration", ObjectTypePropertyFlags.None)]
    public string? Foo { get; set; }
}
