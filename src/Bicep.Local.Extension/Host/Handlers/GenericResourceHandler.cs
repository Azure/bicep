// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;
using System.Text.Json.Nodes;

namespace Bicep.Local.Extension.Host.Handlers;

public abstract class GenericResourceHandler : GenericResourceHandler<GenericResourceHandler.EmptyConfig>
{
    protected override EmptyConfig GetConfig(string configJson) => new();

    public class EmptyConfig { }
}

public abstract class GenericResourceHandler<TConfig> : ResourceHandler<JsonObject, JsonObject, TConfig>
    where TConfig : class
{
    public sealed override string? ApiVersion => null;

    public sealed override string? Type => null;
}
