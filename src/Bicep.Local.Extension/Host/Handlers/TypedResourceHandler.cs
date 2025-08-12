// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Reflection;
using System.Text.Json;
using Bicep.Local.Extension.Types.Attributes;

namespace Bicep.Local.Extension.Host.Handlers;

public abstract class TypedResourceHandler<TProperties, TIdentifiers> : TypedResourceHandler<TProperties, TIdentifiers, TypedResourceHandler<TProperties, TIdentifiers>.EmptyConfig>
    where TProperties : class
    where TIdentifiers : class
{
    protected override EmptyConfig GetConfig(string configJson) => new();

    public class EmptyConfig { }
}

public abstract class TypedResourceHandler<TProperties, TIdentifiers, TConfig> : ResourceHandler<TProperties, TIdentifiers, TConfig>
    where TProperties : class
    where TIdentifiers : class
    where TConfig : class
{
    private static ResourceTypeAttribute ResourceTypeAttribute { get; } = typeof(TProperties).GetCustomAttribute<ResourceTypeAttribute>()
        ?? throw new InvalidOperationException($"The type {typeof(TProperties).FullName} must have a ResourceTypeAttribute defined.");

    public sealed override string? ApiVersion => ResourceTypeAttribute.ApiVersion;

    public sealed override string? Type => ResourceTypeAttribute.Name;
}
